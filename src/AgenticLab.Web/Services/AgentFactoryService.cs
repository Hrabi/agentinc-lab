using AgenticLab.Agents;
using AgenticLab.Core.Abstractions;
using AgenticLab.Models.Ollama;
using AgenticLab.Runtime;

namespace AgenticLab.Web.Services;

/// <summary>
/// Creates and manages agent instances based on configurations.
/// </summary>
public class AgentFactoryService
{
    private readonly ModelRegistryService _modelRegistry;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<AgentFactoryService> _logger;
    private readonly List<AgentConfig> _configs = [];

    public AgentFactoryService(
        ModelRegistryService modelRegistry,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        ILogger<AgentFactoryService> logger)
    {
        _modelRegistry = modelRegistry;
        _httpClientFactory = httpClientFactory;
        _loggerFactory = loggerFactory;
        _logger = logger;

        // Seed default agent configuration
        _configs.Add(new AgentConfig
        {
            Id = "default-qa",
            DisplayName = "Q&A Agent (Llama 3.2)",
            AgentType = "SimpleQuestion",
            ModelConfigId = "default-llama"
        });
    }

    /// <summary>
    /// Gets all agent configurations.
    /// </summary>
    public IReadOnlyList<AgentConfig> GetConfigs() => _configs.AsReadOnly();

    /// <summary>
    /// Gets an agent configuration by ID.
    /// </summary>
    public AgentConfig? GetConfig(string id) => _configs.Find(c => c.Id == id);

    /// <summary>
    /// Adds a new agent configuration.
    /// </summary>
    public AgentConfig AddConfig(AgentConfig config)
    {
        _configs.Add(config);
        _logger.LogInformation("Added agent config: {DisplayName} ({AgentType})", config.DisplayName, config.AgentType);
        return config;
    }

    /// <summary>
    /// Updates an existing agent configuration.
    /// </summary>
    public bool UpdateConfig(AgentConfig config)
    {
        var index = _configs.FindIndex(c => c.Id == config.Id);
        if (index < 0) return false;
        _configs[index] = config;
        return true;
    }

    /// <summary>
    /// Removes an agent configuration.
    /// </summary>
    public bool RemoveConfig(string id) => _configs.RemoveAll(c => c.Id == id) > 0;

    /// <summary>
    /// Gets the list of available agent types.
    /// </summary>
    public static IReadOnlyList<AgentTypeInfo> GetAvailableAgentTypes() =>
    [
        new("SimpleQuestion", "Simple Q&A", "Answers questions using a configured LLM model."),
        new("Summarizer", "Summarizer", "Summarizes long text into concise output."),
        new("DataExtractor", "Data Extractor", "Extracts structured data from unstructured text."),
        new("CodeGenerator", "Code Generator", "Generates code based on natural language descriptions."),
        new("Translator", "Translator", "Translates text between languages."),
        new("Classifier", "Classifier", "Classifies text into predefined categories.")
    ];

    /// <summary>
    /// Creates a live IAgent instance from a configuration.
    /// </summary>
    public IAgent? CreateAgent(AgentConfig agentConfig)
    {
        var modelConfig = _modelRegistry.GetConfig(agentConfig.ModelConfigId);
        if (modelConfig is null)
        {
            _logger.LogWarning("Model config {ModelConfigId} not found for agent {AgentId}", agentConfig.ModelConfigId, agentConfig.Id);
            return null;
        }

        var model = CreateModel(modelConfig);

        return agentConfig.AgentType switch
        {
            "SimpleQuestion" => new SimpleQuestionAgent(model),
            "Summarizer" => new SimpleQuestionAgent(model), // Reuses SimpleQuestion with different system prompt for now
            "DataExtractor" => new SimpleQuestionAgent(model),
            "CodeGenerator" => new SimpleQuestionAgent(model),
            "Translator" => new SimpleQuestionAgent(model),
            "Classifier" => new SimpleQuestionAgent(model),
            _ => null
        };
    }

    /// <summary>
    /// Creates a live IModel instance from a model configuration.
    /// </summary>
    public IModel CreateModel(ModelConfig config)
    {
        return config.Provider switch
        {
            "ollama" => new OllamaModel(
                config.Endpoint,
                config.ModelName,
                _loggerFactory.CreateLogger<OllamaModel>(),
                _httpClientFactory.CreateClient("Ollama")),
            _ => throw new NotSupportedException($"Provider '{config.Provider}' is not supported.")
        };
    }
}

/// <summary>
/// Describes an available agent type.
/// </summary>
public record AgentTypeInfo(string TypeId, string DisplayName, string Description);
