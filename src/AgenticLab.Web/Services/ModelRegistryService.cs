using System.Net.Http.Json;
using System.Text.Json;

namespace AgenticLab.Web.Services;

/// <summary>
/// Manages model configurations and provides access to available Ollama models.
/// </summary>
public class ModelRegistryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ModelRegistryService> _logger;
    private readonly List<ModelConfig> _configs = [];

    public ModelRegistryService(IHttpClientFactory httpClientFactory, ILogger<ModelRegistryService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        // Seed default configurations
        _configs.AddRange([
            new ModelConfig
            {
                Id = "default-llama",
                DisplayName = "Llama 3.2 (Default)",
                ModelName = "llama3.2",
                Temperature = 0.7,
                MaxTokens = 1000,
                SystemPrompt = "You are a helpful assistant. Answer questions clearly and concisely."
            },
            new ModelConfig
            {
                Id = "default-qwen",
                DisplayName = "Qwen 2.5 14B",
                ModelName = "qwen2.5:14b",
                Temperature = 0.7,
                MaxTokens = 2000,
                SystemPrompt = "You are a helpful assistant. Answer questions clearly and concisely."
            },
            new ModelConfig
            {
                Id = "precise-llama",
                DisplayName = "Llama 3.2 (Precise)",
                ModelName = "llama3.2",
                Temperature = 0.2,
                MaxTokens = 1500,
                SystemPrompt = "You are a helpful assistant. Be precise, accurate, and deterministic. Prefer factual, structured answers."
            },
            new ModelConfig
            {
                Id = "precise-qwen",
                DisplayName = "Qwen 2.5 Coder 14B (Precise)",
                ModelName = "qwen2.5-coder:14b",
                Temperature = 0.2,
                MaxTokens = 2000,
                SystemPrompt = "You are a helpful assistant. Be precise, accurate, and deterministic. Prefer factual, structured answers."
            },
            new ModelConfig
            {
                Id = "fast-llama",
                DisplayName = "Llama 3.2 (Fast)",
                ModelName = "llama3.2",
                Temperature = 0.8,
                MaxTokens = 500,
                SystemPrompt = "You are a helpful assistant. Be concise and respond quickly."
            },
            new ModelConfig
            {
                Id = "creative-llama",
                DisplayName = "Llama 3.2 (Creative)",
                ModelName = "llama3.2",
                Temperature = 1.0,
                MaxTokens = 1000,
                SystemPrompt = "You are a creative assistant. Be imaginative and expressive."
            }
        ]);
    }

    /// <summary>
    /// Gets all model configurations.
    /// </summary>
    public IReadOnlyList<ModelConfig> GetConfigs() => _configs.AsReadOnly();

    /// <summary>
    /// Gets a model configuration by ID.
    /// </summary>
    public ModelConfig? GetConfig(string id) => _configs.Find(c => c.Id == id);

    /// <summary>
    /// Adds a new model configuration.
    /// </summary>
    public ModelConfig AddConfig(ModelConfig config)
    {
        _configs.Add(config);
        _logger.LogInformation("Added model config: {DisplayName} ({ModelName})", config.DisplayName, config.ModelName);
        return config;
    }

    /// <summary>
    /// Updates an existing model configuration.
    /// </summary>
    public bool UpdateConfig(ModelConfig config)
    {
        var index = _configs.FindIndex(c => c.Id == config.Id);
        if (index < 0) return false;
        _configs[index] = config;
        return true;
    }

    /// <summary>
    /// Removes a model configuration.
    /// </summary>
    public bool RemoveConfig(string id) => _configs.RemoveAll(c => c.Id == id) > 0;

    /// <summary>
    /// Queries Ollama for available models.
    /// </summary>
    public async Task<List<OllamaModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            var response = await client.GetFromJsonAsync<JsonElement>("/api/tags", cancellationToken);

            var models = new List<OllamaModelInfo>();
            if (response.TryGetProperty("models", out var modelsArray))
            {
                foreach (var model in modelsArray.EnumerateArray())
                {
                    models.Add(new OllamaModelInfo
                    {
                        Name = model.GetProperty("name").GetString() ?? "",
                        Size = model.TryGetProperty("size", out var size) ? size.GetInt64() : 0,
                        ModifiedAt = model.TryGetProperty("modified_at", out var mod) ? mod.GetString() ?? "" : "",
                        Family = model.TryGetProperty("details", out var details) && details.TryGetProperty("family", out var fam)
                            ? fam.GetString() ?? "" : "",
                        ParameterSize = details.TryGetProperty("parameter_size", out var ps)
                            ? ps.GetString() ?? "" : "",
                        QuantizationLevel = details.TryGetProperty("quantization_level", out var ql)
                            ? ql.GetString() ?? "" : ""
                    });
                }
            }

            return models;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch Ollama models");
            return [];
        }
    }

    /// <summary>
    /// Checks if Ollama is reachable.
    /// </summary>
    public async Task<bool> IsOllamaOnlineAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            var response = await client.GetAsync("/", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Information about an Ollama model.
/// </summary>
public class OllamaModelInfo
{
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public string ModifiedAt { get; set; } = "";
    public string Family { get; set; } = "";
    public string ParameterSize { get; set; } = "";
    public string QuantizationLevel { get; set; } = "";

    public string FormattedSize => Size switch
    {
        > 1_000_000_000 => $"{Size / 1_000_000_000.0:F1} GB",
        > 1_000_000 => $"{Size / 1_000_000.0:F1} MB",
        _ => $"{Size} bytes"
    };
}
