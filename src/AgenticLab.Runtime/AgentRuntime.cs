using AgenticLab.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace AgenticLab.Runtime;

/// <summary>
/// The main orchestration engine for running agents.
/// </summary>
public class AgentRuntime
{
    private readonly Dictionary<string, IAgent> _agents = new();
    private readonly ILogger<AgentRuntime> _logger;

    public AgentRuntime(ILogger<AgentRuntime> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Register an agent with the runtime.
    /// </summary>
    public void RegisterAgent(IAgent agent)
    {
        _agents[agent.Name] = agent;
        _logger.LogInformation("Registered agent: {AgentName}", agent.Name);
    }

    /// <summary>
    /// Send a request to a named agent.
    /// </summary>
    public async Task<AgentResponse> SendAsync(string agentName, AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_agents.TryGetValue(agentName, out var agent))
        {
            throw new InvalidOperationException($"Agent '{agentName}' is not registered.");
        }

        _logger.LogInformation("Sending request to agent: {AgentName}", agentName);

        try
        {
            var response = await agent.ProcessAsync(request, cancellationToken);
            _logger.LogInformation("Agent '{AgentName}' responded. Success: {Success}", agentName, response.Success);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent '{AgentName}' failed to process request.", agentName);
            return new AgentResponse
            {
                AgentName = agentName,
                Message = $"Error: {ex.Message}",
                Success = false
            };
        }
    }

    /// <summary>
    /// List all registered agents.
    /// </summary>
    public IReadOnlyCollection<string> GetRegisteredAgents() => _agents.Keys.ToList().AsReadOnly();
}
