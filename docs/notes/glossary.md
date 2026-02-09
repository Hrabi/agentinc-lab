# Glossary

> Key terms and definitions used throughout AgenticLab.

---

## A

### Agent
An autonomous software component that receives a goal, plans how to achieve it, uses tools and models to execute actions, and reports results. Agents can operate independently or collaborate with other agents.

### Agentic System
A software architecture composed of one or more agents that work together to accomplish complex tasks with minimal human intervention.

### Azure OpenAI Service
A Microsoft Azure service that provides access to OpenAI models (GPT-4, GPT-4o, etc.) with enterprise security, compliance, and regional availability.

---

## C

### Chain of Thought (CoT)
A prompting technique where the model is encouraged to reason step-by-step before providing a final answer, improving accuracy on complex tasks.

### Composability
A design principle where components (agents, tools, models) can be freely combined and replaced without affecting the overall system.

---

## D

### Dependency Injection (DI)
A design pattern where components receive their dependencies from external configuration rather than creating them internally. Core to AgenticLab's architecture.

---

## G

### GGUF
A file format for storing quantized LLM models, commonly used by Ollama and llama.cpp. Optimized for CPU and mixed CPU/GPU inference. Ollama can import custom GGUF models via a `Modelfile` with a `FROM ./model.gguf` directive.

### Grounding
The process of connecting an LLM's responses to factual, verifiable information sources such as documents, databases, or APIs.

---

## H

### Hybrid Architecture
An architecture where some components run locally and others run in the cloud, balancing cost, privacy, and capability.

---

## L

### LLM (Large Language Model)
A neural network trained on large amounts of text data that can generate, summarize, translate, and reason about text. Examples: GPT-5, Llama 4, Llama 3.3, Gemma 3, Phi 4, Qwen 2.5, DeepSeek-R1, Mistral.

### Local-First
A development philosophy where applications are designed to work fully on a developer's machine without requiring cloud services.

---

## M

### Model Router
A component that decides whether to route a request to a local model or a cloud model based on criteria like complexity, sensitivity, and cost.

### Multi-Agent System
A system where multiple agents collaborate, each with specialized capabilities, to accomplish tasks that are too complex for a single agent.

---

## O

### Ollama
An open-source tool (v0.15.6, 162k+ GitHub stars, MIT license) for running LLMs locally. Built on [llama.cpp](https://github.com/ggml-org/llama.cpp), it provides a simple CLI and REST API for model management and inference. Supports GGUF and Safetensors model imports, customizable Modelfiles, concurrent model serving, and GPU auto-detection. Available for Windows, macOS, Linux, and Docker. See [ollama.com](https://ollama.com/) and [GitHub](https://github.com/ollama/ollama).

### ONNX (Open Neural Network Exchange)
An open format for representing machine learning models. ONNX Runtime provides optimized inference for .NET applications.

### Orchestration
The process of coordinating multiple agents, managing their lifecycle, routing messages, and aggregating results.

---

## P

### Prompt Engineering
The practice of designing and optimizing prompts (instructions) sent to LLMs to achieve desired outputs.

---

## R

### RAG (Retrieval-Augmented Generation)
A pattern where relevant documents are retrieved from a knowledge base and included in the LLM prompt to improve accuracy and reduce hallucination.

### Runtime
The execution engine that manages agent lifecycle, message routing, and system coordination. See `AgenticLab.Runtime`.

---

## S

### Semantic Kernel
A Microsoft open-source SDK for integrating LLMs into applications. Used as a reference framework in AgenticLab.

---

## T

### Temperature
A parameter controlling the randomness of LLM outputs. Lower values (0.0-0.3) produce more deterministic outputs; higher values (0.7-1.0) produce more creative outputs.

### Tool
A capability that an agent can invoke to interact with external systems (file system, APIs, databases, etc.). Tools extend an agent's abilities beyond text generation.

### Token
The basic unit of text processing in LLMs. A token is roughly 3/4 of a word in English. Models have context windows measured in tokens.

---

## M (continued)

### Modelfile
An Ollama configuration file used to customize models with system prompts, parameters (temperature, top_p), and adapter layers. Similar to a Dockerfile but for LLMs. See [Ollama Modelfile docs](https://docs.ollama.com/modelfile).

---

## V

### Vector Database
A database optimized for storing and searching high-dimensional vectors, used in RAG systems for semantic similarity search.
