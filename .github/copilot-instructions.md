# GitHub Copilot Instructions for AgenticLab

## Project Context

AgenticLab is a .NET-based learning and experimentation project for building agentic AI systems. It focuses on local-first LLM execution with optional hybrid cloud (Azure) capabilities.

## Architecture

- **AgenticLab.Core** — Core abstractions: `IAgent`, `ITool`, `IModel`, `IModelRouter`
- **AgenticLab.Runtime** — Agent orchestration engine
- **AgenticLab.Models** — LLM adapters (Ollama, Azure OpenAI)
- **AgenticLab.Agents** — Concrete agent implementations
- **AgenticLab.Demos** — Console and WebAPI demos

## Coding Conventions

- Use **C# 14** / **.NET 10** features (file-scoped namespaces, primary constructors, extensions, etc.)
- Use **dependency injection** — never use static classes for services
- Use **`async/await`** for all I/O operations
- Use **`CancellationToken`** in all async methods
- Prefer **interfaces** for abstractions (`IAgent`, `IModel`, `ITool`)
- Use **`required`** keyword for mandatory properties
- Use **`init`** setters for immutable data objects
- XML doc comments on all public members
- Follow Microsoft's .NET naming conventions

## Key Abstractions

When implementing new agents, always implement `IAgent`:
```csharp
public interface IAgent
{
    string Name { get; }
    string Description { get; }
    Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken ct = default);
}
```

When implementing new model adapters, implement `IModel`:
```csharp
public interface IModel
{
    string Name { get; }
    Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken ct = default);
}
```

When implementing new tools, implement `ITool`:
```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken ct = default);
}
```

## Preferences

- Local-first: design to work without cloud services
- Composable: keep agents, tools, and models independent
- Testable: design for unit testing with mocks
- Configuration-driven: use `appsettings.json` patterns
- Use `Microsoft.Extensions.*` packages for DI, logging, configuration

## Documentation

- Documentation lives in `docs/` as Markdown files
- Architecture diagrams use Mermaid format (`.mmd` files)
- Every folder with code should have context explaining its purpose
