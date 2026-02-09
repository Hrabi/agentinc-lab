# GitHub Copilot Instructions for AgenticLab

> **AI Mode:** Agent mode (Claude Opus 4.6)  
> **Last updated:** February 2026

## Project Context

AgenticLab is a **.NET 10 / C# 14** learning and experimentation project for building **agentic AI systems**. It focuses on **local-first LLM execution** (primarily via [Ollama](https://ollama.com/) v0.15.6) with optional hybrid cloud (Azure OpenAI) capabilities. This is a **lab**, not a product — experiments are expected.

### Development Environment

- **OS:** Windows 11 Pro
- **Machine:** Lenovo 83EY · Intel Core Ultra 9 275HX (24 cores) · 192 GB DDR5 · NVIDIA RTX 5090 Laptop GPU 24 GB
- **IDE:** Visual Studio Code with C# Dev Kit + GitHub Copilot (Agent mode, Claude Opus 4.6)
- **Runtime:** .NET 10 SDK (`net10.0` target framework)
- **Local LLM:** Ollama v0.15.6 at `http://localhost:11434`
- **Default model:** `llama3.2` (demo) / `qwen2.5:14b` (recommended dev model)
- **Docker:** Docker Desktop with WSL2 backend, NVIDIA Container Toolkit for GPU passthrough

---

## Architecture

Five projects in `src/AgenticLab.sln`, with a shared `Directory.Build.props` that enforces `net10.0`, nullable, implicit usings, and `TreatWarningsAsErrors`:

| Project | Purpose | Key Types |
|---------|---------|-----------|
| **AgenticLab.Core** | Core abstractions and data models (no dependencies) | `IAgent`, `IModel`, `ITool`, `IModelRouter`, `AgentRequest`, `AgentResponse`, `ModelRequest`, `ModelResponse`, `ToolInput`, `ToolResult`, `ChatMessage` |
| **AgenticLab.Runtime** | Agent orchestration engine — registers agents by name, routes `SendAsync` calls, handles errors | `AgentRuntime` |
| **AgenticLab.Models** | LLM adapters — HTTP-based model backends | `OllamaModel` (✅ implemented), `AzureOpenAIModel` (🚧 placeholder) |
| **AgenticLab.Agents** | Concrete agent implementations | `SimpleQuestionAgent` |
| **AgenticLab.Demos** | Console entry point — DI setup, interactive chat loop | `Program.cs` (top-level statements) |

### Dependency Flow

```
Core ← Runtime (references Core)
Core ← Models  (references Core)
Core ← Agents  (references Core, Models)
All  ← Demos   (references all projects)
```

### NuGet Packages Used

- `Microsoft.Extensions.DependencyInjection` / `.Abstractions`
- `Microsoft.Extensions.Logging` / `.Abstractions` / `.Console`
- All models and agents accept dependencies via constructor injection

---

## Key Abstractions (from source code)

All abstractions live in `AgenticLab.Core.Abstractions` namespace with file-scoped namespaces.

### IAgent — autonomous agent contract

```csharp
public interface IAgent
{
    string Name { get; }
    string Description { get; }
    Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default);
}
```

### IModel — LLM backend contract

```csharp
public interface IModel
{
    string Name { get; }
    Task<ModelResponse> GenerateAsync(ModelRequest request, CancellationToken cancellationToken = default);
}
```

