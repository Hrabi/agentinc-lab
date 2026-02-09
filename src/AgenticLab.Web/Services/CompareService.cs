using AgenticLab.Core.Abstractions;
using System.Diagnostics;

namespace AgenticLab.Web.Services;

/// <summary>
/// Runs the same prompt across multiple agent/model configurations and compares results.
/// </summary>
public class CompareService
{
    private readonly AgentFactoryService _agentFactory;
    private readonly ILogger<CompareService> _logger;

    public CompareService(AgentFactoryService agentFactory, ILogger<CompareService> logger)
    {
        _agentFactory = agentFactory;
        _logger = logger;
    }

    /// <summary>
    /// Runs a prompt across multiple agent configurations and returns comparison results.
    /// </summary>
    public async Task<CompareResult> CompareAsync(
        string prompt,
        IEnumerable<string> agentConfigIds,
        CancellationToken cancellationToken = default)
    {
        var result = new CompareResult { Prompt = prompt };
        var tasks = new List<Task<CompareEntry>>();

        foreach (var configId in agentConfigIds)
        {
            tasks.Add(RunSingleAsync(configId, prompt, cancellationToken));
        }

        var entries = await Task.WhenAll(tasks);
        result.Entries.AddRange(entries);

        return result;
    }

    private async Task<CompareEntry> RunSingleAsync(string configId, string prompt, CancellationToken cancellationToken)
    {
        var entry = new CompareEntry { AgentConfigId = configId };

        try
        {
            var config = _agentFactory.GetConfig(configId);
            if (config is null)
            {
                entry.Error = $"Agent config '{configId}' not found.";
                return entry;
            }

            entry.AgentDisplayName = config.DisplayName;
            var agent = _agentFactory.CreateAgent(config);
            if (agent is null)
            {
                entry.Error = $"Failed to create agent from config '{configId}'.";
                return entry;
            }

            var request = new AgentRequest { Message = prompt };

            var sw = Stopwatch.StartNew();
            var response = await agent.ProcessAsync(request, cancellationToken);
            sw.Stop();

            entry.Response = response.Message;
            entry.Success = response.Success;
            entry.DurationMs = sw.ElapsedMilliseconds;
            entry.PromptTokens = response.Metadata?.TryGetValue("promptTokens", out var pt) == true ? Convert.ToInt32(pt) : 0;
            entry.CompletionTokens = response.Metadata?.TryGetValue("completionTokens", out var ct) == true ? Convert.ToInt32(ct) : 0;
            entry.ModelName = response.Metadata?.TryGetValue("model", out var mn) == true ? mn?.ToString() : null;
        }
        catch (Exception ex)
        {
            entry.Error = ex.Message;
            _logger.LogError(ex, "Compare failed for config {ConfigId}", configId);
        }

        return entry;
    }
}

/// <summary>
/// Result of a comparison run across multiple agents.
/// </summary>
public class CompareResult
{
    public string Prompt { get; set; } = "";
    public List<CompareEntry> Entries { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// A single entry in a comparison result.
/// </summary>
public class CompareEntry
{
    public string AgentConfigId { get; set; } = "";
    public string AgentDisplayName { get; set; } = "";
    public string? ModelName { get; set; }
    public string? Response { get; set; }
    public string? Error { get; set; }
    public bool Success { get; set; }
    public long DurationMs { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
}
