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
                Tags = ["basic", "knowledge"],
                SuggestedAgentType = "SimpleQuestion"
            },
            new DemoExample
            {
                Id = "qa-reasoning",
                Category = "Q&A",
                Title = "Reasoning",
                Description = "Test logical reasoning and step-by-step thinking.",
                Prompt = "A farmer has 17 sheep. All but 9 run away. How many sheep does the farmer have left? Explain your reasoning step by step.",
                ExpectedBehavior = "Should correctly answer 9 with clear logical explanation.",
                Tags = ["reasoning", "math"],
                SuggestedAgentType = "SimpleQuestion"
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
                Tags = ["summarization", "text"],
                SuggestedAgentType = "Summarizer"
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
                Tags = ["extraction", "json", "structured"],
                SuggestedAgentType = "DataExtractor"
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
                Tags = ["extraction", "table", "markdown"],
                SuggestedAgentType = "DataExtractor"
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
                Tags = ["code", "csharp"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "code-api",
                Category = "Code Generation",
                Title = "Generate REST API",
                Description = "Generate a minimal API endpoint in .NET.",
                Prompt = "Write a .NET minimal API endpoint that accepts a POST request with a JSON body containing 'text' and 'language', and returns the text translated to the specified language. Include error handling.",
                ExpectedBehavior = "A complete minimal API endpoint with proper model binding and error handling.",
                Tags = ["code", "api", "dotnet"],
                SuggestedAgentType = "CodeGenerator"
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
                Tags = ["translation", "multilingual"],
                SuggestedAgentType = "Translator"
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
                Tags = ["classification", "sentiment", "nlp"],
                SuggestedAgentType = "Classifier"
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
                Tags = ["conversion", "json", "yaml"],
                SuggestedAgentType = "FormatConverter"
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
                Tags = ["parameter", "temperature", "comparison"],
                SuggestedAgentType = "CreativeWriter"
            },

            // ── C# Programming ──
            new DemoExample
            {
                Id = "csharp-record",
                Category = "C# Programming",
                Title = "Record Types & Pattern Matching",
                Description = "Generate C# 14 code using modern language features.",
                Prompt = """
                    Write a C# record hierarchy for a payment system:
                    - A base record `Payment` with Amount (decimal) and Currency (string).
                    - Derived records: CreditCardPayment (CardNumber, ExpiryDate), BankTransfer (IBAN, BIC), CryptoPayment (WalletAddress, Network).
                    - A static method `Describe` that uses pattern matching (switch expression) to return a human-readable description for each payment type.
                    Use C# 14 features, file-scoped namespace, and XML doc comments.
                    """,
                ExpectedBehavior = "Correct C# 14 records with inheritance, switch expression pattern matching, and XML docs.",
                Tags = ["code", "csharp", "records", "pattern-matching"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "csharp-async",
                Category = "C# Programming",
                Title = "Async Pipeline with Channels",
                Description = "Generate an async producer/consumer pipeline using System.Threading.Channels.",
                Prompt = """
                    Write a C# class `DataPipeline<T>` that:
                    1. Uses a bounded Channel<T> as an internal buffer (capacity configurable via constructor).
                    2. Has an async `ProduceAsync(IAsyncEnumerable<T> source, CancellationToken ct)` method that writes items to the channel.
                    3. Has an async `ConsumeAsync(Func<T, Task> handler, CancellationToken ct)` method that reads and processes items.
                    4. Has a `RunAsync` method that starts both producer and consumer concurrently and awaits completion.
                    Include proper cancellation support and error handling. Use file-scoped namespace and XML doc comments.
                    """,
                ExpectedBehavior = "Production-quality C# async code with Channels, CancellationToken, and proper resource cleanup.",
                Tags = ["code", "csharp", "async", "channels"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "csharp-di",
                Category = "C# Programming",
                Title = "Dependency Injection Setup",
                Description = "Generate a DI-wired service layer with interfaces.",
                Prompt = """
                    Write a C# service layer for an order processing system using Microsoft.Extensions.DependencyInjection:
                    - IOrderRepository with methods: GetByIdAsync, CreateAsync, UpdateStatusAsync
                    - IInventoryService with method: ReserveStockAsync(string sku, int quantity)
                    - OrderService that depends on both interfaces and has a PlaceOrderAsync method that validates stock, creates the order, and reserves inventory.
                    - An extension method `AddOrderServices(this IServiceCollection services)` to register everything.
                    Show the interfaces, implementation, and DI registration. Use file-scoped namespaces.
                    """,
                ExpectedBehavior = "Clean interface-based design with constructor injection, async methods, and proper DI registration.",
                Tags = ["code", "csharp", "di", "architecture"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "csharp-linq",
                Category = "C# Programming",
                Title = "Advanced LINQ Queries",
                Description = "Test complex LINQ query generation from natural language.",
                Prompt = """
                    Given these C# records:
                    record Employee(int Id, string Name, string Department, decimal Salary, DateOnly HireDate);
                    record Project(int Id, string Name, string Department, decimal Budget);
                    record Assignment(int EmployeeId, int ProjectId, int HoursPerWeek);
                    
                    Write LINQ queries (method syntax) for:
                    1. Top 3 departments by average salary, with the average rounded to 2 decimals.
                    2. Employees who work on more than 2 projects and their total weekly hours.
                    3. Projects where the total salary cost of assigned employees exceeds the project budget (assume salary is annual, 52 weeks, proportional to hours/40).
                    4. A department summary: department name, headcount, total salary, project count, total budget.
                    """,
                ExpectedBehavior = "Correct LINQ method-syntax queries using GroupBy, Join, Select, Where, OrderBy with proper projections.",
                Tags = ["code", "csharp", "linq", "queries"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "csharp-test",
                Category = "C# Programming",
                Title = "Unit Tests with xUnit",
                Description = "Generate unit tests for a given method.",
                Prompt = """
                    Write xUnit tests for this C# method:
                    
                    public static class StringUtils
                    {
                        public static string Truncate(string input, int maxLength, string suffix = "...")
                        {
                            if (string.IsNullOrEmpty(input) || input.Length <= maxLength) return input;
                            if (maxLength <= suffix.Length) return suffix[..maxLength];
                            return string.Concat(input.AsSpan(0, maxLength - suffix.Length), suffix);
                        }
                    }
                    
                    Cover: null input, empty string, string shorter than max, string exactly at max, string longer than max, custom suffix, maxLength shorter than suffix, maxLength of zero.
                    Use [Theory] with [InlineData] where appropriate.
                    """,
                ExpectedBehavior = "Comprehensive xUnit tests with Theory/InlineData covering all edge cases.",
                Tags = ["code", "csharp", "testing", "xunit"],
                SuggestedAgentType = "CodeGenerator"
            },

            // ── Data Analysis ──
            new DemoExample
            {
                Id = "data-stats",
                Category = "Data Analysis",
                Title = "Descriptive Statistics",
                Description = "Compute and interpret descriptive statistics from raw data.",
                Prompt = """
                    Analyze this monthly website traffic data and provide descriptive statistics:
                    
                    Jan: 12,400 | Feb: 11,800 | Mar: 15,200 | Apr: 14,600 | May: 18,900
                    Jun: 22,100 | Jul: 19,500 | Aug: 17,800 | Sep: 16,300 | Oct: 20,400
                    Nov: 25,600 | Dec: 28,100
                    
                    Calculate: mean, median, standard deviation, min, max, range, and month-over-month growth rates.
                    Identify the trend, any seasonality, and the months with highest/lowest growth.
                    Present the results in a clear table format.
                    """,
                ExpectedBehavior = "Accurate calculations, clear table, and insightful trend analysis.",
                Tags = ["data", "statistics", "analysis"],
                SuggestedAgentType = "DataExtractor"
            },
            new DemoExample
            {
                Id = "data-sql",
                Category = "Data Analysis",
                Title = "SQL Query from Question",
                Description = "Generate SQL queries from natural language analytical questions.",
                Prompt = """
                    Given this database schema:
                    
                    - customers(id, name, email, country, created_at)
                    - orders(id, customer_id, total_amount, status, created_at)
                    - order_items(id, order_id, product_id, quantity, unit_price)
                    - products(id, name, category, price, stock_quantity)
                    
                    Write SQL queries for:
                    1. Top 10 customers by total lifetime spend, including order count and average order value.
                    2. Monthly revenue trend for the last 12 months with month-over-month percentage change.
                    3. Product categories ranked by revenue, with the percentage contribution to total revenue.
                    4. Customers who haven't ordered in the last 90 days but had at least 3 orders before that (churn candidates).
                    
                    Use standard SQL (compatible with PostgreSQL). Include comments explaining each query.
                    """,
                ExpectedBehavior = "Correct SQL with proper JOINs, aggregations, window functions, and CTEs where appropriate.",
                Tags = ["data", "sql", "queries", "analytics"],
                SuggestedAgentType = "CodeGenerator"
            },
            new DemoExample
            {
                Id = "data-clean",
                Category = "Data Analysis",
                Title = "Data Cleaning Rules",
                Description = "Identify data quality issues and recommend cleaning steps.",
                Prompt = """
                    Review this sample dataset and identify all data quality issues. Recommend cleaning steps for each:
                    
                    | Name          | Email                | Phone        | Country | Revenue  | Date       |
                    |---------------|----------------------|--------------|---------|----------|------------|
                    | John Smith    | john@example.com     | 555-0123     | US      | $1,200   | 2025-01-15 |
                    | JANE DOE      | jane@@example.com    | +1-555-0456  | USA     | 1500.00  | 01/15/2025 |
                    |               | bob@test.com         | N/A          | us      | -500     | 2025-13-01 |
                    | Alice Johnson | alice@example.com    | 555.0789     | U.S.A.  | $2,300   | 2025-01-15 |
                    | John Smith    | john@example.com     | 555-0123     | US      | $1,200   | 2025-01-15 |
                    | Test User     | test@test.test       |              | XX      | 0        | TBD        |
                    
                    For each issue found, specify: the problem, affected rows, severity (high/medium/low), and the recommended fix.
                    """,
                ExpectedBehavior = "Comprehensive identification of duplicates, format inconsistencies, invalid values, missing data, and outliers.",
                Tags = ["data", "cleaning", "quality"],
                SuggestedAgentType = "DataExtractor"
            },
            new DemoExample
            {
                Id = "data-kpi",
                Category = "Data Analysis",
                Title = "KPI Dashboard Design",
                Description = "Design KPIs and metrics for a business scenario.",
                Prompt = """
                    You are a data analyst for an e-commerce company. Design a KPI dashboard with the following:
                    
                    1. Define 8-10 key metrics across these categories: Revenue, Customer, Operations, Product.
                    2. For each metric provide: name, formula/calculation, target value, update frequency (daily/weekly/monthly).
                    3. Suggest how to visualize each metric (line chart, bar chart, gauge, number card, etc.).
                    4. Identify 3 metrics that should trigger alerts when they cross certain thresholds.
                    
                    Present as a structured table.
                    """,
                ExpectedBehavior = "Well-structured KPI definitions with realistic formulas, targets, and visualization recommendations.",
                Tags = ["data", "kpi", "dashboard", "business"],
                SuggestedAgentType = "DataExtractor"
            },
            new DemoExample
            {
                Id = "data-pivot",
                Category = "Data Analysis",
                Title = "Pivot & Aggregate",
                Description = "Transform flat data into a pivot summary.",
                Prompt = """
                    Given this sales data, create a pivot table summary:
                    
                    Region: North, Product: Widget A, Q1: 120, Q2: 150, Q3: 130, Q4: 180
                    Region: North, Product: Widget B, Q1: 90, Q2: 110, Q3: 95, Q4: 140
                    Region: South, Product: Widget A, Q1: 200, Q2: 180, Q3: 220, Q4: 250
                    Region: South, Product: Widget B, Q1: 60, Q2: 75, Q3: 80, Q4: 95
                    Region: East, Product: Widget A, Q1: 150, Q2: 165, Q3: 170, Q4: 190
                    Region: East, Product: Widget B, Q1: 110, Q2: 120, Q3: 115, Q4: 130
                    
                    Create:
                    1. A pivot by Region showing total annual sales and average quarterly sales.
                    2. A pivot by Product showing each region's contribution percentage.
                    3. Identify the best and worst performing region-product combination.
                    """,
                ExpectedBehavior = "Correct pivot calculations with totals, percentages, and clear analysis.",
                Tags = ["data", "pivot", "aggregation"],
                SuggestedAgentType = "DataExtractor"
            },

            // ── CRM ──
            new DemoExample
            {
                Id = "crm-lead",
                Category = "CRM",
                Title = "Lead Qualification",
                Description = "Score and qualify a sales lead based on provided information.",
                Prompt = """
                    Score and qualify this sales lead using BANT criteria (Budget, Authority, Need, Timeline):
                    
                    Company: Contoso Manufacturing
                    Contact: Sarah Chen, VP of Operations
                    Company size: 500 employees, $80M annual revenue
                    Current situation: Using spreadsheets and email to manage customer orders. 
                    Experiencing 15% order error rate and 3-day average processing time.
                    Budget mention: "We've allocated around $200K for digital transformation this fiscal year."
                    Timeline: "We need something in place before Q3 when our peak season starts."
                    Competition: Also evaluating SAP and a smaller local vendor.
                    
                    Provide: BANT score (1-10 each), overall qualification (Hot/Warm/Cold), recommended next steps, 
                    key talking points, and potential objections to prepare for.
                    """,
                ExpectedBehavior = "Structured BANT analysis with realistic scores, actionable next steps, and sales-ready talking points.",
                Tags = ["crm", "sales", "lead-scoring", "bant"],
                SuggestedAgentType = "Classifier"
            },
            new DemoExample
            {
                Id = "crm-email",
                Category = "CRM",
                Title = "Sales Follow-up Email",
                Description = "Generate a contextual follow-up email after a sales meeting.",
                Prompt = """
                    Write a follow-up email after a product demo meeting. Context:
                    
                    - Prospect: Maria Lopez, Head of IT at Fabrikam Inc. (200 employees, logistics sector)
                    - Demo was for our CRM platform, focusing on route optimization and delivery tracking
                    - She was impressed by the mobile app but concerned about integration with their existing SAP ERP
                    - Her team needs training — she asked about onboarding time
                    - Decision timeline: wants to present to the board in 3 weeks
                    - Competitor mentioned: they're also looking at Salesforce
                    
                    Write a professional, concise follow-up email that addresses her concerns, 
                    differentiates from the competitor, and proposes clear next steps. Keep it under 250 words.
                    """,
                ExpectedBehavior = "Professional email addressing SAP integration concerns, training timeline, and a clear call to action.",
                Tags = ["crm", "sales", "email", "communication"],
                SuggestedAgentType = "CreativeWriter"
            },
            new DemoExample
            {
                Id = "crm-segment",
                Category = "CRM",
                Title = "Customer Segmentation",
                Description = "Segment customers based on CRM data for targeted campaigns.",
                Prompt = """
                    Segment these customers into groups and recommend a campaign strategy for each:
                    
                    | Customer      | Last Purchase | Total Orders | Lifetime Value | Avg Order | Support Tickets |
                    |---------------|---------------|--------------|----------------|-----------|-----------------|
                    | Acme Corp     | 2 weeks ago   | 45           | $128,000       | $2,844    | 3               |
                    | Beta LLC      | 8 months ago  | 12           | $34,000        | $2,833    | 8               |
                    | Gamma Inc     | 1 week ago    | 3            | $4,500         | $1,500    | 0               |
                    | Delta SA      | 3 months ago  | 28           | $82,000        | $2,929    | 1               |
                    | Epsilon GmbH  | 14 months ago | 6            | $9,200         | $1,533    | 12              |
                    | Zeta Corp     | 1 month ago   | 52           | $195,000       | $3,750    | 2               |
                    | Eta Ltd       | 5 months ago  | 8            | $11,500        | $1,438    | 5               |
                    | Theta AG      | 3 days ago    | 1            | $2,200         | $2,200    | 0               |
                    
                    For each segment: name the segment, list the customers, explain the criteria, 
                    and recommend a specific campaign type (retention, upsell, win-back, onboarding, etc.).
                    """,
                ExpectedBehavior = "Clear RFM-style segmentation with actionable campaign recommendations per segment.",
                Tags = ["crm", "segmentation", "marketing", "rfm"],
                SuggestedAgentType = "Classifier"
            },
            new DemoExample
            {
                Id = "crm-pipeline",
                Category = "CRM",
                Title = "Pipeline Analysis",
                Description = "Analyze a sales pipeline and recommend actions.",
                Prompt = """
                    Analyze this sales pipeline and provide recommendations:
                    
                    | Deal              | Stage        | Value    | Age (days) | Next Step Scheduled | Win Probability |
                    |-------------------|------------- |----------|------------|---------------------|-----------------|
                    | Contoso ERP       | Negotiation  | $450K    | 95         | No                  | 60%             |
                    | Fabrikam CRM      | Demo         | $180K    | 22         | Yes (tomorrow)      | 40%             |
                    | Northwind Migrate | Proposal     | $320K    | 68         | No                  | 50%             |
                    | AdventureWorks    | Prospecting  | $75K     | 5          | Yes (next week)     | 15%             |
                    | WideWorld Import  | Closed-Won   | $210K    | 45         | —                   | 100%            |
                    | Tailspin Toys     | Negotiation  | $95K     | 120        | No                  | 30%             |
                    | Woodgrove Bank    | Qualification| $550K    | 35         | Yes (Friday)        | 25%             |
                    
                    Provide:
                    1. Weighted pipeline value and expected close revenue.
                    2. Deals at risk (stalled, no next steps, aging) with specific actions.
                    3. Pipeline health assessment (stage distribution, velocity).
                    4. Top 3 priority actions for this week.
                    """,
                ExpectedBehavior = "Accurate weighted calculations, identification of at-risk deals, and actionable weekly priorities.",
                Tags = ["crm", "pipeline", "sales", "forecasting"],
                SuggestedAgentType = "DataExtractor"
            },
            new DemoExample
            {
                Id = "crm-mapping",
                Category = "CRM",
                Title = "CRM-to-ERP Field Mapping",
                Description = "Map CRM customer fields to ERP system fields for integration.",
                Prompt = """
                    Create a field mapping document for syncing customer data from a CRM (Dynamics 365) to an ERP (SAP Business One):
                    
                    CRM fields: AccountName, AccountNumber, PrimaryContact.FullName, PrimaryContact.Email, 
                    PrimaryContact.Phone, BillingAddress (Street, City, State, PostalCode, Country), 
                    AnnualRevenue, Industry, PaymentTerms, CreditLimit, TaxId, PreferredCurrency
                    
                    Map each CRM field to the equivalent SAP B1 Business Partner field. For each mapping specify:
                    - Source field (CRM) → Target field (SAP B1)
                    - Data type transformation needed (if any)
                    - Validation rules
                    - Default value if source is empty
                    - Special notes (e.g., lookup tables, format conversion)
                    
                    Present as a detailed mapping table.
                    """,
                ExpectedBehavior = "Complete field mapping with realistic SAP B1 field names, proper type transformations, and validation rules.",
                Tags = ["crm", "erp", "integration", "mapping"],
                SuggestedAgentType = "DataExtractor"
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

    /// <summary>
    /// The agent type best suited for this example (matches AgentConfig.AgentType).
    /// </summary>
    public string SuggestedAgentType { get; set; } = "";
}
