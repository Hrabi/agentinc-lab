using AgenticLab.Core.Abstractions;

namespace AgenticLab.Agents;

/// <summary>
/// A configurable agent that uses metadata overrides for system prompt, temperature, and max tokens.
/// Serves as the base for all specialist agent types.
/// </summary>
public class ConfigurableAgent : IAgent
{
    private readonly IModel _model;
    private readonly string _name;
    private readonly string _description;
    private readonly string _defaultSystemPrompt;
    private readonly double _defaultTemperature;
    private readonly int _defaultMaxTokens;

    /// <inheritdoc />
    public string Name => _name;

    /// <inheritdoc />
    public string Description => _description;

    /// <summary>
    /// Creates a new configurable agent.
    /// </summary>
    public ConfigurableAgent(
        IModel model,
        string name,
        string description,
        string defaultSystemPrompt,
        double defaultTemperature = 0.7,
        int defaultMaxTokens = 1000)
    {
        _model = model;
        _name = name;
        _description = description;
        _defaultSystemPrompt = defaultSystemPrompt;
        _defaultTemperature = defaultTemperature;
        _defaultMaxTokens = defaultMaxTokens;
    }

    /// <inheritdoc />
    public async Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var systemPrompt = _defaultSystemPrompt;
        var temperature = _defaultTemperature;
        var maxTokens = _defaultMaxTokens;
        double? topP = null;
        int? topK = null;
        double? repeatPenalty = null;
        int? numCtx = null;
        int? seed = null;

        // Apply metadata overrides if provided
        if (request.Metadata is not null)
        {
            if (request.Metadata.TryGetValue("systemPrompt", out var sp) && sp is string spStr && !string.IsNullOrWhiteSpace(spStr))
                systemPrompt = spStr;
            if (request.Metadata.TryGetValue("temperature", out var temp))
                temperature = Convert.ToDouble(temp);
            if (request.Metadata.TryGetValue("maxTokens", out var mt))
                maxTokens = Convert.ToInt32(mt);
            if (request.Metadata.TryGetValue("topP", out var tp))
                topP = Convert.ToDouble(tp);
            if (request.Metadata.TryGetValue("topK", out var tk))
                topK = Convert.ToInt32(tk);
            if (request.Metadata.TryGetValue("repeatPenalty", out var rp))
                repeatPenalty = Convert.ToDouble(rp);
            if (request.Metadata.TryGetValue("numCtx", out var nc))
                numCtx = Convert.ToInt32(nc);
            if (request.Metadata.TryGetValue("seed", out var s))
                seed = Convert.ToInt32(s);
        }

        var modelRequest = new ModelRequest
        {
            Prompt = request.Message,
            SystemPrompt = systemPrompt,
            MaxTokens = maxTokens,
            Temperature = temperature,
            TopP = topP,
            TopK = topK,
            RepeatPenalty = repeatPenalty,
            NumCtx = numCtx,
            Seed = seed
        };

        var response = await _model.GenerateAsync(modelRequest, cancellationToken);

        return new AgentResponse
        {
            AgentName = Name,
            Message = response.Text,
            Success = true,
            Metadata = new Dictionary<string, object>
            {
                ["model"] = response.ModelName ?? "unknown",
                ["promptTokens"] = response.PromptTokens,
                ["completionTokens"] = response.CompletionTokens
            }
        };
    }
}
