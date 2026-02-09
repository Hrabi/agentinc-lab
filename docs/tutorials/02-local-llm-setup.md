# Tutorial 02: Local LLM Setup

> Configure and run large language models locally for use with AgenticLab.
>
> **Target machine:** Lenovo 83EY · Intel Core Ultra 9 275HX (24 cores) · 192 GB DDR5 · NVIDIA RTX 5090 Laptop GPU 24 GB · Windows 11 Pro

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

[Ollama](https://ollama.com/) (v0.15.6, [162k+ stars on GitHub](https://github.com/ollama/ollama), MIT license) is the easiest way to run LLMs locally. Built on [llama.cpp](https://github.com/ggml-org/llama.cpp), it provides a simple CLI and REST API for model management and inference. Supports GGUF and Safetensors imports.

### Installation

```powershell
# Windows — use winget:
winget install Ollama.Ollama

# Or download from https://ollama.com/download/OllamaSetup.exe

# Linux:
# curl -fsSL https://ollama.com/install.sh | sh
```

### Pull Models (recommended for RTX 5090)

```powershell
# Best all-rounder for your VRAM (9 GB)
ollama pull qwen2.5:14b

# Fast utility model — Gemma 3 4B (3.3 GB, Ollama's default quickstart model)
ollama pull gemma3

# Lightweight fast model (2.0 GB)
ollama pull llama3.2

# Near-cloud quality (17 GB)
ollama pull gemma3:27b

# Reasoning specialist (20 GB)
ollama pull deepseek-r1:32b

# Alternative reasoning model (20 GB)
ollama pull qwq

# Phi 4 — strong 14B alternative (9.1 GB)
ollama pull phi4

# Embedding model for RAG
ollama pull nomic-embed-text

# If you want to push VRAM limits — needs ~18 GB RAM offload
ollama pull llama3.3:70b
```

### Verify

```powershell
ollama list                                     # List downloaded models
ollama ps                                       # List currently loaded models
ollama run gemma3 "What is an agentic system?"   # Chat with a model
ollama show gemma3                               # Show model info
ollama stop gemma3                               # Unload a model from memory
```

### Generate Embeddings

```powershell
ollama run nomic-embed-text "Your text to embed"
```

### API Access

Ollama exposes a REST API at `http://localhost:11434`:

```powershell
# Chat endpoint (recommended)
curl http://localhost:11434/api/chat -d '{
  "model": "qwen2.5:14b",
  "messages": [{"role": "user", "content": "What is an agentic system?"}]
}'

# Generate endpoint (single prompt)
curl http://localhost:11434/api/generate -d '{
  "model": "qwen2.5:14b",
  "prompt": "Why is the sky blue?"
}'
```

### Custom Models (Modelfile)

Ollama supports customizing models with a `Modelfile`:

```dockerfile
FROM qwen2.5:14b

PARAMETER temperature 0.1

SYSTEM """
You are a CRM data extraction specialist.
Extract structured JSON from user messages.
"""
```

```powershell
ollama create crm-extractor -f ./Modelfile
ollama run crm-extractor
```

### Import Custom GGUF Models

```dockerfile
# Modelfile
FROM ./my-custom-model.Q4_K_M.gguf
```

```powershell
ollama create my-model -f Modelfile
ollama run my-model
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

Ollama memory requirements: **8 GB RAM for 7B models, 16 GB for 13B, 32 GB for 33B.**
With 192 GB DDR5 and 24 GB VRAM, your machine can run any model up to 33B fully on GPU, and much larger models with RAM offload.

| Model | Params | Size | Speed (est.) | Quality | Best For |
|-------|--------|-----:|:-------------|---------|----------|
| Gemma 3 1B | 1B | 815 MB | ~150 tok/s | Decent | Tiny routing, edge tasks |
| Llama 3.2 1B | 1B | 1.3 GB | ~140 tok/s | Decent | Ultra-fast classification |
| Llama 3.2 3B | 3B | 2.0 GB | ~120 tok/s | Good | Classification, routing |
| Phi 4 Mini | 3.8B | 2.5 GB | ~110 tok/s | Good | SLM tasks, quick lookups |
| Gemma 3 4B | 4B | 3.3 GB | ~100 tok/s | Good | **Fast utility** (Ollama default) |
| DeepSeek-R1 7B | 7B | 4.7 GB | ~80 tok/s | Better | Reasoning (small) |
| Llama 3.1 8B | 8B | 4.7 GB | ~75 tok/s | Better | Quick tasks, tool use |
| Mistral 7B | 7B | 4.1 GB | ~80 tok/s | Better | Local coding assistant |
| Granite 3.3 8B | 8B | 4.9 GB | ~75 tok/s | Better | Enterprise text tasks |
| Gemma 3 12B | 12B | 8.1 GB | ~55 tok/s | Very Good | Balanced quality/speed |
| Qwen 2.5 14B | 14B | ~9 GB | ~50 tok/s | Excellent | **Best local all-rounder** |
| Phi 4 | 14B | 9.1 GB | ~50 tok/s | Excellent | Strong 14B alternative |
| Gemma 3 27B | 27B | 17 GB | ~30 tok/s | Near-Cloud | Complex reasoning |
| QwQ | 32B | 20 GB | ~25 tok/s | Excellent | Reasoning specialist |
| DeepSeek-R1 32B | 32B | ~20 GB | ~25 tok/s | Excellent | Reasoning chains |
| Llama 3.3 70B | 70B | 43 GB | ~15 tok/s | Cloud-tier | Complex tasks (uses RAM offload) |
| Llama 4 Scout | 109B | 67 GB | ~8 tok/s | Top | Latest Llama (heavy offload) |

### Developer Recommendation

For AgenticLab development, start with this combination:

```
┌──────────────────────────────────────────────────────────────┐
│  Qwen 2.5 14B   — primary dev model (fits in GPU, fast)     │
│  Gemma 3 4B     — fast routing / classification agent        │
│  Phi 4 Mini     — ultra-fast SLM for simple tasks            │
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

## Ollama Reference

- **Official website:** [ollama.com](https://ollama.com/)
- **GitHub:** [github.com/ollama/ollama](https://github.com/ollama/ollama) (162k+ stars, MIT license)
- **Current version:** v0.15.6
- **Model library:** [ollama.com/library](https://ollama.com/library)
- **REST API docs:** [github.com/ollama/ollama/blob/main/docs/api.md](https://github.com/ollama/ollama/blob/main/docs/api.md)
- **Modelfile docs:** [docs.ollama.com/modelfile](https://docs.ollama.com/modelfile)
- **Python library:** [ollama-python](https://github.com/ollama/ollama-python)
- **JS library:** [ollama-js](https://github.com/ollama/ollama-js)
- **.NET library:** [OllamaSharp](https://github.com/awaescher/OllamaSharp)
- **Community:** [Discord](https://discord.gg/ollama) · [Reddit](https://reddit.com/r/ollama)

---

## Next Steps

- [Tutorial 03: Building Your First Agent](03-first-agent.md)
- [Local Architecture](../architecture/local-agentic.md)
- [Docker Setup Details](../../infra/docker/README.md)