### ITool — tool/action contract

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken cancellationToken = default);
}
```

### IModelRouter — hybrid routing contract

```csharp
public interface IModelRouter
{
    IModel SelectModel(AgentRequest request);
}
```

### Data Models (record-style classes with `required` + `init`)

- **`AgentRequest`** — `Message` (required), `Metadata` (optional dict), `History` (optional `List<ChatMessage>`)
- **`AgentResponse`** — `AgentName` (required), `Message` (required), `Success`, `Metadata`
- **`ModelRequest`** — `Prompt` (required), `SystemPrompt`, `MaxTokens` (default 1000), `Temperature` (default 0.7)
- **`ModelResponse`** — `Text` (required), `ModelName`, `PromptTokens`, `CompletionTokens`
- **`ToolInput`** — `Action` (required), `Parameters` (optional dict)
- **`ToolResult`** — `Success`, `Output`, `Error`
- **`ChatMessage`** — `Role` (required), `Content` (required), `Name`

---

## Coding Conventions

### Language & Framework

- **C# 14** / **.NET 10** — use latest features: file-scoped namespaces, primary constructors, `extension` blocks, collection expressions, etc.
- Target framework: `net10.0` (set in `Directory.Build.props`)
- `TreatWarningsAsErrors` is enabled globally

### Patterns

- **Dependency injection** — all services registered in DI container; never use static classes for services
- **`async/await`** for all I/O operations
- **`CancellationToken cancellationToken = default`** parameter on all async methods (use full name `cancellationToken`, not abbreviated `ct`)
- **Interfaces** for abstractions (`IAgent`, `IModel`, `ITool`, `IModelRouter`)
- **`required`** keyword for mandatory properties on data objects
- **`init`** setters for immutable data objects
- **XML doc comments** (`<summary>`) on all public members — interfaces, classes, methods, and properties
- Follow **Microsoft's .NET naming conventions** (PascalCase for public members, camelCase with `_` prefix for private fields)
- **Constructor injection** for dependencies (e.g., `IModel`, `ILogger<T>`, `HttpClient`)
- **`Microsoft.Extensions.*`** packages for DI, logging, and configuration

### Code Style (observed in codebase)

- File-scoped namespaces (no braces around namespace)
- One type per file (class/interface name matches filename)
- Data model classes in `Models/` subfolder, interfaces in `Abstractions/` subfolder
- Agents receive `IModel` via constructor, build `ModelRequest` internally, return `AgentResponse`
- Model adapters own their `HttpClient` and serialize/deserialize JSON via `System.Net.Http.Json` and `System.Text.Json`
- `AgentRuntime` stores agents in a `Dictionary<string, IAgent>`, routes by name, catches and wraps exceptions
- Demo uses top-level statements (`Program.cs`) with `ServiceCollection` for DI setup

### Agent Implementation Pattern

When creating a new agent, follow this pattern (from `SimpleQuestionAgent`):

```csharp
public class MyAgent : IAgent
{
    private readonly IModel _model;

    public string Name => "MyAgent";
    public string Description => "Does something specific.";

    public MyAgent(IModel model) => _model = model;

