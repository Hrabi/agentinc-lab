# K-Quant — Mixed Precision Quantization by Tensor Importance

> Deep dive into how llama.cpp's K-Quant system assigns different bit depths to different tensor types, and why it outperforms uniform quantization.

---

## The Core Insight

Not all weights in a neural network are equally important. **K-Quant** (developed by [llama.cpp](https://github.com/ggerganov/llama.cpp)) assigns different bit depths to different tensor types based on their **sensitivity** — how much each tensor's precision affects model output quality.

This is fundamentally different from older uniform quantization (Q4_0, Q4_1) which applied the same bit depth to every weight in the network.

---

## Transformer Architecture — Which Tensors Matter Most?

A transformer model (the architecture behind Llama, Gemma, Qwen, Phi, etc.) is built from repeating blocks, each containing:

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│                        TRANSFORMER BLOCK (×N layers)                       │
│                                                                            │
│   ┌───────────────────────────────────────────────────────────────────┐    │
│   │  SELF-ATTENTION                                                   │    │
│   │                                                                   │    │
│   │  Q (Query)  ─┐                                                    │    │
│   │  K (Key)    ─┤──→  Attention Scores  ──→  Weighted Sum ──→ Output │    │
│   │  V (Value)  ─┘                                                    │    │
│   │  O (Output projection)                                            │    │
│   │                                                                   │    │
│   │  These tensors control WHAT tokens attend to each other.          │    │
│   │  Errors here → incoherent text, broken reasoning, hallucination.  │    │
│   └───────────────────────────────────────────────────────────────────┘    │
│                                                                            │
│   ┌───────────────────────────────────────────────────────────────────┐    │
│   │  FEED-FORWARD NETWORK (FFN / MLP)                                 │    │
│   │                                                                   │    │
│   │  gate_proj  ─┐                                                    │    │
│   │  up_proj    ─┤──→  Activation  ──→  down_proj ──→ Output          │    │
│   │              │                                                    │    │
│   │  These tensors transform representations.                         │    │
│   │  More redundant — tolerate compression better.                    │    │
│   └───────────────────────────────────────────────────────────────────┘    │
│                                                                            │
│   Layer Norm / RMSNorm (always kept at high precision — tiny tensors)      │
└─────────────────────────────────────────────────────────────────────────────┘
```

Plus two special layers:

- **Token embedding** (first layer) — maps tokens to vectors; errors here affect *every* subsequent computation
- **Output head / lm_head** (last layer) — produces the probability distribution over vocabulary; errors here directly distort which word the model picks

---

## K-Quant Bit Allocation Strategy

K-Quant uses a tiered system. Here's how **Q4_K_M** (the most popular variant) allocates bits:

| Tensor Type | Role | Q4_K_M Precision | Why |
|-------------|------|:-----------------:|-----|
| **token_embd** | Token → vector mapping | 6 bits (Q6_K) | First layer; errors cascade through all subsequent layers |
| **output / lm_head** | Vector → token probabilities | 6 bits (Q6_K) | Last layer; directly determines word choice |
| **attn_q** | Query projections | 5-6 bits | Controls matching between current position and context |
| **attn_k** | Key projections | 5-6 bits | Defines what each position "advertises" to queries |
| **attn_v** | Value projections | 4-5 bits | Carries information; somewhat more compressible |
| **attn_output** | Attention output projection | 4-5 bits | Combines attended values |
| **ffn_gate** | FFN gating (SwiGLU) | 4 bits (Q4_K) | Activation gating; moderately sensitive |
| **ffn_up** | FFN up projection | 4 bits (Q4_K) | Expansion into wider space; redundant |
| **ffn_down** | FFN down projection | 4 bits (Q4_K) | Compression back; most compressible |
| **norm weights** | RMSNorm / LayerNorm | FP32 | Tiny (a few KB); kept exact |

**Result:** The average bits/weight across the whole model is ~4.8 (hence "Q4"), but critical tensors get 5–6 bits, pushing effective quality closer to Q5 or Q6.

### Comparison with Uniform Q4_0

```text
┌──────────────────────────────────────────────────────────────────────────┐
│            UNIFORM vs K-QUANT at ~4.5 average bits/weight               │
│                                                                          │
│  Q4_0 (Uniform):                                                         │
│    Every tensor:  ████  4 bits                                           │
│    Attention Q:   ████  4 bits  ← same as FFN                           │
│    Attention K:   ████  4 bits                                           │
│    FFN weights:   ████  4 bits                                           │
│    Embedding:     ████  4 bits  ← most damage here                      │
│    Output head:   ████  4 bits  ← and here                              │
│                                                                          │
│  Q4_K_M (K-Quant):                                                       │
│    Every tensor:  varies by importance                                   │
│    Attention Q:   ██████  5-6 bits  ← protected                         │
│    Attention K:   ██████  5-6 bits  ← protected                         │
│    FFN weights:   ████    4 bits    ← compressed                        │
│    Embedding:     ██████  6 bits    ← protected                         │
│    Output head:   ██████  6 bits    ← protected                         │
│                                                                          │
│  Same average bits → dramatically better quality                         │
│  Q4_K_M PPL ≈ 6.12  vs  Q4_0 PPL ≈ 6.35 (for Llama 3.2 8B)            │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## The S / M / L Variants

Within K-Quant, the suffix controls **how many tensors get boosted precision**:

| Variant | Strategy | Effect |
|---------|----------|--------|
| **K_S** (Small) | Only the most critical tensors (embedding, output head) get high bits | Smallest file, more quality loss |
| **K_M** (Medium) | Critical tensors + attention tensors get high bits | ⭐ Best tradeoff |
| **K_L** (Large) | Almost all tensors except FFN get high bits | Larger file, minimal quality loss vs Q5 |

### Concrete Bit Allocation Comparison (Llama-style architecture)

| Tensor | Q4_K_S | Q4_K_M | Q4_K_L |
|--------|:------:|:------:|:------:|
| token_embd | Q5_K | Q6_K | Q6_K |
| output/lm_head | Q6_K | Q6_K | Q6_K |
| attn_q | Q4_K | Q5_K/Q6_K | Q6_K |
| attn_k | Q4_K | Q5_K/Q6_K | Q6_K |
| attn_v | Q4_K | Q4_K/Q5_K | Q5_K |
| attn_output | Q4_K | Q4_K/Q5_K | Q5_K |
| ffn_gate | Q4_K | Q4_K | Q5_K |
| ffn_up | Q4_K | Q4_K | Q4_K |
| ffn_down | Q4_K | Q4_K | Q4_K |
| **Avg bits/weight** | **~4.5** | **~4.8** | **~5.1** |

---

## How Quantization Works Internally — Block Quantization

K-Quant doesn't just round each weight independently. It uses **block quantization** with scale factors:

### Step-by-Step Process

1. **Group weights into blocks** (typically 32 or 256 weights per block)
2. **Find the range** of values in each block: $[\text{min}, \text{max}]$
3. **Compute a scale factor** and zero point for the block:
   $$\text{scale} = \frac{\text{max} - \text{min}}{2^B - 1}$$
   where $B$ = target bits (e.g., 4)
4. **Quantize** each weight in the block:
   $$q_i = \text{round}\!\left(\frac{w_i - \text{min}}{\text{scale}}\right)$$
5. **Store** the quantized integers + per-block scale factor + min value

### Dequantization (at inference time)

$$\hat{w}_i = q_i \times \text{scale} + \text{min}$$

The **quantization error** for each weight is:

$$\epsilon_i = w_i - \hat{w}_i$$

This error is bounded by $\frac{\text{scale}}{2}$, which depends on the range of weights in the block and the number of bits.

### K-Quant's Innovation — Super-Blocks

K-Quant introduces **super-blocks** — blocks of blocks — where the scale factors themselves are quantized:

```text
┌─────────────────────────────────────────────────────────────────┐
│  SUPER-BLOCK (256 weights)                                     │
│                                                                 │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │ Block 1  │ │ Block 2  │ │ Block 3  │ │ Block 8  │          │
│  │ 32 wts   │ │ 32 wts   │ │ 32 wts   │ │ 32 wts   │   ...   │
│  │ 4-bit ea │ │ 4-bit ea │ │ 4-bit ea │ │ 4-bit ea │          │
│  │ + scale  │ │ + scale  │ │ + scale  │ │ + scale  │          │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘          │
│                                                                 │
│  Super-scale (FP16): scales the block scales                   │
│  Super-min (FP16): minimum for block minimums                  │
│  Block scales: quantized to 6-bit                              │
│  Block mins: quantized to 6-bit                                │
│                                                                 │
│  This reduces overhead: only 2 FP16 values per 256 weights     │
│  instead of 2 FP16 values per 32 weights.                      │
└─────────────────────────────────────────────────────────────────┘
```

This hierarchical approach is why K-Quant achieves better compression ratios than flat quantization while maintaining quality.

---

## Why Q4_K_M Is the Sweet Spot — The Math

For an 8B parameter model:

| Quantization | Bits | File Size | PPL Delta | Bits Saved vs FP16 | Quality Retained |
|:-------------|:----:|----------:|:---------:|:------------------:|:----------------:|
| FP16 | 16.0 | 16.0 GB | — | — | 100% |
| Q8_0 | 8.0 | 8.0 GB | +0.01 | 50% | 99.8% |
| Q6_K | 6.6 | 6.6 GB | +0.03 | 59% | 99.5% |
| Q5_K_M | 5.5 | 5.5 GB | +0.06 | 66% | 99.0% |
| **Q4_K_M** | **4.8** | **4.8 GB** | **+0.12** | **70%** | **98.0%** |
| Q4_K_S | 4.5 | 4.5 GB | +0.18 | 72% | 97.0% |
| Q3_K_M | 3.9 | 3.9 GB | +0.40 | 76% | 93.3% |
| Q2_K | 2.6 | 2.6 GB | +1.50 | 84% | 75.0% |

The **marginal quality loss per bit saved** increases dramatically below Q4_K_M:

```text
  From FP16 to Q8_0:   saved 50% of bits,  lost  0.2% quality  →  gentle slope
  From Q8_0 to Q6_K:   saved  9% more,     lost  0.3% more     →  gentle slope
  From Q6_K to Q5_K_M: saved  7% more,     lost  0.5% more     →  gentle slope
  From Q5_K_M to Q4_K_M: saved 4% more,    lost  1.0% more     →  still OK ⭐
  ─────────────────────────────────────────────────── knee of the curve ───
  From Q4_K_M to Q3_K_M: saved 6% more,    lost  4.7% more     →  steep drop!
  From Q3_K_M to Q2_K:   saved 8% more,    lost 18.3% more     →  cliff edge!
```

Q4_K_M sits right at the **knee of the quality-size curve** — the point where further compression starts producing disproportionate quality loss.

---

## Newer Quantization Methods

Beyond K-Quant, llama.cpp has added **importance-matrix quantization** (IQ/i-quant):

| Method | Key Innovation | Used In |
|--------|---------------|---------|
| **Q_0 / Q_1** | Uniform rounding per block | Legacy (Q4_0, Q5_1) — obsolete |
| **K-Quant** | Mixed precision by tensor type | Q*_K_S/M/L — current standard |
| **IQ (i-quant)** | Importance-weighted within blocks + lattice coding | IQ2_XXS, IQ3_M — extreme compression |

IQ methods use a **calibration dataset** to identify which individual weights (not just tensor types) matter most, achieving even better quality at 2–3 bits. However, they require more compute during quantization and are primarily useful when VRAM is extremely constrained.

---

## Practical Implications for AgenticLab

| Scenario | Recommendation | Why |
|----------|---------------|-----|
| Default deployment | Q4_K_M | Best quality/size tradeoff |
| Quality-critical (code gen, reasoning) | Q6_K or Q8_0 | Minimal loss, still fits in 24GB for models ≤27B |
| Running multiple models simultaneously | Q4_K_S or Q3_K_M | Leave VRAM for other models |
| Maximum VRAM utilization | Q4_K_M sized to ~80% of VRAM | Reserve 20% for KV cache overhead |
| Edge/mobile deployment | IQ3_M or Q3_K_S | Extreme compression for small devices |

### AgenticLab Hybrid Example

With an `IModelRouter`, you can load different quantizations for different tasks:

```csharp
// In DI configuration — two models, different quants
// Ollama can serve both simultaneously
services.AddSingleton<IModel>(sp => new OllamaModel(
    "http://localhost:11434", "qwen2.5:14b",        // Q4_K_M (default)
    sp.GetRequiredService<ILogger<OllamaModel>>()));

services.AddKeyedSingleton<IModel>("highQuality", (sp, _) => new OllamaModel(
    "http://localhost:11434", "qwen2.5:14b-q8_0",   // Q8_0 for precision
    sp.GetRequiredService<ILogger<OllamaModel>>()));
```

---

## References

- [llama.cpp quantization types](https://github.com/ggerganov/llama.cpp/blob/master/ggml/include/ggml.h) — GGML type definitions
- [K-Quant PR by ikawrakow](https://github.com/ggerganov/llama.cpp/pull/1684) — original implementation
- [GGUF format specification](https://github.com/ggerganov/ggml/blob/master/docs/gguf.md) — file format details
- [Ollama model library](https://ollama.com/library) — pre-quantized models
