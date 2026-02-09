using AgenticLab.Core.Abstractions;

namespace AgenticLab.Agents;

/// <summary>
/// A simple agent that answers questions using a configured LLM model.
/// </summary>
public class SimpleQuestionAgent : IAgent
{
    private readonly IModel _model;

    public string Name => "SimpleQuestion";
    public string Description => "Answers simple questions using a configured LLM model.";

    public SimpleQuestionAgent(IModel model)
    {
        _model = model;
    }

    public async Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var modelRequest = new ModelRequest
        {
            Prompt = request.Message,
            SystemPrompt = "You are a helpful assistant. Answer questions clearly and concisely.",
            MaxTokens = 500,
            Temperature = 0.7
        };

        var response = await _model.GenerateAsync(modelRequest, cancellationToken);

        return new AgentResponse
        {
            AgentName = Name,
            Message = response.Text,
            Success = true,
            Metadata = new Dictionary<string, object>
            {
                ["model"] = response.ModelName ?? "unknown",
                ["promptTokens"] = response.PromptTokens,
                ["completionTokens"] = response.CompletionTokens
            }
        };
    }
}
