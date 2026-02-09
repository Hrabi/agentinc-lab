namespace AgenticLab.Web.Services;

/// <summary>
/// Configuration for an LLM model instance.
/// </summary>
public class ModelConfig
{
    /// <summary>
    /// Unique identifier for this model configuration.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>
    /// Display name for this model configuration.
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// The provider type (e.g., "ollama", "azure-openai").
    /// </summary>
    public string Provider { get; set; } = "ollama";

    /// <summary>
    /// The model name/identifier (e.g., "llama3.2", "qwen2.5:14b").
    /// </summary>
    public string ModelName { get; set; } = "llama3.2";

    /// <summary>
    /// The endpoint URL for the model provider.
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Default temperature for this model (0.0-1.0).
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Default maximum tokens to generate.
    /// </summary>
    public int MaxTokens { get; set; } = 1000;

    /// <summary>
    /// Default system prompt.
    /// </summary>
    public string SystemPrompt { get; set; } = "You are a helpful assistant.";

    /// <summary>
    /// Whether this configuration is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When this configuration was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
