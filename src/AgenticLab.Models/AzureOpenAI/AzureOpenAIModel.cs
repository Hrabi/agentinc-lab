// Placeholder for Azure OpenAI model adapter
// This will be implemented when cloud integration is added.

using AgenticLab.Core.Abstractions;

namespace AgenticLab.Models.AzureOpenAI;

/// <summary>
/// LLM model adapter for Azure OpenAI Service.
/// </summary>
public class AzureOpenAIModel : IModel
{
    public string Name => "azure-openai";

    public Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement Azure OpenAI integration
        throw new NotImplementedException("Azure OpenAI adapter is not yet implemented. See docs/architecture/hybrid-agentic.md for the planned architecture.");
    }
}
