# RAG Demo Data

This folder contains sample documents for testing the RAG (Retrieval-Augmented Generation) pipeline.

## Files

| File | Type | Description | Approx. Size |
|------|------|-------------|-------------|
| `company-handbook.md` | Markdown | Employee policies: leave, expenses, security, returns | ~3,500 words |
| `product-catalog.json` | JSON | Full product catalog with 8 products, specs, pricing | ~250 fields |
| `OrderRepository.cs` | C# | Order domain model: Order, OrderLine, Address, OrderStatus | ~100 LOC |
| `OrderService.cs` | C# | Order repository with validation, CRUD, returns, statistics | ~160 LOC |
| `smarthub-api-reference.md` | Markdown | SmartHub 400 REST + WebSocket API docs | ~2,000 words |
| `quarterly-report-q4-2025.md` | Markdown | Q4 2025 financial report: revenue, metrics, outlook | ~1,500 words |
| `faq.md` | Markdown | Customer FAQ: ordering, returns, products, warranty, tech | ~1,800 words |

## Sample Questions for Testing

These questions span multiple documents and test the RAG pipeline's ability to find and synthesize relevant information:

### Cross-document questions
1. "What is the return policy and how do I initiate a return?" → handbook + FAQ
2. "How much does the SmartHub 400 cost and what protocols does it support?" → catalog + FAQ
3. "What warranty comes with industrial IoT sensors?" → handbook + catalog + FAQ
4. "How did the SmartHub 400 perform in Q4 2025?" → quarterly report + catalog

### Single-document questions
5. "What are the overtime pay rates?" → handbook
6. "How do I send a command to a device via the SmartHub API?" → API reference
7. "What validation does the OrderRepository perform?" → OrderService.cs
8. "What is the price of the SoundBuds NC3?" → catalog
9. "What is the Q1 2026 revenue guidance?" → quarterly report
10. "How do I file a warranty claim?" → FAQ

### Questions that should NOT be answerable (hallucination test)
11. "What is Contoso's stock price?" → not in any document
12. "Who is the CTO?" → not in any document (CEO and CFO are mentioned)
13. "Does Contoso sell televisions?" → no, not in catalog

## Expected Chunking

With a 512-token / 64-overlap chunking strategy, expect approximately:

- `company-handbook.md` → ~12-15 chunks
- `product-catalog.json` → ~8-10 chunks
- `OrderRepository.cs` → ~4-5 chunks
- `OrderService.cs` → ~5-7 chunks
- `smarthub-api-reference.md` → ~8-10 chunks
- `quarterly-report-q4-2025.md` → ~6-8 chunks
- `faq.md` → ~7-9 chunks

**Total: ~50-65 chunks** → ~50-65 vectors in Qdrant
