# AgenticLab

**AgenticLab** is a learning and experimentation repository focused on building **agentic systems** using **.NET**, **local large language models (LLMs)**, and **hybrid cloud architectures** based on Microsoft technologies.

The goal of this project is to understand, design, and implement agent-based AI systems that can run:

- Fully **locally** on a developer machine
- In a **hybrid mode** (local + cloud)
- Or fully in the **cloud** (Azure)

---

## Mission

AgenticLab exists to:

- Learn agentic system design from an **engineering-first** perspective
- Explore **local LLM execution** (Docker, ONNX, Ollama, etc.)
- Build **framework-agnostic agent runtimes** in .NET
- Compare **local vs cloud** execution models
- Document patterns, pitfalls, and best practices
- Use **Visual Studio Code** and **Visual Studio 2026** effectively
- Leverage **GitHub Copilot** as a development accelerator

This is a **lab**, not a product. Experiments are expected.

---

## Core Principles

- **Local-first**: cloud is optional, not required
- **Composable agents**: small, focused, replaceable
- **Deterministic orchestration** where possible
- **Clear boundaries** between agents, tools, and models
- **Dependency injection** over static coupling
- **Configuration over hardcoding**
- **Documentation lives next to code**

---

## Repository Structure

```text
agenticlab/
│
├─ README.md
│
├─ docs/
│  ├─ deck.md                    # Markdown presentation (slides)
│  ├─ architecture/
│  │  ├─ local-agentic.md        # Local-only agentic architecture
│  │  ├─ hybrid-agentic.md       # Hybrid (local + cloud) architecture
│  │  └─ diagrams/               # Architecture diagrams
│  ├─ tutorials/
│  │  ├─ 01-getting-started.md   # First steps with the lab
│  │  ├─ 02-local-llm-setup.md   # Setting up local LLMs
│  │  └─ 03-first-agent.md       # Building your first agent
│  ├─ examples/
│  │  └─ example-index.md        # Index of code examples
│  └─ notes/
│     └─ glossary.md             # Key terms and definitions
│
├─ src/
│  ├─ AgenticLab.sln
│  ├─ AgenticLab.Core/           # Core abstractions (agents, tools, messages)
│  ├─ AgenticLab.Runtime/        # Orchestration and execution engine
│  ├─ AgenticLab.Models/         # Local and cloud LLM adapters
│  ├─ AgenticLab.Agents/         # Concrete agent implementations
│  └─ AgenticLab.Demos/          # Console / WebAPI demos
│
├─ infra/
│  ├─ docker/                    # Local LLM containers
│  ├─ local/                     # Local-only configs
│  └─ azure/                     # Azure deployment examples
│
└─ .github/
   ├─ copilot-instructions.md
   └─ workflows/
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) with C# Dev Kit
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for local LLMs)
- [Ollama](https://ollama.com/) (optional, for local model execution)

### Clone and Build

```bash
git clone https://github.com/your-org/agenticlab.git
cd agenticlab
dotnet build src/AgenticLab.sln
```

---

## Documentation

| Document | Description |
|----------|-------------|
| [Presentation Deck](docs/deck.md) | Slide deck covering agentic system concepts |
| [Local Architecture](docs/architecture/local-agentic.md) | Local-only agentic patterns |
| [Hybrid Architecture](docs/architecture/hybrid-agentic.md) | Hybrid local + cloud patterns |
| [Getting Started Tutorial](docs/tutorials/01-getting-started.md) | Step-by-step first run |
| [Glossary](docs/notes/glossary.md) | Key terms and definitions |

---

## License

This project is for learning and experimentation purposes.
