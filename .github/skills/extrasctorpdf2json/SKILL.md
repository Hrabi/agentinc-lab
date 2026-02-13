---
name: extrasctorpdf2json
description: Extract structured data from PDF documents and convert it to JSON. Use when an agent needs to parse PDF files (invoices, reports, forms, catalogs), extract text/tables/metadata, and output clean JSON for downstream processing, RAG ingestion, or data analysis. Keywords: PDF parsing, text extraction, document processing, PDF-to-JSON, data extraction, table extraction, OCR, ingestion, structured output.
---

# Skill: PDF to JSON Extractor

## Purpose

Extract text, tables, and metadata from PDF documents and produce structured JSON output. This skill is the foundation of the document ingestion pipeline — transforming unstructured PDF content into machine-readable JSON that can be embedded, stored in a vector database, or consumed by other agents.

## When to Use

- User asks to **extract data from a PDF** file
- User wants to **convert PDF content to JSON** format
- A pipeline needs to **ingest PDF documents** for RAG (Retrieval-Augmented Generation)
- User wants to **parse invoices, reports, forms, or catalogs** from PDF
- User needs **structured output** from an unstructured PDF source
- User asks to **extract tables** from PDF documents

## Technology Stack

| Component | Choice | Reason |
|-----------|--------|--------|
| PDF text extraction | `UglyToad.PdfPig` (NuGet) | Pure C#, no native dependencies, MIT license |
| OCR fallback | Ollama `llava` vision model or `Tesseract` | For scanned/image-only PDFs |
| JSON serialization | `System.Text.Json` | Built-in, fast, .NET standard |
| Runtime | .NET 10 / C# 14 | Project standard |

## Implementation Instructions

### Step 1: Install the PDF Extraction Package

Add `UglyToad.PdfPig` to the project that performs extraction:

```xml
<PackageReference Include="UglyToad.PdfPig" Version="0.4.*" />
```

### Step 2: Create the PDF Loader

```csharp
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AgenticLab.Ingestion.Loaders;

/// <summary>
/// Extracts text content from PDF files using PdfPig.
/// </summary>
public class PdfLoader : IDocumentLoader
{
    /// <summary>
    /// Supported file extensions for this loader.
    /// </summary>
    public IReadOnlyList<string> SupportedExtensions => [".pdf"];

    /// <summary>
    /// Loads and extracts text from a PDF file.
    /// </summary>
    public Task<ExtractedDocument> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var document = PdfDocument.Open(filePath);

        var pages = new List<PageContent>();

        foreach (var page in document.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var text = page.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                pages.Add(new PageContent
                {
                    PageNumber = page.Number,
                    Text = text.Trim(),
                    Width = page.Width,
                    Height = page.Height
                });
            }
        }

        var result = new ExtractedDocument
        {
            SourceFile = filePath,
            FileName = Path.GetFileName(filePath),
            PageCount = document.NumberOfPages,
            Pages = pages,
            Metadata = ExtractMetadata(document)
        };

        return Task.FromResult(result);
    }

    private static Dictionary<string, string> ExtractMetadata(PdfDocument document)
    {
        var metadata = new Dictionary<string, string>
        {
            ["pageCount"] = document.NumberOfPages.ToString(),
            ["format"] = "pdf"
        };

        var info = document.Information;
        if (!string.IsNullOrWhiteSpace(info.Title))
            metadata["title"] = info.Title;
        if (!string.IsNullOrWhiteSpace(info.Author))
            metadata["author"] = info.Author;
        if (!string.IsNullOrWhiteSpace(info.Subject))
            metadata["subject"] = info.Subject;
        if (!string.IsNullOrWhiteSpace(info.Creator))
            metadata["creator"] = info.Creator;
        if (info.CreationDate.HasValue)
            metadata["createdDate"] = info.CreationDate.Value.ToString("o");

        return metadata;
    }
}
```

### Step 3: Define the Data Models

```csharp
namespace AgenticLab.Ingestion.Loaders;

/// <summary>
/// Represents extracted content from a document.
/// </summary>
public class ExtractedDocument
{
    public required string SourceFile { get; init; }
    public required string FileName { get; init; }
    public required int PageCount { get; init; }
    public required List<PageContent> Pages { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Represents extracted content from a single page.
/// </summary>
public class PageContent
{
    public required int PageNumber { get; init; }
    public required string Text { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}
```

### Step 4: Convert to JSON

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgenticLab.Ingestion;

/// <summary>
/// Converts extracted PDF content to structured JSON.
/// </summary>
public static class PdfToJsonConverter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Converts an extracted document to a JSON string.
    /// </summary>
    public static string ToJson(ExtractedDocument document)
    {
        return JsonSerializer.Serialize(document, JsonOptions);
    }

    /// <summary>
    /// Converts an extracted document and writes JSON to a file.
    /// </summary>
    public static async Task ToJsonFileAsync(
        ExtractedDocument document,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(outputPath);
        await JsonSerializer.SerializeAsync(stream, document, JsonOptions, cancellationToken);
    }
}
```

### Step 5: Full Pipeline Usage Example

```csharp
// Single file extraction
var loader = new PdfLoader();
var extracted = await loader.LoadAsync("data/invoice.pdf");
var json = PdfToJsonConverter.ToJson(extracted);
Console.WriteLine(json);

// Batch extraction from a folder
var pdfFiles = Directory.GetFiles("data/rag-demo", "*.pdf");
foreach (var pdf in pdfFiles)
{
    var doc = await loader.LoadAsync(pdf);
    var outputPath = Path.ChangeExtension(pdf, ".json");
    await PdfToJsonConverter.ToJsonFileAsync(doc, outputPath);
}
```

### Example JSON Output

```json
{
  "sourceFile": "data/rag-demo/quarterly-report-q4-2025.pdf",
  "fileName": "quarterly-report-q4-2025.pdf",
  "pageCount": 5,
  "pages": [
    {
      "pageNumber": 1,
      "text": "Quarterly Financial Report — Q4 2025\n\nRevenue: $12.4M (+18% YoY)\nNet Income: $2.1M...",
      "width": 612.0,
      "height": 792.0
    },
    {
      "pageNumber": 2,
      "text": "Product Performance\n\nSmartHub Pro: 45,000 units sold...",
      "width": 612.0,
      "height": 792.0
    }
  ],
  "metadata": {
    "pageCount": "5",
    "format": "pdf",
    "title": "Q4 2025 Financial Report",
    "author": "Finance Department",
    "createdDate": "2025-12-15T10:30:00.0000000+00:00"
  }
}
```

## Integration with RAG Pipeline

This skill feeds into the broader AgenticLab RAG pipeline:

```
PDF file → PdfLoader (this skill) → ExtractedDocument → Chunker → Embeddings → Qdrant → RagAgent
```

After extraction, the JSON output can be:
1. **Chunked** — split into smaller pieces (512 tokens with 64-token overlap)
2. **Embedded** — converted to vectors via Ollama `nomic-embed-text`
3. **Stored** — upserted into Qdrant vector database
4. **Queried** — retrieved by the RagAgent to answer user questions with source citations

## Limitations

- **Scanned PDFs**: PdfPig extracts embedded text only. For scanned/image PDFs, use OCR (Tesseract) or a vision model (Ollama `llava`) as a fallback.
- **Complex tables**: PdfPig extracts text in reading order. For precise table extraction, consider supplementing with layout analysis.
- **Encrypted PDFs**: Password-protected PDFs require the password to be supplied to `PdfDocument.Open()`.