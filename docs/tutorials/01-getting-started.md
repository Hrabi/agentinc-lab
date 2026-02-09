# Tutorial 01: Getting Started with AgenticLab

> Set up your development environment and run your first agent.

---

## Prerequisites

Before you begin, ensure you have the following installed:

- [ ] **.NET 10 SDK** — [Download](https://dotnet.microsoft.com/download)
- [ ] **Visual Studio Code** — [Download](https://code.visualstudio.com/)
- [ ] **C# Dev Kit** extension for VS Code
- [ ] **Docker Desktop** — [Download](https://www.docker.com/products/docker-desktop/)
- [ ] **Git** — [Download](https://git-scm.com/)

### Optional

- [ ] **Ollama** — [Download](https://ollama.com/) (for local LLM execution)
- [ ] **Visual Studio 2026** (for full IDE experience)
- [ ] **GitHub Copilot** subscription (for AI-assisted development)

---

## Step 1: Clone the Repository

```bash
git clone https://github.com/your-org/agenticlab.git
cd agenticlab
```

---

## Step 2: Build the Solution

```bash
dotnet restore src/AgenticLab.sln
dotnet build src/AgenticLab.sln
```

Verify all projects build successfully.

---

## Step 3: Set Up a Local LLM (Optional)

### Using Ollama

```bash
# Install Ollama (if not already installed)
# Then pull a model:
ollama pull llama3.2

# Verify it's running:
ollama list
```

The Ollama API runs at `http://localhost:11434` by default.

### Using Docker

```bash
cd infra/docker
docker-compose up -d
```

---

## Step 4: Run the Demo

```bash
dotnet run --project src/AgenticLab.Demos
```

You should see output showing the agent initializing, processing a task, and returning results.

---

## Step 5: Explore the Code

Key files to examine:

| File | Purpose |
|------|---------|
| `src/AgenticLab.Core/IAgent.cs` | Agent interface |
| `src/AgenticLab.Core/ITool.cs` | Tool interface |
| `src/AgenticLab.Core/IModel.cs` | Model interface |
| `src/AgenticLab.Runtime/AgentRuntime.cs` | Orchestration engine |
| `src/AgenticLab.Demos/Program.cs` | Demo entry point |

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Build fails | Ensure .NET 10 SDK is installed: `dotnet --version` |
| Ollama not responding | Check if Ollama is running: `ollama list` |
| Docker issues | Ensure Docker Desktop is running |

---

## Next Steps

- [Tutorial 02: Local LLM Setup](02-local-llm-setup.md)
- [Tutorial 03: Building Your First Agent](03-first-agent.md)
- [Architecture Overview](../architecture/local-agentic.md)
