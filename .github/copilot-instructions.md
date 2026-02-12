# GitHub Copilot Instructions for AgenticLab

> **AI Mode:** Agent mode (Claude Opus 4.6)  
> **Last updated:** February 2026

## Project Context

AgenticLab is a **.NET 10 / C# 14** learning/experimentation project for building **agentic AI systems** with **local-first LLM execution** via [Ollama](https://ollama.com/) v0.15.6 and optional hybrid cloud (Azure OpenAI). This is a **lab**, not a product — experiments are expected.

- **Runtime:** .NET 10 SDK (`net10.0` via shared `Directory.Build.props`)
- **Local LLM:** Ollama at `http://localhost:11434` — models: `llama3.2` (demo), `qwen2.5:14b` (dev)
- **GPU:** NVIDIA RTX 5090 24 GB, Docker Desktop with NVIDIA Container Toolkit

---

## Architecture (8 projects)

| Project | Purpose | Key Types |
|---------|---------|-----------|
| **Core** | Abstractions + data models (zero dependencies) | `IAgent`, `IModel`, `ITool`, `IModelRouter`, request/response models |
| **Runtime** | Agent orchestration — register by name, route `SendAsync`, error wrapping | `AgentRuntime` |
| **Models** | LLM adapters (HTTP-based) | `OllamaModel` ✅, `AzureOpenAIModel` 🚧 |
| **Agents** | Agent implementations + factory | `ConfigurableAgent`, `SpecialistAgents` (static factory, 8 types), `SimpleQuestionAgent` |
| **Web** | Blazor Server app (Fluent UI v4) — dashboard, playground, compare, examples | 6 services, 8 pages |
| **Demos** | Console entry point — DI setup, interactive chat loop | `Program.cs` (top-level statements) |
| **AppHost** | .NET Aspire orchestration | `Program.cs` |
| **ServiceDefaults** | Shared Aspire defaults (OpenTelemetry, health checks) | `Extensions.cs` |

### Dependency Flow

```
Core ← Runtime, Models, Agents
Core + Runtime + Models + Agents ← Web, Demos
Web ← AppHost (Aspire orchestration)
```

### Key Data Flow

```
User → AgentRequest → ConfigurableAgent → ModelRequest → OllamaModel → POST /api/generate → Ollama
                                                                                ↓
User ← AgentResponse ← Agent ← ModelResponse ← JSON ← Ollama
```

In the Web app: `AgentConfig` → `AgentFactoryService.CreateAgent()` → `SpecialistAgents.Create()` → `ConfigurableAgent`. Parameter overrides (temperature, topP, etc.) flow via `AgentRequest.Metadata` dictionary.

---

## Coding Conventions

### Must-follow rules

- **C# 14 / .NET 10** — use latest features: file-scoped namespaces, primary constructors, collection expressions
- **`TreatWarningsAsErrors`** is enabled globally — zero warnings allowed
- **`CancellationToken cancellationToken = default`** on all async methods (full name, not `ct`)
- **XML doc comments** (`<summary>`) on all public members
- **`required` + `init`** for Core data models (immutable); Web service POCOs use `{ get; set; }` (mutable)
- **Constructor injection** for dependencies (`IModel`, `ILogger<T>`, `HttpClient`)
- **File-scoped namespaces**, one type per file, filename matches type name

### Two data model styles (intentional)

| Layer | Style | Example |
|-------|-------|---------|
| `Core/Models/` | `required` + `init` (immutable) | `AgentRequest`, `ModelRequest`, `ModelResponse` |
| `Web/Services/` | `{ get; set; }` (mutable POCOs) | `ModelConfig`, `AgentConfig`, `ChatSession` |

### Agent pattern — ConfigurableAgent (preferred)

New agents should use the `SpecialistAgents` factory + `ConfigurableAgent`. Add a system prompt to `SpecialistAgents.SystemPrompts` and a description in `SpecialistAgents.Create()`:

```csharp
// In SpecialistAgents.cs — add to SystemPrompts dictionary:
["MySpecialist"] = """
    You are an expert at X. Instructions: ...
    """,

// In Create() — add to descriptions dictionary:
["MySpecialist"] = "Does X with precision.",
```

`ConfigurableAgent` handles metadata overrides (temperature, topP, topK, repeatPenalty, numCtx, seed) automatically from `AgentRequest.Metadata`.

### Tool pattern

```csharp
public class MyTool : ITool
{
    public string Name => "MyTool";
    public string Description => "Does a specific action.";
    public async Task<ToolResult> ExecuteAsync(ToolInput input, CancellationToken cancellationToken = default)
    {
        return new ToolResult { Success = true, Output = "result" };
    }
}
```

