# Tutorial 03: Building Your First Agent

> Create a simple agent that uses a local LLM to answer questions.

---

## Overview

In this tutorial, you'll build a basic agent that:

1. Accepts a user question
2. Sends it to a local LLM
3. Returns the response

This introduces the core abstractions: `IAgent`, `ITool`, and `IModel`.

---

## Step 1: Understand the Abstractions

### IAgent

```csharp
public interface IAgent
{
    string Name { get; }
    string Description { get; }
    Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken ct = default);
}
```

### IModel

```csharp
public interface IModel
{
    string Name { get; }
    Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken ct = default);
}
```

### ITool

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken ct = default);
}
```

---

## Step 2: Create a Simple Agent

```csharp
public class SimpleQuestionAgent : IAgent
{
    private readonly IModel _model;

    public string Name => "SimpleQuestion";
    public string Description => "Answers simple questions using a local LLM";

    public SimpleQuestionAgent(IModel model)
    {
        _model = model;
    }

    public async Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken ct = default)
    {
        var modelRequest = new ModelRequest
        {
            Prompt = request.Message,
            MaxTokens = 500,
            Temperature = 0.7
        };

        var response = await _model.GenerateAsync(modelRequest, ct);

        return new AgentResponse
        {
            AgentName = Name,
            Message = response.Text,
            Success = true
        };
    }
}
```

---

## Step 3: Wire It Up

In `Program.cs`:

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register model
services.AddSingleton<IModel>(new OllamaModel(
    endpoint: "http://localhost:11434",
    model: "llama3.2"
));

// Register agent
services.AddTransient<IAgent, SimpleQuestionAgent>();

var provider = services.BuildServiceProvider();
var agent = provider.GetRequiredService<IAgent>();

// Process a request
var response = await agent.ProcessAsync(new AgentRequest
{
    Message = "What are the benefits of agentic systems?"
});

Console.WriteLine($"[{response.AgentName}]: {response.Message}");
```

---

## Step 4: Run It

```bash
# Make sure Ollama is running with a model loaded
ollama run llama3.2

# In a new terminal:
dotnet run --project src/AgenticLab.Demos
```

---

## What You Learned

- The three core abstractions: Agent, Model, Tool
- How to create a simple agent
- How to use dependency injection to wire components
- How to connect to a local LLM

---

## Exercises

1. **Add temperature control** — Let the user specify temperature via configuration
2. **Add a system prompt** — Give the agent a persona
3. **Add a tool** — Create a `DateTimeTool` that returns the current date/time
4. **Add error handling** — Handle model connection failures gracefully

---

## Next Steps

- Explore the [Core Project](../../src/AgenticLab.Core/) for the full set of abstractions
- See [Local Architecture](../architecture/local-agentic.md) for system design patterns
- Check the [Examples Index](../examples/example-index.md) for more code samples
