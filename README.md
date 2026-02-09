# AgenticLab

**AgenticLab** is a **.NET 10 / C# 14** learning and experimentation project for building **agentic AI systems** — autonomous software agents that plan, reason, use tools, and produce results with minimal human intervention.

It focuses on **local-first LLM execution** via [Ollama](https://ollama.com/) (v0.15.6), with optional hybrid cloud (Azure OpenAI) capabilities. This is a **lab**, not a product — experiments are expected.

> **Target machine:** Lenovo 83EY · Intel Core Ultra 9 275HX (24 cores) · 192 GB DDR5 · NVIDIA RTX 5090 Laptop GPU 24 GB · Windows 11 Pro

---

## Mission

AgenticLab exists to:

- Learn agentic system design from an **engineering-first** perspective
- Explore **local LLM execution** (Ollama, Docker, vLLM, ONNX Runtime)
- Build **framework-agnostic agent runtimes** in .NET
- Design **specialist agents** for real business tasks (document processing, CRM integration, RAG Q&A)
- Compare **local vs hybrid vs cloud** execution models
- Document patterns, pitfalls, and best practices
- Leverage **GitHub Copilot** (Agent mode, Claude Opus 4.6) as a development accelerator

---

## Core Principles

- **Local-first** — cloud is optional, not required; design to work fully offline
- **Composable** — agents, tools, and models are small, focused, and replaceable
- **Interface-based** — agents don't know their backing model; models don't know their callers
- **Explicit boundaries** — agents speak `AgentRequest`/`AgentResponse`, models speak `ModelRequest`/`ModelResponse`
- **Dependency injection** over static coupling
- **Configuration-driven** — use `appsettings.json`; avoid hardcoded values
- **Testable** — all interfaces are mockable; DI-first design
- **Documentation next to code** — every folder with code should have context explaining its purpose

---

## Architecture

Eight .NET projects with a shared `Directory.Build.props` enforcing `net10.0`, nullable, implicit usings, and `TreatWarningsAsErrors`:

| Project | Purpose | Key Types |
|---------|---------|----------|
| **AgenticLab.Core** | Core abstractions and data models (no dependencies) | `IAgent`, `IModel`, `ITool`, `IModelRouter`, `AgentRequest/Response`, `ModelRequest/Response`, `ToolInput/Result`, `ChatMessage` |
| **AgenticLab.Runtime** | Agent orchestration — registers agents by name, routes requests, handles errors | `AgentRuntime` |
| **AgenticLab.Models** | LLM adapters — HTTP-based model backends | `OllamaModel` ✅, `AzureOpenAIModel` 🚧 |
| **AgenticLab.Agents** | Configurable agent implementations with domain-specific system prompts | `ConfigurableAgent`, `SpecialistAgents` (factory), `SimpleQuestionAgent` |
| **AgenticLab.Web** | Blazor Server web application — dashboard, model/agent configuration, playground, compare, examples | `ChatService`, `CompareService`, `ModelRegistryService`, `AgentFactoryService`, `ExampleService`, `ExportService` |
| **AgenticLab.Demos** | Console entry point — DI setup, interactive chat loop | `Program.cs` |
| **AgenticLab.AppHost** | .NET Aspire orchestration host | `Program.cs` |
| **AgenticLab.ServiceDefaults** | Shared Aspire service defaults (telemetry, health checks) | `Extensions.cs` |

```text
Dependency flow:

  Core ← Runtime          (references Core)
  Core ← Models           (references Core)
  Core ← Agents           (references Core)
  Core, Runtime, Models, Agents ← Web   (references all)
  Core, Runtime, Models, Agents ← Demos (references all)
  Web  ← AppHost          (.NET Aspire orchestration)
```

### How It Works (Console Demo)

```text
  You type a question ──▶ Program.cs ──▶ AgentRuntime ──▶ SimpleQuestionAgent
                                                                │
                                                          ModelRequest
                                                                │
                                                                ▼
                                                          OllamaModel
                                                                │
                                                     POST /api/generate
                                                                │
                                                                ▼
                                                     Ollama @ localhost:11434
                                                                │
                                                          ModelResponse
                                                                │
  Console prints answer ◀── AgentResponse ◀── Agent ◀──────────┘
```

### How It Works (Web Application)

```text
  Browser ──▶ Blazor Server (Fluent UI) ──▶ ChatService / CompareService
                                                    │
                                              AgentFactoryService
                                              (creates ConfigurableAgent
                                               + OllamaModel per request)
                                                    │
                                              ModelRequest (with overrides:
                                               temperature, top_p, top_k,
                                               repeat_penalty, num_ctx, seed)
                                                    │
                                                    ▼
                                              Ollama @ localhost:11434
                                                    │
                                              AgentResponse + metadata
                                              (model, tokens, duration)
                                                    │
  Browser shows response ◀── ChatSession ◀─────────┘
```

---

## Web Application

The **AgenticLab.Web** project provides a full-featured Blazor Server UI for configuring, testing, and comparing agents — all running locally against Ollama.

**Stack:** Blazor Server (.NET 10) · [Microsoft Fluent UI](https://www.fluentui-blazor.net/) v4 · Interactive Server render mode

### Pages

| Page | Path | Description |
|------|------|-------------|
| **Dashboard** | `/` | System health, stats (Ollama status, model/agent counts), quick start links, available Ollama models table |
| **Models** | `/models` | Create, edit, and delete model configurations (model name, temperature, max tokens, system prompt) |
| **Agents** | `/agents` | Create, edit, and delete agent configurations with type selection, model binding, parameter overrides, and advanced Ollama parameters |
| **Playground** | `/playground` | Interactive chat with selected agent — sidebar shows active configuration, all parameters, and system prompt |
| **Compare** | `/compare` | Run the same prompt against multiple agents simultaneously — side-by-side result cards with timing and token metrics |
| **Examples** | `/examples` | 26 built-in demo prompts across 10 categories with suggested agents — one-click launch to Playground or Compare |
| **Export** | `/export` | Export agent configurations and results |
| **Learn** | `/learn` | Learning resources and documentation links |

### Specialist Agent Types

Eight built-in agent types with domain-specific system prompts optimized for local models:

| Agent Type | Purpose | Recommended Temp |
|------------|---------|:----------------:|
| **Simple Q&A** | Clear, concise answers to questions | 0.3–0.7 |
| **Summarizer** | Structured summaries (main point + key details + conclusion) | 0.2–0.4 |
| **Data Extractor** | Extracts structured JSON from unstructured text | 0.1–0.3 |
| **Code Generator** | Production-quality code (defaults to C#) | 0.2–0.5 |
| **Translator** | Multi-language translation preserving tone and formatting | 0.2–0.5 |
| **Classifier** | Text classification with confidence levels and reasoning | 0.1–0.3 |
| **Format Converter** | Data conversion between JSON/YAML/XML/CSV/TOML/SQL | 0.1–0.3 |
| **Creative Writer** | Stories, poems, dialogue, essays | 0.7–1.0 |

### Default Configurations

Ships with **6 model configurations** and **16 agent configurations** out of the box:

**Models:** Llama 3.2 (Default, Precise, Fast, Creative) + Qwen 2.5 14B (Default) + Qwen 2.5 Coder 14B (Precise)

**Agents:** Two variants per type — *Precise* (Qwen 2.5 Coder 14B, low temperature) and *Fast* (Llama 3.2, higher temperature, fewer tokens)

### Ollama Parameters

Full support for [Ollama model parameters](https://docs.ollama.com/modelfile) — configurable per agent:

| Parameter | Default | Effect |
|-----------|---------|--------|
| Temperature | 0.7 | Randomness (0.0 = deterministic, 1.0 = creative) |
| Max Tokens | 1000 | Maximum response length |
| Top-P | 0.9 | Nucleus sampling diversity |
| Top-K | 40 | Token candidate limit |
| Repeat Penalty | 1.1 | Penalizes repeated tokens |
| Context Window | 2048 | How much context the model considers |
| Seed | 0 (random) | Fixed seed for reproducible outputs |

> See the [Web Guide](docs/tutorials/04-web-guide.md) for detailed usage instructions with screenshots.

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|--------|
| **Language** | C# 14 / .NET 10 | Agent runtime, abstractions, DI |
| **Web UI** | Blazor Server + [Fluent UI](https://www.fluentui-blazor.net/) v4 | Web-based agent workbench |
| **Orchestration** | .NET Aspire | AppHost, service defaults, telemetry |
| **Local LLM** | [Ollama](https://github.com/ollama/ollama) v0.15.6 (162k+ ⭐, MIT) | Run models locally via REST API |
| **Recommended model** | Qwen 2.5 Coder 14B (~9 GB) | Best local model for code and analysis |
| **Fast model** | Llama 3.2 3B (2.0 GB) | Quick responses, routing |
| **Embeddings** | nomic-embed-text | RAG pipeline (planned) |
| **Alt. LLM serving** | vLLM (Docker) | OpenAI-compatible API, higher throughput |
| **Cloud LLM** | Azure OpenAI (planned) | Hybrid routing for complex tasks |
| **Containers** | Docker Desktop + NVIDIA Container Toolkit | GPU-accelerated local infrastructure |
| **IDE** | VS Code + C# Dev Kit + GitHub Copilot | Development environment |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) with C# Dev Kit
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for containerized LLMs)
- [Ollama](https://ollama.com/) v0.15.6 (for local LLM execution)

### Quick Start (Web Application)

```powershell
# 1. Clone and build
git clone https://github.com/your-org/agentinc-lab.git
cd agentinc-lab
dotnet build src/AgenticLab.sln

# 2. Install Ollama and pull models
winget install Ollama.Ollama
ollama pull llama3.2
ollama pull qwen2.5-coder:14b

# 3. Run the web application
dotnet run --project src/AgenticLab.Web
# Open http://localhost:5210 in your browser
```

### Quick Start (Console Demo)

```powershell
# Run the interactive console demo
dotnet run --project src/AgenticLab.Demos
```

### Using Docker (alternative)

```powershell
cd infra/docker
docker compose up -d                      # Start Ollama with GPU
docker compose up ollama-init             # Pull default models (qwen2.5:14b, llama3.2, nomic-embed-text)
```

---

## Implementation Status

| Feature | Status |
|---------|--------|
| Core abstractions (`IAgent`, `IModel`, `ITool`, `IModelRouter`) | ✅ Implemented |
| `AgentRuntime` (register, send, error handling) | ✅ Implemented |
| `OllamaModel` adapter (HTTP, `/api/generate`, full Ollama params) | ✅ Implemented |
| `ConfigurableAgent` with metadata overrides | ✅ Implemented |
| `SpecialistAgents` factory (8 agent types with domain prompts) | ✅ Implemented |
| Blazor Server web application (Fluent UI) | ✅ Implemented |
| Dashboard with Ollama status and system stats | ✅ Implemented |
| Model configuration management (CRUD) | ✅ Implemented |
| Agent configuration with advanced Ollama parameters | ✅ Implemented |
| Interactive Playground with chat sessions | ✅ Implemented |
| Compare — multi-agent parallel prompt execution | ✅ Implemented |
| 26 built-in examples across 10 categories | ✅ Implemented |
| Export and Learn pages | ✅ Implemented |
| .NET Aspire AppHost orchestration | ✅ Implemented |
| Interactive console demo | ✅ Implemented |
| Docker Compose (Ollama + vLLM + Open WebUI) | ✅ Implemented |
| `AzureOpenAIModel` adapter | 🚧 Placeholder |
| `IModelRouter` implementation (hybrid routing) | 📋 Planned |
| Tool-using agents (`ITool` integration) | 📋 Planned |
| Multi-agent collaboration | 📋 Planned |
| Conversation history/memory persistence | 📋 Planned |
| RAG pipeline (embeddings + vector store) | 📋 Planned |
| BDI mapping agent (CRM → SAP/Navision) | 📋 Planned |

---

## Repository Structure

```text
agentinc-lab/
│
├─ README.md
├─ .github/
│  └─ copilot-instructions.md          # GitHub Copilot agent instructions
│
├─ docs/
│  ├─ deck.md                          # Markdown slide deck
│  ├─ demo-walkthrough.md              # Demo app explained — data flow, patterns, roadmap
│  ├─ local-llm-rag-specialist-agents.md  # Presentation: specialist agents, RAG, BDI case study
│  ├─ architecture/
│  │  ├─ local-agentic.md              # Local-only architecture patterns
│  │  ├─ hybrid-agentic.md             # Hybrid local + cloud architecture
│  │  └─ diagrams/                     # Mermaid architecture diagrams (.mmd)
│  ├─ tutorials/
│  │  ├─ 01-getting-started.md         # Setup prerequisites and first run
│  │  ├─ 02-local-llm-setup.md         # Ollama, Docker, vLLM, ONNX setup & model guide
│  │  └─ 03-first-agent.md             # Build and wire up a simple agent
│  ├─ examples/
│  │  └─ example-index.md              # Code examples index
│  └─ notes/
│     └─ glossary.md                   # Key terms and definitions
│
├─ src/
│  ├─ AgenticLab.sln
│  ├─ Directory.Build.props            # Shared: net10.0, nullable, TreatWarningsAsErrors
│  ├─ AgenticLab.Core/                 # Abstractions + data models (IAgent, IModel, ITool, etc.)
│  ├─ AgenticLab.Runtime/              # Agent orchestration engine (AgentRuntime)
│  ├─ AgenticLab.Models/               # LLM adapters (OllamaModel, AzureOpenAIModel)
│  ├─ AgenticLab.Agents/               # Concrete agents (SimpleQuestionAgent)
│  └─ AgenticLab.Demos/                # Console demo (Program.cs, interactive chat loop)
│
└─ infra/
   ├─ docker/                          # Docker Compose: Ollama, vLLM, Open WebUI
   ├─ local/                           # Local config (appsettings.local.json)
   └─ azure/                           # Azure deployment (planned)
```

---

## Documentation

### Tutorials

| Document | Description |
|----------|-------------|
| [01 — Getting Started](docs/tutorials/01-getting-started.md) | Prerequisites, build, and first run |
| [02 — Local LLM Setup](docs/tutorials/02-local-llm-setup.md) | Ollama, Docker, vLLM, ONNX — model selection guide for RTX 5090 |
| [03 — First Agent](docs/tutorials/03-first-agent.md) | Build a simple agent with IAgent, IModel, and DI |
| [04 — Web Guide](docs/tutorials/04-web-guide.md) | Complete web application guide — models, agents, playground, compare (with screenshots) |

### Architecture

| Document | Description |
|----------|-------------|
| [Local Architecture](docs/architecture/local-agentic.md) | Patterns for running agents fully on a developer machine |
| [Hybrid Architecture](docs/architecture/hybrid-agentic.md) | Local + cloud routing with IModelRouter |
| [Architecture Diagrams](docs/architecture/diagrams/) | Mermaid diagrams: agent lifecycle, local, hybrid |

### Presentations & Guides

| Document | Description |
|----------|-------------|
| [Local LLMs & RAG — Specialist Agents](docs/local-llm-rag-specialist-agents.md) | 17-slide presentation: AI levels, RAG, specialist agents, BDI case study, cost analysis |
| [Demo Walkthrough](docs/demo-walkthrough.md) | How the demo app works — data flow, design patterns, roadmap |
| [Presentation Deck](docs/deck.md) | Slide deck on agentic system concepts |

### Reference

| Document | Description |
|----------|-------------|
| [Glossary](docs/notes/glossary.md) | Key terms: Agent, RAG, Ollama, GGUF, Modelfile, Vector Database, etc. |
| [Quantization Reference](docs/notes/k-quant-mixed-precision.md) | K-quant and mixed precision details |
| [Examples Index](docs/examples/example-index.md) | Code examples index with planned samples |
| [Docker Setup](infra/docker/README.md) | Docker Compose profiles, GPU setup, model recommendations |

---

## Recommended Models for RTX 5090 (24 GB VRAM)

Ollama memory requirements: 8 GB RAM for 7B models, 16 GB for 13B, 32 GB for 33B.

| Model | Size | Best For |
|-------|-----:|----------|
| Gemma 3 4B | 3.3 GB | Fast utility (Ollama default) |
| Phi 4 Mini 3.8B | 2.5 GB | SLM tasks, classification |
| Llama 3.2 3B | 2.0 GB | Quick routing, fast responses |
| **Qwen 2.5 Coder 14B** | **~9 GB** | **Best local model for code and analysis** |
| Qwen 2.5 14B | ~9 GB | Best local all-rounder |
| Phi 4 14B | 9.1 GB | Strong 14B alternative |
| Gemma 3 27B | 17 GB | Near-cloud quality |
| QwQ 32B | 20 GB | Reasoning specialist |
| DeepSeek-R1 32B | ~20 GB | Reasoning chains |

See [02 — Local LLM Setup](docs/tutorials/02-local-llm-setup.md) for the full model selection guide.

---

## Build & Run

```powershell
# Build the solution
dotnet build src/AgenticLab.sln

# Run the web application (requires Ollama running)
dotnet run --project src/AgenticLab.Web
# → http://localhost:5210

# Run the interactive console demo (requires Ollama with llama3.2)
dotnet run --project src/AgenticLab.Demos

# VS Code tasks (Ctrl+Shift+B):
#   build    — default build task
#   restore  — restore NuGet packages
#   clean    — clean build output
#   run demo — start the interactive console
#   run web  — start the web application
```

---

## License

This project is for learning and experimentation purposes.
