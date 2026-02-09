namespace AgenticLab.Web.Services;

/// <summary>
/// Provides built-in demo examples for testing agents with different scenarios.
/// </summary>
public class ExampleService
{
    private readonly List<DemoExample> _examples;

    public ExampleService()
    {
        _examples =
        [
            // Q&A Examples
            new DemoExample
            {
                Id = "qa-basic",
                Category = "Q&A",
                Title = "Basic Question",
                Description = "Test basic question answering capability.",
                Prompt = "What is the difference between a compiled language and an interpreted language? Give 3 examples of each.",
                ExpectedBehavior = "Clear, structured answer with accurate examples.",
                Tags = ["basic", "knowledge"]
            },
            new DemoExample
            {
                Id = "qa-reasoning",
                Category = "Q&A",
                Title = "Reasoning",
                Description = "Test logical reasoning and step-by-step thinking.",
                Prompt = "A farmer has 17 sheep. All but 9 run away. How many sheep does the farmer have left? Explain your reasoning step by step.",
                ExpectedBehavior = "Should correctly answer 9 with clear logical explanation.",
                Tags = ["reasoning", "math"]
            },

            // Summarization Examples
            new DemoExample
            {
                Id = "sum-article",
                Category = "Summarization",
                Title = "Article Summary",
                Description = "Summarize a technical article into key points.",
                Prompt = """
                    Summarize the following text into 3 bullet points:
                    
                    Machine learning is a subset of artificial intelligence that enables systems to learn and improve from experience without being explicitly programmed. It focuses on the development of computer programs that can access data and use it to learn for themselves. The process begins with observations or data, such as examples, direct experience, or instruction, to look for patterns in data and make better decisions in the future. The primary aim is to allow computers to learn automatically without human intervention or assistance and adjust actions accordingly. Machine learning algorithms are often categorized as supervised, unsupervised, or reinforcement learning. Supervised algorithms can apply what has been learned in the past to new data. Unsupervised algorithms can draw inferences from datasets. Reinforcement learning algorithms learn through trial and error.
                    """,
                ExpectedBehavior = "Three concise, accurate bullet points covering the key concepts.",
                Tags = ["summarization", "text"]
            },

            // Data Extraction Examples
            new DemoExample
            {
                Id = "extract-json",
                Category = "Data Extraction",
                Title = "Extract to JSON",
                Description = "Extract structured data from unstructured text into JSON format.",
                Prompt = """
                    Extract the following information as JSON:
                    
                    John Smith is a 35-year-old software engineer at Microsoft in Seattle.
                    He has 10 years of experience and specializes in cloud computing and distributed systems.
                    His email is john.smith@example.com and his phone number is +1-555-0123.
                    
                    Return a JSON object with fields: name, age, title, company, location, experience_years, specializations (array), email, phone.
                    """,
                ExpectedBehavior = "Valid JSON with all fields correctly populated.",
                Tags = ["extraction", "json", "structured"]
            },
            new DemoExample
            {
                Id = "extract-table",
                Category = "Data Extraction",
                Title = "Extract to Table",
                Description = "Extract tabular data from a description.",
                Prompt = """
                    Convert the following into a markdown table:
                    
                    Our Q1 revenue was $2.5M with a 15% growth rate. Q2 saw $3.1M in revenue, growing 24%. 
                    Q3 revenue reached $2.8M with 12% growth. Q4 closed at $4.2M, the highest growth at 35%.
                    """,
                ExpectedBehavior = "A proper markdown table with Quarter, Revenue, and Growth Rate columns.",
                Tags = ["extraction", "table", "markdown"]
            },

            // Code Generation Examples
            new DemoExample
            {
                Id = "code-function",
                Category = "Code Generation",
                Title = "Generate Function",
                Description = "Generate a C# function from a natural language description.",
                Prompt = "Write a C# method that takes a list of integers and returns the top N most frequent numbers. Include XML doc comments.",
                ExpectedBehavior = "Correct C# code with proper LINQ usage, generics, and XML documentation.",
                Tags = ["code", "csharp"]
            },
            new DemoExample
            {
                Id = "code-api",
                Category = "Code Generation",
                Title = "Generate REST API",
                Description = "Generate a minimal API endpoint in .NET.",
                Prompt = "Write a .NET minimal API endpoint that accepts a POST request with a JSON body containing 'text' and 'language', and returns the text translated to the specified language. Include error handling.",
                ExpectedBehavior = "A complete minimal API endpoint with proper model binding and error handling.",
                Tags = ["code", "api", "dotnet"]
            },

            // Translation Examples
            new DemoExample
            {
                Id = "translate-multi",
                Category = "Translation",
                Title = "Multi-language Translation",
                Description = "Translate text to multiple languages.",
                Prompt = "Translate 'The quick brown fox jumps over the lazy dog' into Spanish, French, German, and Japanese. Format each translation on its own line with the language name.",
                ExpectedBehavior = "Accurate translations in all four languages with proper formatting.",
                Tags = ["translation", "multilingual"]
            },

            // Classification Examples
            new DemoExample
            {
                Id = "classify-sentiment",
                Category = "Classification",
                Title = "Sentiment Analysis",
                Description = "Classify the sentiment of text passages.",
                Prompt = """
                    Classify the sentiment of each message as Positive, Negative, or Neutral:
                    
                    1. "This product exceeded all my expectations! Absolutely love it."
                    2. "The delivery was late and the packaging was damaged."
                    3. "I received the package today. It contained 3 items."
                    4. "Terrible customer service. Will never buy again."
                    5. "Pretty good overall, though the color was slightly different from the picture."
                    
                    Format as: [number]. [sentiment] - [brief reasoning]
                    """,
                ExpectedBehavior = "Correct sentiment labels with reasonable justifications.",
                Tags = ["classification", "sentiment", "nlp"]
            },

            // Format Conversion Examples
            new DemoExample
            {
                Id = "convert-format",
                Category = "Format Conversion",
                Title = "JSON to YAML",
                Description = "Convert data between formats.",
                Prompt = """
                    Convert the following JSON to YAML format:
                    
                    {
                      "application": {
                        "name": "AgenticLab",
                        "version": "1.0.0",
                        "settings": {
                          "model": "llama3.2",
                          "temperature": 0.7,
                          "maxTokens": 1000,
                          "features": ["chat", "compare", "export"]
                        }
                      }
                    }
                    """,
                ExpectedBehavior = "Valid YAML with proper indentation and structure.",
                Tags = ["conversion", "json", "yaml"]
            },

            // Temperature Comparison
            new DemoExample
            {
                Id = "temp-creative",
                Category = "Parameter Testing",
                Title = "Temperature Effect",
                Description = "Compare responses at different temperatures to see creativity vs determinism.",
                Prompt = "Write a one-paragraph story about a robot who discovers music for the first time.",
                ExpectedBehavior = "At low temperature: factual, predictable. At high temperature: creative, varied between runs.",
                Tags = ["parameter", "temperature", "comparison"]
            }
        ];
    }

    /// <summary>
    /// Gets all demo examples.
    /// </summary>
    public IReadOnlyList<DemoExample> GetExamples() => _examples.AsReadOnly();

    /// <summary>
    /// Gets demo examples filtered by category.
    /// </summary>
    public IReadOnlyList<DemoExample> GetExamplesByCategory(string category) =>
        _examples.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList().AsReadOnly();

    /// <summary>
    /// Gets a demo example by ID.
    /// </summary>
    public DemoExample? GetExample(string id) => _examples.Find(e => e.Id == id);

    /// <summary>
    /// Gets all unique categories.
    /// </summary>
    public IReadOnlyList<string> GetCategories() =>
        _examples.Select(e => e.Category).Distinct().Order().ToList().AsReadOnly();
}

/// <summary>
/// A built-in demo example scenario.
/// </summary>
public class DemoExample
{
    public string Id { get; set; } = "";
    public string Category { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Prompt { get; set; } = "";
    public string ExpectedBehavior { get; set; } = "";
    public List<string> Tags { get; set; } = [];
}
