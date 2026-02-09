using System.Net.Http.Json;
using System.Text.Json;
using AgenticLab.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace AgenticLab.Models.Ollama;

/// <summary>
/// LLM model adapter for Ollama (local LLM execution).
/// </summary>
public class OllamaModel : IModel
{
    private readonly HttpClient _httpClient;
    private readonly string _modelName;
    private readonly ILogger<OllamaModel> _logger;

    public string Name => $"ollama:{_modelName}";

    public OllamaModel(string endpoint, string modelName, ILogger<OllamaModel> logger, HttpClient? httpClient = null)
    {
        _modelName = modelName;
        _logger = logger;
        _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(endpoint) };
    }

    public async Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating response with Ollama model: {Model}", _modelName);

        var ollamaRequest = new
        {
            model = _modelName,
            prompt = request.Prompt,
            system = request.SystemPrompt,
            stream = false,
            options = new
            {
                temperature = request.Temperature,
                num_predict = request.MaxTokens
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", ollamaRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        return new ModelResponse
        {
            Text = json.GetProperty("response").GetString() ?? string.Empty,
            ModelName = _modelName,
            PromptTokens = json.TryGetProperty("prompt_eval_count", out var promptTokens) ? promptTokens.GetInt32() : 0,
            CompletionTokens = json.TryGetProperty("eval_count", out var completionTokens) ? completionTokens.GetInt32() : 0
        };
    }
}
