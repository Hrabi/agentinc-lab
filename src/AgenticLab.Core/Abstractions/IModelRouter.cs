namespace AgenticLab.Core.Abstractions;

/// <summary>
/// Routes model requests to the appropriate model provider (local or cloud).
/// </summary>
public interface IModelRouter
{
    /// <summary>
    /// Select the best model for a given request.
    /// </summary>
    IModel SelectModel(AgentRequest request);
}
