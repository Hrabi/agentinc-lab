# Hybrid Agentic Architecture

> Architecture patterns for running agentic systems across local and cloud environments.

---

## Overview

The hybrid architecture combines local execution with cloud services:

- **Routine tasks** run on local LLMs (cost-effective, private)
- **Complex reasoning** is routed to cloud models (GPT-4, Claude)
- **Cloud services** provide search, storage, and scaling
- **Orchestration** remains local for control and observability

---

## Architecture Diagram

```text
┌───────────────────────────┐      ┌───────────────────────────┐
│      Local Machine        │      │         Azure Cloud       │
│                           │      │                           │
│  ┌─────────────────────┐  │      │  ┌─────────────────────┐  │
│  │  AgenticLab.Runtime │  │      │  │  Azure OpenAI       │  │
│  │  ┌───────┐          │  │ HTTP │  │  (GPT-4o, GPT-4)    │  │
│  │  │ Agent │──────────┼──┼──────┼──▶                     │  │
│  │  └───────┘          │  │      │  └─────────────────────┘  │
│  │       │             │  │      │                           │
│  │  ┌────▼──────┐      │  │      │  ┌─────────────────────┐  │
│  │  │ Router    │      │  │      │  │  Azure AI Search    │  │
│  │  │ (local/   │      │  │──────┼──▶  (RAG, indexing)    │  │
│  │  │  cloud)   │      │  │      │  └─────────────────────┘  │
│  │  └───────────┘      │  │      │                           │
│  └─────────────────────┘  │      │  ┌─────────────────────┐  │
│                           │      │  │  Azure Storage      │  │
│  ┌─────────────────────┐  │      │  │  (Blobs, Tables)    │  │
│  │  Local LLM (Ollama) │  │      │  └─────────────────────┘  │
│  └─────────────────────┘  │      │                           │
└───────────────────────────┘      └───────────────────────────┘
```

---

## Routing Strategy

The **model router** decides where to send each request:

| Criteria | Route | Rationale |
|----------|-------|-----------|
| Simple classification | Local | Fast, free, private |
| Code generation | Cloud | Higher quality output |
| Summarization (< 4K tokens) | Local | Adequate quality |
| Complex reasoning | Cloud | Requires larger models |
| Sensitive data | Local | Data privacy |
| Batch processing | Local | Cost optimization |

### Router Implementation Pattern

```csharp
public class ModelRouter : IModelRouter
{
    public IModel SelectModel(AgentRequest request)
    {
        if (request.RequiresHighQuality || request.TokenCount > 4000)
            return _cloudModel;

        if (request.ContainsSensitiveData)
            return _localModel;

        return _localModel; // Default to local
    }
}
```

---

## Azure Services Used

| Service | Purpose | Required? |
|---------|---------|-----------|
| **Azure OpenAI** | Cloud LLM inference | Optional |
| **Azure AI Search** | RAG and document search | Optional |
| **Azure Blob Storage** | Document and artifact storage | Optional |
| **Azure Key Vault** | Secrets management | Recommended |
| **Azure Monitor** | Observability and logging | Optional |

---

## Configuration Example

```json
{
  "runtime": {
    "mode": "hybrid",
    "routing": "auto"
  },
  "models": {
    "local": {
      "provider": "ollama",
      "endpoint": "http://localhost:11434",
      "model": "llama3.2"
    },
    "cloud": {
      "provider": "azure-openai",
      "endpoint": "https://your-instance.openai.azure.com/",
      "deployment": "gpt-4o",
      "apiKeySource": "environment"
    }
  },
  "routing": {
    "default": "local",
    "escalateOn": ["high-complexity", "large-context"],
    "forceLocal": ["sensitive-data"]
  }
}
```

---

## Benefits

- **Best of both worlds** — local speed + cloud power
- **Cost optimization** — use cloud only when needed
- **Graceful degradation** — works offline with local models
- **Data control** — sensitive data stays local

## Considerations

- **Network dependency** for cloud calls
- **Latency variance** between local and cloud
- **API key management** and security
- **Cost monitoring** for cloud usage

---

## Next Steps

- See [Local Architecture](local-agentic.md) for local-only patterns
- See [Azure Setup](../../infra/azure/README.md) for cloud deployment