---

## Web Application (Blazor Server + Fluent UI)

**Stack:** Blazor Server (.NET 10), `Microsoft.FluentUI.AspNetCore.Components` v4, Interactive Server render mode.

### Service lifetimes (critical for DI)

| Service | Lifetime | Why |
|---------|----------|-----|
| `ModelRegistryService` | **Singleton** | Shared model config registry, queries Ollama `/api/tags` |
| `AgentFactoryService` | **Singleton** | Shared agent config registry + factory |
| `ExampleService` | **Singleton** | Static demo data (26 examples, 10 categories) |
| `ExportService` | **Singleton** | Stateless format conversion |
| `ChatService` | **Scoped** | Per-circuit chat sessions + history |
| `CompareService` | **Scoped** | Per-circuit multi-agent comparison |

### HttpClient pattern

Web uses `IHttpClientFactory` with a named client `"Ollama"`, configured from `appsettings.json`:

```json
{ "Ollama": { "Endpoint": "http://localhost:11434", "TimeoutMinutes": 5 } }
```

`OllamaModel` constructor accepts optional `HttpClient?` for this integration.

### State management

All state is **in-memory** — model configs, agent configs, chat sessions are seeded in constructors and stored in `List<T>`. No database. Ship with 6 model configs + 16 agent configs as defaults.

### Component tree

```
Components/App.razor → Routes.razor → Layout/MainLayout.razor (FluentLayout + nav)
Pages: Home(/), Models(/models), Agents(/agents), Playground(/playground),
       Compare(/compare), Examples(/examples), Export(/export), Learn(/learn)
```

---

## Ollama Integration

- **Generation:** `POST /api/generate` with `stream: false` — `OllamaModel.GenerateAsync()`
- **Model listing:** `GET /api/tags` — `ModelRegistryService.GetAvailableModelsAsync()`
- **Health check:** `GET /api/tags` — `ModelRegistryService.IsOllamaOnlineAsync()`
- **Embeddings (planned):** `POST /api/embed` with `nomic-embed-text` — see `docs/architecture/rag-pipeline.md`

### ModelRequest properties (all passed to Ollama `options`)

`Prompt` (required), `SystemPrompt`, `MaxTokens` → `num_predict`, `Temperature`, `TopP` → `top_p`, `TopK` → `top_k`, `RepeatPenalty` → `repeat_penalty`, `NumCtx` → `num_ctx`, `Seed`, `Stop`

### Model naming convention

`OllamaModel.Name` returns `"ollama:{modelName}"` (e.g., `"ollama:qwen2.5:14b"`).

---

## Infrastructure

### Docker (`infra/docker/docker-compose.yml`)

- **Default:** Ollama with GPU passthrough (port 11434)
- **Profile `vllm`:** vLLM OpenAI-compatible API (port 8000)
- **Profile `webui`:** Open WebUI chat interface (port 3000)
- **Init service:** Pulls `qwen2.5:14b`, `llama3.2`, `nomic-embed-text`

### RAG pipeline (planned, documented)

Full design in `docs/architecture/rag-pipeline.md`. Demo data in `data/rag-demo/` (7 files: handbook, product catalog, C# code, API docs, financial report, FAQ). Planned stack: Qdrant vector DB (Docker) + `OllamaEmbeddingModel` + `IVectorStore` + `RagAgent`.

---

## Build & Run

```powershell
dotnet build src/AgenticLab.sln                  # Build (Ctrl+Shift+B in VS Code)
dotnet run --project src/AgenticLab.Web          # Web app → http://localhost:5210
dotnet run --project src/AgenticLab.Demos        # Console demo (needs Ollama + llama3.2)
```

VS Code tasks: `build` (default), `restore`, `clean`, `run demo`, `run web`  
VS Code launch configs: "AgenticLab Web", "AgenticLab AppHost (Aspire)", "AgenticLab Demo (Console)"

---

## Implementation Status

| Feature | Status |
|---------|--------|
| Core abstractions, AgentRuntime, OllamaModel, ConfigurableAgent, SpecialistAgents (8 types) | ✅ |
| Blazor Web app (Fluent UI): dashboard, models, agents, playground, compare, examples, export | ✅ |
| Docker Compose (Ollama + vLLM + WebUI), .NET Aspire AppHost | ✅ |
| Conversation history (ChatService passes History in AgentRequest) | ✅ |
| AzureOpenAIModel adapter | 🚧 Placeholder |
| RAG pipeline (embeddings + Qdrant + ingestion + RagAgent) | 📋 Designed |
| IModelRouter, tool-using agents, multi-agent collaboration | 📋 Planned |
