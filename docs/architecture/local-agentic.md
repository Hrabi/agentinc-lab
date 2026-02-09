# Local Agentic Architecture

> Architecture patterns for running agentic systems entirely on a developer machine.

---

## Overview

The local agentic architecture runs all components on a single machine:

- **LLM inference** via Ollama, ONNX Runtime, or Docker containers
- **Agent runtime** as a .NET console or background service
- **Tools** as local function calls or HTTP endpoints
- **Storage** using local file system or SQLite

---

## Architecture Diagram

```text
┌─────────────────────────────────────────────┐
│               Developer Machine             │
│                                             │
│  ┌──────────────────────────────────────┐   │
│  │         AgenticLab.Runtime           │   │
│  │  ┌──────────┐    ┌──────────────┐    │   │
│  │  │  Agent A │    │   Agent B    │    │   │
│  │  │  (Plan)  │──▶│  (Execute)   │    │   │
│  │  └──────────┘    └──────────────┘    │   │
│  └──────────┬───────────────┬───────────┘   │
│             │               │               │
│  ┌──────────▼──┐    ┌──────▼───────────┐   │
│  │  Local LLM  │    │   Local Tools    │   │
│  │  (Ollama)   │    │  (File, Search)  │   │
│  └─────────────┘    └─────────────────┘    │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │         Local Storage               │    │
│  │  (SQLite / File System)             │    │
│  └─────────────────────────────────────┘    │
└─────────────────────────────────────────────┘
```

---

## Key Components

### Local LLM Providers

| Provider | Model Format | Notes |
|----------|-------------|-------|
| **Ollama** | GGUF | Easy setup, REST API, many models |
| **ONNX Runtime** | ONNX | Optimized for .NET, GPU support |
| **Docker + vLLM** | Various | Production-grade serving |
| **LM Studio** | GGUF | GUI-based, easy experimentation |

### Agent Runtime

The runtime is a .NET application that:

1. Loads agent configurations
2. Connects agents to their assigned model and tools
3. Manages message routing between agents
4. Handles execution lifecycle (start, pause, stop)

### Local Tools

Tools are registered capabilities that agents can invoke:

- **FileSystem** — read/write files
- **Search** — search local documents
- **CodeExecution** — run code snippets
- **Database** — query local SQLite

---

## Configuration Example

```json
{
  "runtime": {
    "mode": "local",
    "maxConcurrentAgents": 4
  },
  "models": {
    "default": {
      "provider": "ollama",
      "endpoint": "http://localhost:11434",
      "model": "llama3.2"
    }
  },
  "agents": [
    {
      "name": "planner",
      "model": "default",
      "tools": ["filesystem", "search"]
    }
  ]
}
```

---

## Benefits

- **Zero cloud cost** — everything runs locally
- **Data privacy** — no data leaves the machine
- **Fast iteration** — no network latency
- **Offline capable** — works without internet

## Limitations

- **Model quality** — local models may be smaller/less capable
- **Hardware requirements** — GPU recommended for larger models
- **Scaling** — limited to single machine resources

---

## Next Steps

- See [Hybrid Architecture](hybrid-agentic.md) for adding cloud capabilities
- See [Getting Started](../tutorials/01-getting-started.md) for setup instructions
