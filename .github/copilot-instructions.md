# GitHub Copilot Instructions for AgenticLab

> **Last updated:** February 2026

## Project Context

AgenticLab is a **.NET 10 / C# 14** learning/experimentation lab for **agentic AI systems** with **local-first LLM execution** via Ollama and optional hybrid cloud (Azure OpenAI). This is a lab, not a product — experiments are expected.

- **Runtime:** .NET 10 SDK (`net10.0` via shared `Directory.Build.props`)
- **Local LLM:** Ollama at `http://localhost:11434` — models: `llama3.2` (demo), `qwen2.5:14b` / `qwen2.5-coder:14b` (dev)
- **GPU:** NVIDIA RTX 5090 24 GB, Docker Desktop with NVIDIA Container Toolkit
- **No test projects exist.** No unit/integration tests.

---

## Architecture (8 projects)

| Project | Purpose | Key Types |
|---------|---------|-----------|
| **Core** | Abstractions + data models (zero NuGet dependencies) | `IAgent`, `IModel`, `ITool`, `IModelRouter`, request/response models |
| **Runtime** | Agent orchestration — register by name, route `SendAsync`, error wrapping | `AgentRuntime` |
| **Models** | LLM adapters (HTTP-based) | `OllamaModel` ✅, `AzureOpenAIModel` 🚧 (`NotImplementedException`) |
| **Agents** | Agent implementations + static factory (references Core + Models) | `ConfigurableAgent`, `SpecialistAgents` (8 types), `SimpleQuestionAgent` (legacy) |
| **Web** | Blazor Server app (Fluent UI v4) — 8 services, 9 pages | Dashboard, playground, compare, examples, RAG demo |
| **Demos** | Console entry point — manual DI, interactive chat with `SimpleQuestionAgent` | `Program.cs` (top-level statements) |
| **AppHost** | .NET Aspire orchestration (only orchestrates Web) | `Program.cs` |
| **ServiceDefaults** | Shared Aspire defaults (OpenTelemetry, health checks, resilience) | `Extensions.cs` |

### Dependency flow

```
Core ← Runtime, Models
Core + Models ← Agents
Core + Runtime + Models + Agents ← Web, Demos
Web ← AppHost
```

### Key data flow (Web app)

```
AgentConfig → AgentFactoryService.CreateAgent() → SpecialistAgents.Create() → ConfigurableAgent
User → AgentRequest → ConfigurableAgent → ModelRequest → OllamaModel → POST /api/generate → Ollama
Ollama → JSON → ModelResponse → AgentResponse → User
```

Parameter overrides (temperature, topP, topK, repeatPenalty, numCtx, seed) flow via `AgentRequest.Metadata` dictionary.

---

## Coding Conventions

### Must-follow rules

- **`TreatWarningsAsErrors`** is enabled globally via `Directory.Build.props` — zero warnings allowed
- **`CancellationToken cancellationToken = default`** on every async method (full name, not `ct`)
- **XML doc comments** (`<summary>`) on all public members
- **File-scoped namespaces**, one type per file, filename matches type name
- **Constructor injection** for dependencies — traditional constructors with `private readonly` fields (no primary constructors in existing code)
- **Collection expressions** (`[]`) for list initialization where applicable

### Two data model styles (intentional)

| Layer | Style | Namespace | Examples |
|-------|-------|-----------|----------|
| `Core/Models/` and `Core/Abstractions/` | `required` + `init` (immutable classes) | `AgenticLab.Core.Abstractions` | `AgentRequest`, `ModelRequest`, `ModelResponse`, `ChatMessage` |
| `Web/Services/` | `{ get; set; }` (mutable POCOs) | `AgenticLab.Web.Services` | `ModelConfig`, `AgentConfig`, `ChatSession`, `ChatEntry` |

**Note:** Core data model files live in `Core/Models/` folder but share the `AgenticLab.Core.Abstractions` namespace with the interfaces.

### Adding a new specialist agent

Use the `SpecialistAgents` static factory + `ConfigurableAgent`. Two changes needed in `SpecialistAgents.cs`:

```csharp
// 1. Add to private static SystemPrompts dictionary:
["MySpecialist"] = """
    You are an expert at X. Instructions: ...
    """,

// 2. Add to local descriptions dictionary in Create():
["MySpecialist"] = "Does X with precision.",
```

