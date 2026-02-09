using AgenticLab.Agents;
using AgenticLab.Core.Abstractions;
using AgenticLab.Models.Ollama;

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

        // Seed agent configurations — precise and fast variants for each specialist type
        _configs.AddRange([
            // Q&A
            new AgentConfig
            {
                Id = "qa-precise",
                DisplayName = "Q&A (Precise)",
                AgentType = "SimpleQuestion",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.2,
                MaxTokensOverride = 1500
            },
            new AgentConfig
            {
                Id = "qa-fast",
                DisplayName = "Q&A (Fast)",
                AgentType = "SimpleQuestion",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.8,
                MaxTokensOverride = 500
            },

            // Summarizer
            new AgentConfig
            {
                Id = "sum-precise",
                DisplayName = "Summarizer (Precise)",
                AgentType = "Summarizer",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.2,
                MaxTokensOverride = 1000
            },
            new AgentConfig
            {
                Id = "sum-fast",
                DisplayName = "Summarizer (Fast)",
                AgentType = "Summarizer",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.7,
                MaxTokensOverride = 500
            },

            // Data Extractor
            new AgentConfig
            {
                Id = "extract-precise",
                DisplayName = "Data Extractor (Precise)",
                AgentType = "DataExtractor",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.1,
                MaxTokensOverride = 2000
            },
            new AgentConfig
            {
                Id = "extract-fast",
                DisplayName = "Data Extractor (Fast)",
                AgentType = "DataExtractor",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.3,
                MaxTokensOverride = 1000
            },

            // Code Generator
            new AgentConfig
            {
                Id = "code-precise",
                DisplayName = "Code Generator (Precise)",
                AgentType = "CodeGenerator",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.2,
                MaxTokensOverride = 2000
            },
            new AgentConfig
            {
                Id = "code-fast",
                DisplayName = "Code Generator (Fast)",
                AgentType = "CodeGenerator",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.5,
                MaxTokensOverride = 1000
            },

            // Translator
            new AgentConfig
            {
                Id = "translate-precise",
                DisplayName = "Translator (Precise)",
                AgentType = "Translator",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.2,
                MaxTokensOverride = 1500
            },
            new AgentConfig
            {
                Id = "translate-fast",
                DisplayName = "Translator (Fast)",
                AgentType = "Translator",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.5,
                MaxTokensOverride = 800
            },

            // Classifier
            new AgentConfig
            {
                Id = "classify-precise",
                DisplayName = "Classifier (Precise)",
                AgentType = "Classifier",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.1,
                MaxTokensOverride = 1000
            },
            new AgentConfig
            {
                Id = "classify-fast",
                DisplayName = "Classifier (Fast)",
                AgentType = "Classifier",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.3,
                MaxTokensOverride = 500
            },

            // Format Converter
            new AgentConfig
            {
                Id = "convert-precise",
                DisplayName = "Format Converter (Precise)",
                AgentType = "FormatConverter",
                ModelConfigId = "precise-qwen",
                TemperatureOverride = 0.1,
                MaxTokensOverride = 2000
            },
            new AgentConfig
            {
                Id = "convert-fast",
                DisplayName = "Format Converter (Fast)",
                AgentType = "FormatConverter",
                ModelConfigId = "fast-llama",
                TemperatureOverride = 0.3,
                MaxTokensOverride = 1000
            },

            // Creative Writer (temperature comparison)
            new AgentConfig
            {
                Id = "creative-low",
                DisplayName = "Creative Writer (Low Temp)",
                AgentType = "CreativeWriter",
                ModelConfigId = "default-llama",
                TemperatureOverride = 0.2,
                MaxTokensOverride = 1000
            },
            new AgentConfig
            {
                Id = "creative-high",
                DisplayName = "Creative Writer (High Temp)",
                AgentType = "CreativeWriter",
                ModelConfigId = "creative-llama",
                TemperatureOverride = 1.0,
                MaxTokensOverride = 1000
            }
        ]);
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
        new("SimpleQuestion", "Simple Q&A", "Answers questions using a configured LLM model.", SpecialistAgents.GetDefaultSystemPrompt("SimpleQuestion")),
        new("Summarizer", "Summarizer", "Summarizes long text into concise output.", SpecialistAgents.GetDefaultSystemPrompt("Summarizer")),
        new("DataExtractor", "Data Extractor", "Extracts structured data from unstructured text.", SpecialistAgents.GetDefaultSystemPrompt("DataExtractor")),
        new("CodeGenerator", "Code Generator", "Generates code based on natural language descriptions.", SpecialistAgents.GetDefaultSystemPrompt("CodeGenerator")),
        new("Translator", "Translator", "Translates text between languages.", SpecialistAgents.GetDefaultSystemPrompt("Translator")),
        new("Classifier", "Classifier", "Classifies text into predefined categories.", SpecialistAgents.GetDefaultSystemPrompt("Classifier")),
        new("FormatConverter", "Format Converter", "Converts data between formats (JSON, YAML, XML, etc.).", SpecialistAgents.GetDefaultSystemPrompt("FormatConverter")),
        new("CreativeWriter", "Creative Writer", "Generates creative content with variable temperature.", SpecialistAgents.GetDefaultSystemPrompt("CreativeWriter"))
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

        return SpecialistAgents.Create(agentConfig.AgentType, model);
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
public record AgentTypeInfo(string TypeId, string DisplayName, string Description, string SystemPrompt);
