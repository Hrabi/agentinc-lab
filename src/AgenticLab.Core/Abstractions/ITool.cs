namespace AgenticLab.Core.Abstractions;

/// <summary>
/// Represents a tool that an agent can invoke to perform actions.
/// </summary>
public interface ITool
{
    /// <summary>
    /// The unique name of this tool.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A human-readable description of what this tool does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Execute the tool with the given input.
    /// </summary>
    Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken cancellationToken = default);
}
