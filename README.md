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

Five .NET projects with a shared `Directory.Build.props` enforcing `net10.0`, nullable, implicit usings, and `TreatWarningsAsErrors`:

| Project | Purpose | Key Types |
|---------|---------|-----------|
| **AgenticLab.Core** | Core abstractions and data models (no dependencies) | `IAgent`, `IModel`, `ITool`, `IModelRouter`, `AgentRequest/Response`, `ModelRequest/Response`, `ToolInput/Result`, `ChatMessage` |
| **AgenticLab.Runtime** | Agent orchestration — registers agents by name, routes requests, handles errors | `AgentRuntime` |
| **AgenticLab.Models** | LLM adapters — HTTP-based model backends | `OllamaModel` ✅, `AzureOpenAIModel` 🚧 |
| **AgenticLab.Agents** | Concrete agent implementations | `SimpleQuestionAgent` |
| **AgenticLab.Demos** | Console entry point — DI setup, interactive chat loop | `Program.cs` |

```text
Dependency flow:

  Core ← Runtime   (references Core)
  Core ← Models    (references Core)
  Core ← Agents    (references Core, Models)
  All  ← Demos     (references all projects)
```

### How It Works

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

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Language** | C# 14 / .NET 10 | Agent runtime, abstractions, DI |
| **Local LLM** | [Ollama](https://github.com/ollama/ollama) v0.15.6 (162k+ ⭐, MIT) | Run models locally via REST API |
| **Recommended model** | Qwen 2.5 14B (~9 GB) | Best local all-rounder for 24 GB VRAM |
| **Fast model** | Gemma 3 4B (3.3 GB) | Quick routing, classification |
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

### Quick Start

```powershell
# 1. Clone and build
git clone https://github.com/your-org/agentinc-lab.git
cd agentinc-lab
dotnet build src/AgenticLab.sln

# 2. Install Ollama and pull a model
winget install Ollama.Ollama
ollama pull llama3.2

# 3. Run the interactive demo
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
| `OllamaModel` adapter (HTTP, `/api/generate`) | ✅ Implemented |
| `SimpleQuestionAgent` | ✅ Implemented |
| Interactive console demo | ✅ Implemented |
| Docker Compose (Ollama + vLLM + Open WebUI) | ✅ Implemented |
| `AzureOpenAIModel` adapter | 🚧 Placeholder |
| `IModelRouter` implementation (hybrid routing) | 📋 Planned |
| Tool-using agents (`ITool` integration) | 📋 Planned |
| Multi-agent collaboration | 📋 Planned |
| Conversation history/memory | 📋 Planned |
| Configuration from `appsettings.json` | 📋 Planned |
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
| [Examples Index](docs/examples/example-index.md) | Code examples index with planned samples |
| [Docker Setup](infra/docker/README.md) | Docker Compose profiles, GPU setup, model recommendations |

---

## Recommended Models for RTX 5090 (24 GB VRAM)

Ollama memory requirements: 8 GB RAM for 7B models, 16 GB for 13B, 32 GB for 33B.

| Model | Size | Best For |
|-------|-----:|----------|
| Gemma 3 4B | 3.3 GB | Fast utility (Ollama default) |
| Phi 4 Mini 3.8B | 2.5 GB | SLM tasks, classification |
| Llama 3.2 3B | 2.0 GB | Quick routing |
| **Qwen 2.5 14B** | **~9 GB** | **Best local all-rounder** |
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

# Run the interactive demo (requires Ollama with llama3.2)
dotnet run --project src/AgenticLab.Demos

# VS Code tasks (Ctrl+Shift+B):
#   build    — default build task
#   restore  — restore NuGet packages
#   clean    — clean build output
#   run demo — start the interactive console
```

---

## License

This project is for learning and experimentation purposes.
