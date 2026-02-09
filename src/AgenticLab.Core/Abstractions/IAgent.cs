namespace AgenticLab.Core.Abstractions;

/// <summary>
/// Represents an autonomous agent that can receive tasks and produce results.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// The unique name of this agent.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A human-readable description of what this agent does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Process an incoming request and return a response.
    /// </summary>
    Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default);
}