Then add agent configs in `AgentFactoryService` constructor (follow the existing `{type}-precise` / `{type}-fast` naming pattern).

Existing 8 types: `SimpleQuestion`, `Summarizer`, `DataExtractor`, `CodeGenerator`, `Translator`, `Classifier`, `FormatConverter`, `CreativeWriter`.

### Tool pattern (ITool — no implementations yet)

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

### Service lifetimes (critical — DI errors if wrong)

| Service | Lifetime | Why |
|---------|----------|-----|
| `ModelRegistryService` | **Singleton** | Shared model config registry, queries Ollama |
| `AgentFactoryService` | **Singleton** | Shared agent config registry + factory |
| `ExampleService` | **Singleton** | Static demo data (26 examples, 11 categories) |
| `ExportService` | **Singleton** | Stateless format conversion (JSON/CSV/Markdown/PlainText) |
| `RagDemoService` | **Singleton** | Static RAG demo data (7 docs, 56 chunks, 10 scenarios) |
| `AgentRuntime` | **Singleton** | Shared agent registry |
| `ChatService` | **Scoped** | Per-circuit chat sessions + history |
| `CompareService` | **Scoped** | Per-circuit multi-agent comparison (runs agents via `Task.WhenAll`) |

### HttpClient pattern

Named client `"Ollama"` via `IHttpClientFactory`, configured from `appsettings.json`:

```json
{ "Ollama": { "Endpoint": "http://localhost:11434", "TimeoutMinutes": 5 } }
```

### State management

All state is **in-memory** — no database. Defaults seeded in service constructors: 6 model configs + 16 agent configs. `ChatSession` / `ChatEntry` stored in `List<T>`.

### Pages (9 total)

```
Home(/), Models(/models), Agents(/agents), Playground(/playground),
Compare(/compare), Examples(/examples), Export(/export), Learn(/learn), RagDemo(/ragdemo)
```

Component tree: `App.razor → Routes.razor → Layout/MainLayout.razor` (FluentLayout + nav sidebar)

---

## Ollama Integration

- **Generation:** `POST /api/generate` with `stream: false` — `OllamaModel.GenerateAsync()`
- **Model listing:** `GET /api/tags` — `ModelRegistryService.GetAvailableModelsAsync()`
- **Health check:** `GET /` (root endpoint) — `ModelRegistryService.IsOllamaOnlineAsync()`
- **Embeddings (planned):** `POST /api/embed` — see `docs/architecture/rag-pipeline.md`

### ModelRequest → Ollama options mapping

`MaxTokens` → `num_predict`, `TopP` → `top_p`, `TopK` → `top_k`, `RepeatPenalty` → `repeat_penalty`, `NumCtx` → `num_ctx`. Also: `Temperature`, `Seed`, `Stop`.

`OllamaModel.Name` returns `"ollama:{modelName}"` (e.g., `"ollama:qwen2.5:14b"`).

---

## Infrastructure

### Docker (`infra/docker/docker-compose.yml`)

- **Default:** Ollama with GPU passthrough (port 11434)
- **Profile `vllm`:** vLLM OpenAI-compatible API (port 8000)
- **Profile `webui`:** Open WebUI chat interface (port 3000)
- **Init service:** Pulls `qwen2.5:14b`, `llama3.2`, `nomic-embed-text`

### RAG pipeline (designed, not implemented)

Design in `docs/architecture/rag-pipeline.md`. Demo data in `data/rag-demo/` (7 files). `RagDemoService` provides static demo scenarios (no real vector search). Planned: Qdrant + `OllamaEmbeddingModel` + `IVectorStore` + `RagAgent`.

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
| Blazor Web (Fluent UI): 9 pages incl. RAG demo, 8 services | ✅ |
| Docker Compose (Ollama + vLLM + WebUI), .NET Aspire AppHost | ✅ |
| Conversation history (`ChatService` passes `History` in `AgentRequest`) | ✅ |
| AzureOpenAIModel adapter | 🚧 Placeholder (`NotImplementedException`) |
| RAG pipeline (embeddings + Qdrant + ingestion + RagAgent) | 📋 Designed, demo-only `RagDemoService` exists |
| IModelRouter, tool-using agents, multi-agent collaboration | 📋 Planned |
