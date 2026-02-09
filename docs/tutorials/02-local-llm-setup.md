# Tutorial 02: Local LLM Setup

> Configure and run large language models locally for use with AgenticLab.
>
> **Target machine:** Intel Core Ultra 9 275HX · 192 GB DDR5 · RTX 5090 24 GB · Windows 11 Pro

---

## Overview

AgenticLab supports multiple local LLM providers. Choose based on your workflow:

| Provider | Best For | API Style | GPU Support |
|----------|----------|-----------|-------------|
| **Ollama** (native) | Quickest start, dev iteration | Ollama API | Auto-detect |
| **Ollama** (Docker) | Reproducible, team-shared setup | Ollama API | NVIDIA Container Toolkit |
| **vLLM** (Docker) | Production serving, multi-agent | OpenAI-compatible | Full CUDA |
| **ONNX Runtime** | In-process .NET inference | C# library | DirectML / CUDA |

---

## Option 1: Ollama — Native Install (Fastest Start)

[Ollama](https://ollama.com/) is the easiest way to run LLMs locally. Install natively for the simplest experience.

### Installation

```powershell
# Windows — use winget:
winget install Ollama.Ollama

# Or download from https://ollama.com/download
```

### Pull Models (recommended for RTX 5090)

```powershell
# Best all-rounder for your VRAM (9 GB)
ollama pull qwen2.5:14b

# Fast utility model (5 GB)
ollama pull llama3.2

# Near-cloud quality (16 GB)
ollama pull gemma3:27b

# Reasoning specialist (20 GB)
ollama pull deepseek-r1:32b

# Embedding model for RAG
ollama pull nomic-embed-text

# If you want to push VRAM limits — needs ~18 GB RAM offload
ollama pull llama3.3:70b
```

### Verify

```powershell
ollama list
ollama run qwen2.5:14b "What is an agentic system?"
```

### API Access

Ollama exposes a REST API at `http://localhost:11434`:

```powershell
curl http://localhost:11434/api/chat -d '{
  "model": "qwen2.5:14b",
  "messages": [{"role": "user", "content": "What is an agentic system?"}]
}'
```

---

## Option 2: Ollama in Docker (Recommended for Team Dev)

Running Ollama in Docker ensures a reproducible environment and easy GPU management.

### Prerequisites

1. **Docker Desktop** with WSL2 backend
2. **NVIDIA Container Toolkit** (auto-installed by Docker Desktop on Windows)

Verify GPU access:
```powershell
docker run --rm --gpus all nvidia/cuda:12.8.0-base-ubuntu24.04 nvidia-smi
```

### Start with Docker Compose

```powershell
cd infra/docker

# Start Ollama with GPU acceleration
docker compose up -d

# Pull default models (qwen2.5:14b, llama3.2, nomic-embed-text)
docker compose up ollama-init

# Verify
curl http://localhost:11434/api/version
```

### Pull Additional Models

```powershell
docker exec agenticlab-ollama ollama pull gemma3:27b
docker exec agenticlab-ollama ollama pull deepseek-r1:32b
```

> **Tip:** Models persist in the `ollama-data` Docker volume — they survive container restarts.

---

## Option 3: vLLM in Docker (Production-Grade Serving)

[vLLM](https://docs.vllm.ai/) provides an OpenAI-compatible API with superior throughput for concurrent agent requests. Use this when you need:

- **Multiple agents** hitting the model simultaneously
- **OpenAI API compatibility** (same code works with cloud and local)
- **Higher quality** with FP16 / AWQ quantized HuggingFace models

### Start vLLM

```powershell
cd infra/docker

# Start Ollama + vLLM together
docker compose --profile vllm up -d

# Wait ~1-2 min for model loading, then test:
curl http://localhost:8000/v1/chat/completions `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer agenticlab-dev" `
  -d '{"model":"qwen2.5-14b","messages":[{"role":"user","content":"Hello!"}]}'
```

### Advantages Over Ollama

| Feature | Ollama | vLLM |
|---------|--------|------|
| Continuous batching | ❌ | ✅ |
| PagedAttention (less VRAM waste) | ❌ | ✅ |
| Concurrent request throughput | ~1-2x | ~5-10x |
| OpenAI-compatible API | ❌ | ✅ |
| Tensor parallelism (multi-GPU) | ❌ | ✅ |
| HuggingFace model support | via GGUF | Native |

### Using vLLM in AgenticLab

Since vLLM is OpenAI-compatible, you can use the same Azure OpenAI adapter with a different endpoint:

```json
{
  "Models": {
    "LocalVllm": {
      "Provider": "openai-compatible",
      "Endpoint": "http://localhost:8000/v1",
      "ApiKey": "agenticlab-dev",
      "Model": "qwen2.5-14b"
    }
  }
}
```

---

## Option 4: ONNX Runtime (In-Process .NET)

[ONNX Runtime](https://onnxruntime.ai/) provides optimized in-process inference — no separate server needed.

### Setup

```powershell
dotnet add package Microsoft.ML.OnnxRuntimeGenAI
dotnet add package Microsoft.ML.OnnxRuntimeGenAI.Cuda  # for GPU
```

### Usage

```csharp
using Microsoft.ML.OnnxRuntimeGenAI;

var model = new Model("path/to/model");
var tokenizer = new Tokenizer(model);
// ... generate completions
```

> Best for edge deployment and minimal dependencies, but limited model selection.

---

## Option 5: Open WebUI (Browser Chat for Testing)

For interactive testing without code:

```powershell
cd infra/docker
docker compose --profile webui up -d
```

Open `http://localhost:3000` — connects to both Ollama and vLLM.

---

## Model Selection Guide for RTX 5090 (24 GB VRAM)

| Model | Quant | VRAM | Speed (est.) | Quality | Best For |
|-------|-------|-----:|:-------------|---------|----------|
| Llama 3.2 3B | Q4_K_M | ~2 GB | ~120 tok/s | Good | Classification, routing |
| Llama 3.2 8B | Q4_K_M | ~5 GB | ~80 tok/s | Better | Quick tasks, tool use |
| Mistral Nemo 12B | Q4_K_M | ~7 GB | ~60 tok/s | Very Good | Local coding assistant |
| Qwen 2.5 14B | Q4_K_M | ~9 GB | ~50 tok/s | Excellent | **Best local all-rounder** |
| gpt-oss-20B | Q4_K_M | ~12 GB | ~40 tok/s | Excellent | OpenAI open-weight |
| Gemma 3 27B | Q4_K_M | ~16 GB | ~30 tok/s | Near-Cloud | Complex reasoning |
| DeepSeek R1 32B | Q4_K_M | ~20 GB | ~25 tok/s | Excellent | Reasoning chains |
| Llama 3.3 70B | Q4_K_M | 24+18 GB | ~15 tok/s | Cloud-tier | Complex tasks (uses RAM offload) |
| gpt-oss-120B | Q4_K_M | 24+48 GB | ~8 tok/s | Top | Maximum power (heavy offload) |

### Developer Recommendation

For AgenticLab development, start with this combination:

```
┌──────────────────────────────────────────────────────────────┐
│  Qwen 2.5 14B   — primary dev model (fits in GPU, fast)     │
│  Llama 3.2 8B   — fast routing / classification agent       │
│  nomic-embed     — embeddings for RAG pipeline               │
│  Cloud fallback  — Opus 4.6 / GPT-5.2 via IModelRouter      │
└──────────────────────────────────────────────────────────────┘
```

---

## Configuration in AgenticLab

### Ollama (default)

```json
{
  "Models": {
    "Default": {
      "Provider": "ollama",
      "Endpoint": "http://localhost:11434",
      "Model": "qwen2.5:14b",
      "MaxTokens": 2000,
      "Temperature": 0.7
    }
  }
}
```

### vLLM (OpenAI-compatible)

```json
{
  "Models": {
    "Default": {
      "Provider": "openai-compatible",
      "Endpoint": "http://localhost:8000/v1",
      "ApiKey": "agenticlab-dev",
      "Model": "qwen2.5-14b",
      "MaxTokens": 2000,
      "Temperature": 0.7
    }
  }
}
```

### Hybrid (both local + cloud)

```json
{
  "Models": {
    "Local": {
      "Provider": "ollama",
      "Endpoint": "http://localhost:11434",
      "Model": "qwen2.5:14b"
    },
    "Cloud": {
      "Provider": "azure-openai",
      "Endpoint": "https://your-resource.openai.azure.com/",
      "DeploymentName": "gpt-5",
      "ApiKey": "your-key"
    }
  },
  "Routing": {
    "Default": "Local",
    "ComplexTasks": "Cloud",
    "SensitiveData": "Local"
  }
}
```

---

## Next Steps

- [Tutorial 03: Building Your First Agent](03-first-agent.md)
- [Local Architecture](../architecture/local-agentic.md)
- [Docker Setup Details](../../infra/docker/README.md)