    public async Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var modelRequest = new ModelRequest
        {
            Prompt = request.Message,
            SystemPrompt = "Your specialist system prompt here.",
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
```

### Tool Implementation Pattern

When creating a new tool:

```csharp
public class MyTool : ITool
{
    public string Name => "MyTool";
    public string Description => "Does a specific action.";

    public async Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken cancellationToken = default)
    {
        // Perform action based on input.Action and input.Parameters
        return new ToolResult { Success = true, Output = "result" };
    }
}
```

---

## Local LLM Setup (Ollama)

The project uses [Ollama](https://github.com/ollama/ollama) (v0.15.6, 162k+ GitHub stars, MIT license) as the primary local LLM runtime.

- **REST API:** `http://localhost:11434` — endpoints `/api/generate` (completions), `/api/chat` (chat)
- **OllamaModel adapter** uses `POST /api/generate` with `stream: false`
- **Model naming:** `Name` returns `"ollama:{modelName}"` (e.g., `"ollama:qwen2.5:14b"`)
- **Default demo model:** `llama3.2` (in `Program.cs`)
- **Recommended dev models:** `qwen2.5:14b` (best all-rounder), `gemma3` (fast 4B utility), `phi4-mini` (SLM)

### Docker Infrastructure (`infra/docker/`)

- `docker-compose.yml` provides: Ollama (default), vLLM (profile: `vllm`), Open WebUI (profile: `webui`)
- Ollama init service pulls `qwen2.5:14b`, `llama3.2`, `nomic-embed-text` on first run
- GPU passthrough via NVIDIA Container Toolkit
- vLLM provides OpenAI-compatible API at port 8000

### Configuration (`infra/local/appsettings.local.json`)

Uses this structure for model configuration:

```json
{
  "AgenticLab": {
    "Runtime": { "Mode": "local", "MaxConcurrentAgents": 4 },
    "Models": {
      "Default": { "Provider": "ollama", "Endpoint": "http://localhost:11434", "Model": "qwen2.5:14b" },
      "Fast":    { "Provider": "ollama", "Endpoint": "http://localhost:11434", "Model": "llama3.2" },
      "Embedding": { "Provider": "ollama", "Model": "nomic-embed-text" }
    },
    "Routing": { "Default": "Default", "SimpleClassification": "Fast" }
  }
}
```

---

## Documentation Structure

Documentation lives in `docs/` as Markdown files:

| Path | Purpose |
|------|---------|
| `docs/tutorials/01-getting-started.md` | Setup prerequisites and first run |
| `docs/tutorials/02-local-llm-setup.md` | Ollama, Docker, vLLM, ONNX setup with model selection guide |
| `docs/tutorials/03-first-agent.md` | Build and wire up a simple agent |
| `docs/architecture/local-agentic.md` | Local-only architecture patterns |
| `docs/architecture/hybrid-agentic.md` | Hybrid local + cloud architecture |
| `docs/architecture/diagrams/*.mmd` | Mermaid architecture diagrams |
| `docs/local-llm-rag-specialist-agents.md` | Presentation: specialist agents, RAG, BDI case study |
| `docs/demo-walkthrough.md` | Demo app explained — data flow, design patterns, roadmap |
| `docs/deck.md` | Markdown slide deck |
| `docs/notes/glossary.md` | Key terms and definitions |
| `docs/examples/example-index.md` | Code examples index |
| `infra/docker/README.md` | Docker Compose setup, model recommendations |

### Documentation Conventions

- Every doc references the **target machine specs** where relevant
- Model tables include **sizes from official Ollama library** (not estimated)
- Mermaid diagrams (`.mmd`) for architecture; inline Mermaid in Markdown for docs
- Tutorials are numbered sequentially (`01-`, `02-`, `03-`)
- Presentation deck (`local-llm-rag-specialist-agents.md`) uses "Slide N:" headers

---

## Design Principles

1. **Local-first** — design to work fully offline without cloud services
2. **Composable** — agents, tools, and models are independent and replaceable
3. **Testable** — all interfaces are mockable; DI-first design
4. **Configuration-driven** — use `appsettings.json`; avoid hardcoded values (demo excepted)
5. **Interface-based** — agents don't know their backing model; models don't know their callers
6. **Explicit boundaries** — agents speak `AgentRequest`/`AgentResponse`, models speak `ModelRequest`/`ModelResponse`
7. **Documentation next to code** — every folder with code should have context explaining its purpose

---

## Current Implementation Status

| Feature | Status |
|---------|--------|
| Core abstractions (IAgent, IModel, ITool, IModelRouter) | ✅ Implemented |
| AgentRuntime (register, send, error handling) | ✅ Implemented |
| OllamaModel adapter (HTTP, /api/generate) | ✅ Implemented |
| SimpleQuestionAgent | ✅ Implemented |
| Interactive console demo | ✅ Implemented |
| Docker Compose (Ollama + vLLM + WebUI) | ✅ Implemented |
| AzureOpenAIModel adapter | 🚧 Placeholder (throws NotImplementedException) |
| IModelRouter implementation | 📋 Planned |
| Tool-using agents (ITool integration) | 📋 Planned |
| Multi-agent collaboration | 📋 Planned |
| Conversation history/memory | 📋 Planned (AgentRequest.History defined but unused) |
| Configuration from appsettings.json | 📋 Planned (currently hardcoded in Program.cs) |
| RAG pipeline (embeddings + vector store) | 📋 Planned |
| BDI mapping agent (CRM → SAP/Navision) | 📋 Planned (designed in docs) |

---

## Build & Run

```powershell
# Build
dotnet build src/AgenticLab.sln

# Run demo (requires Ollama running with llama3.2)
dotnet run --project src/AgenticLab.Demos

# VS Code: tasks defined in .vscode/tasks.json
#   - build (default build task)
#   - restore
#   - clean
#   - run demo
```
