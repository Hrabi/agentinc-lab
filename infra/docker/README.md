# Local LLM Docker Setup

Docker Compose configurations for running LLMs locally with GPU acceleration.

> **Target machine:** Intel Core Ultra 9 275HX · 192 GB DDR5 · RTX 5090 24 GB · Windows 11 Pro

## Prerequisites

- **Docker Desktop** with WSL2 backend enabled
- **NVIDIA GPU drivers** (Game Ready or Studio, 570+ for RTX 5090)
- **NVIDIA Container Toolkit** — installed automatically by Docker Desktop on Windows with WSL2

Verify GPU access in Docker:
```powershell
docker run --rm --gpus all nvidia/cuda:12.8.0-base-ubuntu24.04 nvidia-smi
```

## Quick Start

```powershell
# Start Ollama with GPU (default profile)
docker compose up -d

# Pull default models (qwen2.5:14b, llama3.2, nomic-embed-text)
docker compose up ollama-init

# Test it
curl http://localhost:11434/api/generate -d '{"model":"qwen2.5:14b","prompt":"Hello!"}'
```

## Profiles

| Command | What starts | Ports |
|---------|-------------|-------|
| `docker compose up -d` | Ollama only | 11434 |
| `docker compose --profile vllm up -d` | Ollama + vLLM | 11434, 8000 |
| `docker compose --profile webui up -d` | Ollama + Open WebUI | 11434, 3000 |
| `docker compose --profile all up -d` | Everything | 11434, 8000, 3000 |

## Services

### Ollama (port 11434)

Best for quick development iteration. Pull models with `ollama pull <model>`.

```powershell
# Pull additional models
docker exec agenticlab-ollama ollama pull mistral-nemo
docker exec agenticlab-ollama ollama pull deepseek-r1:32b
docker exec agenticlab-ollama ollama pull gemma3:27b

# List loaded models
docker exec agenticlab-ollama ollama list
```

**API:** Custom Ollama API at `http://localhost:11434`
- Endpoint: `POST /api/generate` (completions) or `POST /api/chat` (chat)

### vLLM (port 8000)

Production-grade OpenAI-compatible API. Better throughput for multi-agent workloads.

```powershell
# Test with OpenAI-compatible API
curl http://localhost:8000/v1/chat/completions `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer agenticlab-dev" `
  -d '{"model":"qwen2.5-14b","messages":[{"role":"user","content":"Hello!"}]}'
```

**API:** OpenAI-compatible at `http://localhost:8000/v1`
- Supports `/v1/chat/completions`, `/v1/completions`, `/v1/models`
- Default API key: `agenticlab-dev` (change via `VLLM_API_KEY` env var)

### Open WebUI (port 3000)

Browser-based chat UI for testing models interactively. Connects to both Ollama and vLLM.

Open `http://localhost:3000` after starting.

## Ollama vs vLLM — When to Use Which

| Scenario | Use |
|----------|-----|
| Exploring / pulling new models | **Ollama** |
| Single-agent prototyping | **Ollama** |
| Multi-agent concurrent load | **vLLM** |
| OpenAI API compatibility needed | **vLLM** |
| Quick model switching | **Ollama** |
| Production-like benchmarking | **vLLM** |
| Quantized GGUF models (Q4/Q5/Q8) | **Ollama** |
| HuggingFace FP16 / AWQ / GPTQ models | **vLLM** |

## Recommended Models for RTX 5090 (24 GB VRAM)

| Model | VRAM (Q4) | Pull Command |
|-------|----------:|-------------|
| Qwen 2.5 14B | ~9 GB | `ollama pull qwen2.5:14b` |
| Llama 3.2 8B | ~5 GB | `ollama pull llama3.2` |
| Gemma 3 27B | ~16 GB | `ollama pull gemma3:27b` |
| Mistral Nemo 12B | ~7 GB | `ollama pull mistral-nemo` |
| DeepSeek R1 Distill 32B | ~20 GB | `ollama pull deepseek-r1:32b` |
| Llama 3.3 70B (offload) | 24+18 GB | `ollama pull llama3.3:70b` |

## Environment Variables

Create a `.env` file in this directory for sensitive config:

```env
# HuggingFace token (needed for gated models like Llama 3.3)
HF_TOKEN=hf_your_token_here

# vLLM API key (default: agenticlab-dev)
VLLM_API_KEY=your-key-here
```

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `nvidia-smi` not found in container | Ensure NVIDIA Container Toolkit is installed; restart Docker Desktop |
| Ollama OOM on large model | Reduce `OLLAMA_MAX_LOADED_MODELS` or use smaller quant |
| vLLM slow to start | Normal — loading 14B weights takes 1–2 min; check `docker logs agenticlab-vllm` |
| Port conflict on 11434 | Stop native Ollama if running: `ollama stop` or `taskkill /f /im ollama.exe` |

## Data Persistence

All data is stored in Docker volumes:
- `ollama-data` — downloaded Ollama models
- `huggingface-cache` — HuggingFace model weights for vLLM
- `webui-data` — Open WebUI settings and chat history
