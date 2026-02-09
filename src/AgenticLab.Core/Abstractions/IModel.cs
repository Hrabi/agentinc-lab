namespace AgenticLab.Core.Abstractions;

/// <summary>
/// Represents an LLM model backend (local or cloud).
/// </summary>
public interface IModel
{
    /// <summary>
    /// The name of this model provider.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Generate a completion from the model.
    /// </summary>
    Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken cancellationToken = default);
}
