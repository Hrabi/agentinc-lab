namespace AgenticLab.Web.Services;

/// <summary>
/// Configuration for an agent instance.
/// </summary>
public class AgentConfig
{
    /// <summary>
    /// Unique identifier for this agent configuration.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>
    /// Display name for this agent.
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// The agent type (e.g., "SimpleQuestion", "DataExtractor", "Summarizer").
    /// </summary>
    public string AgentType { get; set; } = "SimpleQuestion";

    /// <summary>
    /// The ID of the model configuration to use.
    /// </summary>
    public string ModelConfigId { get; set; } = "";

    /// <summary>
    /// Override system prompt for this agent (null = use agent default).
    /// </summary>
    public string? SystemPromptOverride { get; set; }

    /// <summary>
    /// Override temperature (null = use model default).
    /// </summary>
    public double? TemperatureOverride { get; set; }

    /// <summary>
    /// Override max tokens (null = use model default).
    /// </summary>
    public int? MaxTokensOverride { get; set; }

    /// <summary>
    /// Whether this agent configuration is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When this configuration was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
