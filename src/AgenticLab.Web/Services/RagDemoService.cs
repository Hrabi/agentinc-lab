namespace AgenticLab.Web.Services;

/// <summary>
/// Provides pre-built demo data and simulated RAG pipeline results for the RAG presentation page.
/// Uses the demo files from data/rag-demo/ to demonstrate how chunking, embedding, and retrieval work.
/// </summary>
public class RagDemoService
{
    private readonly List<DemoDocument> _documents;
    private readonly List<DemoChunk> _chunks;
    private readonly List<RagDemoScenario> _scenarios;

    public RagDemoService()
    {
        _documents = BuildDocuments();
        _chunks = BuildChunks();
        _scenarios = BuildScenarios();
    }

    /// <summary>
    /// Gets all ingested demo documents.
    /// </summary>
    public IReadOnlyList<DemoDocument> GetDocuments() => _documents.AsReadOnly();

    /// <summary>
    /// Gets all chunks across all documents.
    /// </summary>
    public IReadOnlyList<DemoChunk> GetChunks() => _chunks.AsReadOnly();

    /// <summary>
    /// Gets chunks for a specific document.
    /// </summary>
    public IReadOnlyList<DemoChunk> GetChunksByDocument(string documentId) =>
        _chunks.Where(c => c.DocumentId == documentId).ToList().AsReadOnly();

    /// <summary>
    /// Gets all pre-built RAG demo scenarios with questions, retrieved chunks, and expected answers.
    /// </summary>
    public IReadOnlyList<RagDemoScenario> GetScenarios() => _scenarios.AsReadOnly();

    /// <summary>
    /// Gets a scenario by ID.
    /// </summary>
    public RagDemoScenario? GetScenario(string id) => _scenarios.Find(s => s.Id == id);

    /// <summary>
    /// Simulates a vector search by returning pre-matched chunks for a scenario.
    /// </summary>
    public List<RetrievedChunk> SimulateSearch(string scenarioId)
    {
        var scenario = _scenarios.Find(s => s.Id == scenarioId);
        return scenario?.RetrievedChunks ?? [];
    }

    /// <summary>
    /// Builds the augmented prompt that would be sent to the LLM in a real RAG pipeline.
    /// </summary>
    public string BuildAugmentedPrompt(string question, List<RetrievedChunk> retrievedChunks)
    {
        var context = string.Join("\n\n", retrievedChunks.Select((c, i) =>
            $"[Source {i + 1}: {c.SourceFile}, Chunk {c.ChunkIndex}]\n{c.Text}"));

        return $"""
            Answer the question based ONLY on the provided context below.
            If the answer is not in the context, say "I don't have enough information to answer that."
            Cite the source file for each claim you make.

            CONTEXT:
            {context}

            QUESTION: {question}
            """;
    }

    private static List<DemoDocument> BuildDocuments() =>
    [
        new DemoDocument
        {
            Id = "doc-handbook",
            FileName = "company-handbook.md",
            FileType = "Markdown",
            Description = "Employee & policy handbook — leave, expenses, security, returns",
            WordCount = 3500,
            ChunkCount = 14,
            Icon = "📋"
        },
        new DemoDocument
        {
            Id = "doc-catalog",
            FileName = "product-catalog.json",
            FileType = "JSON",
            Description = "Full product catalog — 8 products with specs, pricing, features",
            WordCount = 2200,
            ChunkCount = 9,
            Icon = "📦"
        },
        new DemoDocument
        {
            Id = "doc-order-model",
            FileName = "OrderRepository.cs",
            FileType = "C#",
            Description = "Order domain model — Order, OrderLine, Address, OrderStatus",
            WordCount = 400,
            ChunkCount = 4,
            Icon = "💻"
        },
        new DemoDocument
        {
            Id = "doc-order-service",
            FileName = "OrderService.cs",
            FileType = "C#",
            Description = "Order repository — validation, CRUD, returns, statistics",
            WordCount = 600,
            ChunkCount = 6,
            Icon = "💻"
        },
        new DemoDocument
        {
            Id = "doc-api",
            FileName = "smarthub-api-reference.md",
            FileType = "Markdown",
            Description = "SmartHub 400 REST + WebSocket API reference",
            WordCount = 2000,
            ChunkCount = 8,
            Icon = "🔌"
        },
        new DemoDocument
        {
            Id = "doc-report",
            FileName = "quarterly-report-q4-2025.md",
            FileType = "Markdown",
            Description = "Q4 2025 financial report — revenue, metrics, outlook",
            WordCount = 1500,
            ChunkCount = 7,
            Icon = "📊"
        },
        new DemoDocument
        {
            Id = "doc-faq",
            FileName = "faq.md",
            FileType = "Markdown",
            Description = "Customer FAQ — ordering, returns, products, warranty, tech",
            WordCount = 1800,
            ChunkCount = 8,
            Icon = "❓"
        }
    ];

    private static List<DemoChunk> BuildChunks() =>
    [
        // handbook chunks
        new DemoChunk { Id = "hb-1", DocumentId = "doc-handbook", ChunkIndex = 0, Preview = "Company Overview — Contoso Electronics is a global provider of smart home devices...", TokenCount = 180 },
        new DemoChunk { Id = "hb-2", DocumentId = "doc-handbook", ChunkIndex = 1, Preview = "Working Hours & Flexibility — Standard work week is 40 hours, Monday through Friday...", TokenCount = 310 },
        new DemoChunk { Id = "hb-3", DocumentId = "doc-handbook", ChunkIndex = 2, Preview = "Annual Leave — Standard: 25 working days per year. Senior staff (5+ years): 28 days...", TokenCount = 280 },
        new DemoChunk { Id = "hb-4", DocumentId = "doc-handbook", ChunkIndex = 3, Preview = "Sick Leave — Full pay for first 10 sick days. Days 11-30 at 70% of base salary...", TokenCount = 190 },
        new DemoChunk { Id = "hb-5", DocumentId = "doc-handbook", ChunkIndex = 4, Preview = "Parental Leave — Primary caregiver: 16 weeks at full pay + up to 10 additional weeks...", TokenCount = 220 },
        new DemoChunk { Id = "hb-6", DocumentId = "doc-handbook", ChunkIndex = 5, Preview = "Customer Returns — Products may be returned within 30 calendar days with proof of purchase...", TokenCount = 350 },
        new DemoChunk { Id = "hb-7", DocumentId = "doc-handbook", ChunkIndex = 6, Preview = "Warranty — 2-year manufacturer warranty for consumer electronics. 3-year for industrial IoT...", TokenCount = 290 },
        new DemoChunk { Id = "hb-8", DocumentId = "doc-handbook", ChunkIndex = 7, Preview = "ContosoGuard — Extended warranty for 1-3 additional years. Covers accidental damage...", TokenCount = 180 },
        new DemoChunk { Id = "hb-9", DocumentId = "doc-handbook", ChunkIndex = 8, Preview = "Travel Expenses — Domestic flights: Economy. International 6+ hours: Business class...", TokenCount = 340 },
        new DemoChunk { Id = "hb-10", DocumentId = "doc-handbook", ChunkIndex = 9, Preview = "Reimbursement Process — Submit via Contoso Expense Portal within 30 days...", TokenCount = 210 },
        new DemoChunk { Id = "hb-11", DocumentId = "doc-handbook", ChunkIndex = 10, Preview = "Information Security — Data classification: Public, Internal, Confidential, Restricted...", TokenCount = 310 },
        new DemoChunk { Id = "hb-12", DocumentId = "doc-handbook", ChunkIndex = 11, Preview = "Password Policy — Minimum 14 characters, MFA mandatory for all employees...", TokenCount = 250 },
        new DemoChunk { Id = "hb-13", DocumentId = "doc-handbook", ChunkIndex = 12, Preview = "Performance Reviews — Twice per year (June, December). 360-degree feedback model...", TokenCount = 270 },
        new DemoChunk { Id = "hb-14", DocumentId = "doc-handbook", ChunkIndex = 13, Preview = "Code of Conduct — Treat all colleagues with respect. No discrimination. Report conflicts...", TokenCount = 240 },

        // catalog chunks
        new DemoChunk { Id = "cat-1", DocumentId = "doc-catalog", ChunkIndex = 0, Preview = "SmartHub 400 — Central smart home controller, Matter/Zigbee/Z-Wave/Thread/Wi-Fi 6E, €249.99...", TokenCount = 380 },
        new DemoChunk { Id = "cat-2", DocumentId = "doc-catalog", ChunkIndex = 1, Preview = "TempSense Pro — Temperature/humidity sensor, e-paper display, 18-month battery, €39.99...", TokenCount = 290 },
        new DemoChunk { Id = "cat-3", DocumentId = "doc-catalog", ChunkIndex = 2, Preview = "SecureView 360 — 4K security camera, IP67, on-device AI detection, €179.99...", TokenCount = 320 },
        new DemoChunk { Id = "cat-4", DocumentId = "doc-catalog", ChunkIndex = 3, Preview = "SmartLock BT2 — Fingerprint + PIN + NFC smart deadbolt, Matter, €219.99, out of stock...", TokenCount = 310 },
        new DemoChunk { Id = "cat-5", DocumentId = "doc-catalog", ChunkIndex = 4, Preview = "VibraSense X1 — Industrial vibration sensor, triaxial MEMS, LoRaWAN, ATEX Zone 2, €449.99...", TokenCount = 350 },
        new DemoChunk { Id = "cat-6", DocumentId = "doc-catalog", ChunkIndex = 5, Preview = "FlowMeter M3 — Non-invasive ultrasonic flow meter, ±1% accuracy, MQTT/Modbus, €899.99...", TokenCount = 310 },
        new DemoChunk { Id = "cat-7", DocumentId = "doc-catalog", ChunkIndex = 6, Preview = "SoundBuds NC3 — True wireless earbuds, -42dB ANC, Bluetooth 5.4, IPX5, €149.99...", TokenCount = 330 },
        new DemoChunk { Id = "cat-8", DocumentId = "doc-catalog", ChunkIndex = 7, Preview = "SoundPillar S5 — Floor-standing wireless speaker, 360° sound, 120W, €599.99...", TokenCount = 300 },
        new DemoChunk { Id = "cat-9", DocumentId = "doc-catalog", ChunkIndex = 8, Preview = "Shipping — Standard €5.99 (3-5 days), free over €75. Express €14.99 (1-2 days)...", TokenCount = 200 },

        // order model chunks
        new DemoChunk { Id = "ord-1", DocumentId = "doc-order-model", ChunkIndex = 0, Preview = "class Order — OrderId, CustomerId, OrderDate, Status, Lines, ShippingAddress...", TokenCount = 280 },
        new DemoChunk { Id = "ord-2", DocumentId = "doc-order-model", ChunkIndex = 1, Preview = "class OrderLine — Sku, ProductName, Quantity [1-100], UnitPrice, LineTotal...", TokenCount = 180 },
        new DemoChunk { Id = "ord-3", DocumentId = "doc-order-model", ChunkIndex = 2, Preview = "class Address — Street, City, PostalCode, Country, State (optional)...", TokenCount = 120 },
        new DemoChunk { Id = "ord-4", DocumentId = "doc-order-model", ChunkIndex = 3, Preview = "enum OrderStatus — Pending, Confirmed, Shipped, Delivered, Cancelled, ReturnRequested, Refunded", TokenCount = 150 },

        // order service chunks
        new DemoChunk { Id = "svc-1", DocumentId = "doc-order-service", ChunkIndex = 0, Preview = "CreateOrder — Validates customerId, line items (qty 1-100, positive price), address, discount code...", TokenCount = 380 },
        new DemoChunk { Id = "svc-2", DocumentId = "doc-order-service", ChunkIndex = 1, Preview = "GetOrder, GetOrdersByCustomer — Retrieves by ID or customer, ordered by date descending...", TokenCount = 180 },
        new DemoChunk { Id = "svc-3", DocumentId = "doc-order-service", ChunkIndex = 2, Preview = "CancelOrder — Only Pending or Confirmed orders can be cancelled...", TokenCount = 200 },
        new DemoChunk { Id = "svc-4", DocumentId = "doc-order-service", ChunkIndex = 3, Preview = "RequestReturn — Only Delivered orders within 30-day window. Checks days since order...", TokenCount = 220 },
        new DemoChunk { Id = "svc-5", DocumentId = "doc-order-service", ChunkIndex = 4, Preview = "GetStatistics — TotalOrders, TotalRevenue, AverageOrderValue, OrdersByStatus, TopProducts...", TokenCount = 250 },
        new DemoChunk { Id = "svc-6", DocumentId = "doc-order-service", ChunkIndex = 5, Preview = "ValidateAddress — Street, City, PostalCode, Country required. IsValidDiscountCode regex...", TokenCount = 170 },

        // api chunks
        new DemoChunk { Id = "api-1", DocumentId = "doc-api", ChunkIndex = 0, Preview = "SmartHub API Overview — REST API on port 8443 (HTTPS). Bearer token authentication...", TokenCount = 250 },
        new DemoChunk { Id = "api-2", DocumentId = "doc-api", ChunkIndex = 1, Preview = "GET /devices — Returns all paired devices with id, name, type, protocol, online status...", TokenCount = 320 },
        new DemoChunk { Id = "api-3", DocumentId = "doc-api", ChunkIndex = 2, Preview = "POST /devices/{id}/command — Send commands: set, toggle, identify, lock, unlock...", TokenCount = 350 },
        new DemoChunk { Id = "api-4", DocumentId = "doc-api", ChunkIndex = 3, Preview = "GET /automations — Returns all rules. Triggers: sun, device, time. Conditions + actions...", TokenCount = 380 },
        new DemoChunk { Id = "api-5", DocumentId = "doc-api", ChunkIndex = 4, Preview = "GET /scenes — Saved device states. POST /scenes/{id}/activate to apply...", TokenCount = 220 },
        new DemoChunk { Id = "api-6", DocumentId = "doc-api", ChunkIndex = 5, Preview = "Rate Limits — 100 req/sec local. WebSocket at wss://<ip>:8443/ws/events...", TokenCount = 250 },
        new DemoChunk { Id = "api-7", DocumentId = "doc-api", ChunkIndex = 6, Preview = "Error Codes — 400 invalid_request, 401 unauthorized, 404 device_not_found, 409 device_offline...", TokenCount = 200 },
        new DemoChunk { Id = "api-8", DocumentId = "doc-api", ChunkIndex = 7, Preview = "WebSocket Events — device_state_changed with old/new values, timestamp...", TokenCount = 180 },

        // report chunks
        new DemoChunk { Id = "rpt-1", DocumentId = "doc-report", ChunkIndex = 0, Preview = "Executive Summary — Q4 2025 revenue €47.3M, +12% YoY. SmartHub 400 launch drove growth...", TokenCount = 280 },
        new DemoChunk { Id = "rpt-2", DocumentId = "doc-report", ChunkIndex = 1, Preview = "Smart Home Division — €22.1M (+20.8% YoY). SmartHub 400: 38,000 units sold, exceeded forecast...", TokenCount = 350 },
        new DemoChunk { Id = "rpt-3", DocumentId = "doc-report", ChunkIndex = 2, Preview = "Industrial IoT — €14.7M (+5.8%). VibraSense X1 contract with Bosch (€2.1M annual)...", TokenCount = 280 },
        new DemoChunk { Id = "rpt-4", DocumentId = "doc-report", ChunkIndex = 3, Preview = "Consumer Audio — €10.5M (+6.1%). SoundBuds NC3: 62,000 units. SoundPillar S5: 68% margin...", TokenCount = 290 },
        new DemoChunk { Id = "rpt-5", DocumentId = "doc-report", ChunkIndex = 4, Preview = "Key Metrics — Gross Margin 52.3%, Operating Margin 14.7%, NPS 72, Return Rate 3.1%...", TokenCount = 250 },
        new DemoChunk { Id = "rpt-6", DocumentId = "doc-report", ChunkIndex = 5, Preview = "Regional Performance — Western Europe 45%, North America 28% (+18% YoY fastest growing)...", TokenCount = 240 },
        new DemoChunk { Id = "rpt-7", DocumentId = "doc-report", ChunkIndex = 6, Preview = "Q1 2026 Outlook — SmartLock BT2 launch March, 8+ enterprise IoT trials, revenue €44-46M...", TokenCount = 310 },

        // faq chunks
        new DemoChunk { Id = "faq-1", DocumentId = "doc-faq", ChunkIndex = 0, Preview = "Shipping — Standard 3-5 days (€5.99, free >€75). Express 1-2 days (€14.99)...", TokenCount = 250 },
        new DemoChunk { Id = "faq-2", DocumentId = "doc-faq", ChunkIndex = 1, Preview = "Order Changes — Pending orders can be modified. Confirmed can be cancelled. Shipped → return...", TokenCount = 220 },
        new DemoChunk { Id = "faq-3", DocumentId = "doc-faq", ChunkIndex = 2, Preview = "Return Policy — 30 calendar days, original packaging, 5-7 day refund. Software non-refundable...", TokenCount = 280 },
        new DemoChunk { Id = "faq-4", DocumentId = "doc-faq", ChunkIndex = 3, Preview = "SmartHub 400 Compatibility — Matter 1.3, Zigbee 3.0, Z-Wave, Thread, Wi-Fi 6E. Local-first...", TokenCount = 270 },
        new DemoChunk { Id = "faq-5", DocumentId = "doc-faq", ChunkIndex = 4, Preview = "TempSense Pro battery 18 months (CR2477). SecureView 360 IP67 waterproof. SmartLock restock March...", TokenCount = 230 },
        new DemoChunk { Id = "faq-6", DocumentId = "doc-faq", ChunkIndex = 5, Preview = "Warranty — Consumer 2-year, Industrial 3-year. ContosoGuard extended, €50 deductible per claim...", TokenCount = 260 },
        new DemoChunk { Id = "faq-7", DocumentId = "doc-faq", ChunkIndex = 6, Preview = "Support — Email support@contoso-electronics.example.com, Phone +31 20 555 0199, Mon-Fri 08-20 CET...", TokenCount = 200 },
        new DemoChunk { Id = "faq-8", DocumentId = "doc-faq", ChunkIndex = 7, Preview = "Technical — SmartHub API 100 req/sec. SecureView RTSP rtsp://<ip>:554/live. VibraSense ATEX Zone 2...", TokenCount = 240 }
    ];

    private static List<RagDemoScenario> BuildScenarios() =>
    [
        // Cross-document scenarios
        new RagDemoScenario
        {
            Id = "rag-1",
            Category = "Cross-Document",
            Title = "Return Policy (Handbook + FAQ)",
            Question = "What is the return policy and how do I initiate a return?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "company-handbook.md", ChunkIndex = 5, Score = 0.92,
                    Text = "Customer Returns — Products may be returned within 30 calendar days of delivery with proof of purchase. Items must be in original packaging and in unused or like-new condition. Refunds are processed within 5–7 business days to the original payment method. Opened software/firmware licenses are non-refundable. Shipping costs for returns are covered by Contoso only if the item is defective." },
                new RetrievedChunk { SourceFile = "faq.md", ChunkIndex = 2, Score = 0.88,
                    Text = "Return Policy — Products can be returned within 30 calendar days of delivery. Items must be in original packaging and unused or like-new condition. How to initiate: 1. Log in at contoso-electronics.example.com/returns 2. Select the order and items to return 3. Print the prepaid return label (for defective items) or pay €5.99 return shipping 4. Drop off the package at any DHL pickup point 5. Refund is processed within 5-7 business days of receiving the item." },
                new RetrievedChunk { SourceFile = "company-handbook.md", ChunkIndex = 6, Score = 0.71,
                    Text = "Warranty — All consumer electronics carry a 2-year manufacturer warranty from date of purchase. Industrial IoT sensors carry a 3-year warranty. Warranty covers defects in materials and workmanship but excludes damage from misuse, unauthorized modifications, or natural disasters." }
            ],
            ExpectedAnswer = "Products can be returned within 30 calendar days of delivery with proof of purchase, in original packaging and unused/like-new condition (company-handbook.md). To initiate: log in at the returns portal, select items, print the label, and drop off at DHL. Refund processes in 5-7 business days (faq.md). Note: software licenses are non-refundable, and return shipping is free only for defective items.",
            GroundedInDocuments = ["company-handbook.md", "faq.md"]
        },
        new RagDemoScenario
        {
            Id = "rag-2",
            Category = "Cross-Document",
            Title = "SmartHub 400 Overview (Catalog + FAQ + Report)",
            Question = "How much does the SmartHub 400 cost and what protocols does it support? How did it perform in Q4?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "product-catalog.json", ChunkIndex = 0, Score = 0.94,
                    Text = "SmartHub 400 — Central smart home controller with Matter 1.3, Zigbee 3.0, Z-Wave, Thread, and Wi-Fi 6E support. Controls up to 200 devices. 5-inch touchscreen. Price: €249.99. Rating: 4.7★ (1,243 reviews). Features: Matter 1.3 certified, offline voice control, Thread border router, OTA firmware updates, local processing." },
                new RetrievedChunk { SourceFile = "faq.md", ChunkIndex = 3, Score = 0.86,
                    Text = "SmartHub 400 — Supports Matter 1.3, Zigbee 3.0, Z-Wave, Thread, and Wi-Fi 6E. Compatible with Apple HomeKit, Google Home, Amazon Alexa, and Samsung SmartThings. Designed for local-first operation — all automations and voice control work without internet." },
                new RetrievedChunk { SourceFile = "quarterly-report-q4-2025.md", ChunkIndex = 1, Score = 0.82,
                    Text = "Smart Home Division — SmartHub 400, launched October 1, sold 38,000 units in first quarter — exceeding the forecast of 30,000 by 27%. Matter protocol support attracted customers from competitor ecosystems. Bundle promotions drove 42% attach rate for accessories. Strong reviews (4.7★) generated organic demand." }
            ],
            ExpectedAnswer = "The SmartHub 400 costs €249.99 and supports Matter 1.3, Zigbee 3.0, Z-Wave, Thread, and Wi-Fi 6E (product-catalog.json). It's compatible with HomeKit, Google Home, Alexa, and SmartThings, and works fully offline (faq.md). In Q4 2025, it sold 38,000 units — 27% above forecast — driven by Matter support and strong 4.7★ reviews (quarterly-report-q4-2025.md).",
            GroundedInDocuments = ["product-catalog.json", "faq.md", "quarterly-report-q4-2025.md"]
        },
        new RagDemoScenario
        {
            Id = "rag-3",
            Category = "Cross-Document",
            Title = "Industrial IoT Warranty (Handbook + Catalog + FAQ)",
            Question = "What warranty comes with industrial IoT sensors?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "company-handbook.md", ChunkIndex = 6, Score = 0.91,
                    Text = "Warranty — All consumer electronics carry a 2-year manufacturer warranty. Industrial IoT sensors carry a 3-year warranty. Warranty covers defects in materials and workmanship but excludes damage from misuse, unauthorized modifications, or natural disasters. Warranty claims require the original invoice and product serial number." },
                new RetrievedChunk { SourceFile = "product-catalog.json", ChunkIndex = 4, Score = 0.84,
                    Text = "VibraSense X1 — Industrial vibration sensor. Warranty: 3 years. Certifications: CE, FCC, ATEX Zone 2. Operating temp: -40°C to +85°C. 5-year battery life." },
                new RetrievedChunk { SourceFile = "faq.md", ChunkIndex = 5, Score = 0.80,
                    Text = "Warranty — Consumer electronics: 2-year manufacturer warranty. Industrial IoT sensors: 3-year manufacturer warranty. Warranty covers defects in materials and workmanship but excludes damage from misuse. ContosoGuard extended warranty available." }
            ],
            ExpectedAnswer = "Industrial IoT sensors carry a 3-year manufacturer warranty covering defects in materials and workmanship (company-handbook.md, faq.md). Claims require the original invoice and serial number. ContosoGuard extended warranty can add 1-3 more years. Warranty excludes damage from misuse or unauthorized modifications.",
            GroundedInDocuments = ["company-handbook.md", "product-catalog.json", "faq.md"]
        },

        // Single-document scenarios
        new RagDemoScenario
        {
            Id = "rag-4",
            Category = "Single Document",
            Title = "Overtime Pay (Handbook)",
            Question = "What are the overtime pay rates?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "company-handbook.md", ChunkIndex = 1, Score = 0.95,
                    Text = "Working Hours — Standard work week is 40 hours, Monday through Friday. Core hours: 10:00–15:00 CET. Overtime above 40 hours requires prior written approval from the department head. Overtime is compensated at 1.5× the hourly rate for weekdays and 2× the hourly rate for weekends and public holidays." }
            ],
            ExpectedAnswer = "Overtime above 40 hours/week requires department head approval. Rates: 1.5× hourly rate on weekdays, 2× on weekends and public holidays (company-handbook.md).",
            GroundedInDocuments = ["company-handbook.md"]
        },
        new RagDemoScenario
        {
            Id = "rag-5",
            Category = "Single Document",
            Title = "SmartHub API Commands (API Reference)",
            Question = "How do I send a command to a device via the SmartHub API?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "smarthub-api-reference.md", ChunkIndex = 2, Score = 0.96,
                    Text = """POST /devices/{id}/command — Send a command to a device. Request body: { "command": "set", "properties": { "on": true, "brightness": 50 } }. Supported commands: set (set properties), toggle (on/off), identify (blink 10s), lock, unlock. Response 200: { "status": "ok", "device_id": "...", "applied": { ... } }.""" },
                new RetrievedChunk { SourceFile = "smarthub-api-reference.md", ChunkIndex = 0, Score = 0.78,
                    Text = "SmartHub API — REST API on port 8443 (HTTPS). Bearer token authentication required. Tokens generated in Settings → Developer → API Keys. Base URL: https://<smarthub-ip>:8443/api/v2." }
            ],
            ExpectedAnswer = "POST to /devices/{id}/command with a JSON body containing 'command' (set, toggle, identify, lock, unlock) and 'properties'. Authentication: Bearer token in Authorization header. Base URL: https://<smarthub-ip>:8443/api/v2 (smarthub-api-reference.md).",
            GroundedInDocuments = ["smarthub-api-reference.md"]
        },
        new RagDemoScenario
        {
            Id = "rag-6",
            Category = "Single Document",
            Title = "Order Validation (C# Code)",
            Question = "What validation does the OrderRepository perform when creating an order?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "OrderService.cs", ChunkIndex = 0, Score = 0.94,
                    Text = "CreateOrder — Validates: (1) Customer ID is required (not null/empty), (2) At least one line item required, (3) Each line: quantity 1-100, unit price must be positive, (4) Shipping address: street, city, postal code, country required, (5) Discount code format: CONTOSO-XXXX-XXXX (regex validated). On success, generates OrderId like ORD-2026-00001." },
                new RetrievedChunk { SourceFile = "OrderService.cs", ChunkIndex = 3, Score = 0.76,
                    Text = "RequestReturn — Only Delivered orders can be returned. Checks 30-day return window by computing days since OrderDate. Throws InvalidOperationException if expired or wrong status." }
            ],
            ExpectedAnswer = "CreateOrder validates: (1) non-empty customer ID, (2) at least one line item, (3) quantity 1-100 and positive unit price per line, (4) address with street/city/postal/country required, (5) discount code matches CONTOSO-XXXX-XXXX regex format (OrderService.cs).",
            GroundedInDocuments = ["OrderService.cs"]
        },
        new RagDemoScenario
        {
            Id = "rag-7",
            Category = "Single Document",
            Title = "Q1 2026 Revenue Guidance (Report)",
            Question = "What is the Q1 2026 revenue guidance?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "quarterly-report-q4-2025.md", ChunkIndex = 6, Score = 0.93,
                    Text = "Q1 2026 Outlook — Priorities: SmartLock BT2 launch (March), close 8+ enterprise IoT trials (€5M target), ContosoGuard 25% attach rate, cost optimization. Revenue guidance: Q1 2026: €44-46M (softer post-holiday). Full Year 2026: €195-210M (15-22% growth). Risks: Bluetooth 5.4 chipset supply (16-week lead times), Apple/Google smart home competition, EUR/USD volatility." }
            ],
            ExpectedAnswer = "Q1 2026 revenue guidance is €44-46M (typically softer post-holiday quarter). Full year 2026 guidance is €195-210M, representing 15-22% growth (quarterly-report-q4-2025.md).",
            GroundedInDocuments = ["quarterly-report-q4-2025.md"]
        },

        // Hallucination tests
        new RagDemoScenario
        {
            Id = "rag-h1",
            Category = "Hallucination Test",
            Title = "Stock Price (Not in documents)",
            Question = "What is Contoso's stock price?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "quarterly-report-q4-2025.md", ChunkIndex = 0, Score = 0.52,
                    Text = "Q4 2025 revenue €47.3M, +12% YoY. Smart Home division led growth. Strong Q4 performance." },
                new RetrievedChunk { SourceFile = "quarterly-report-q4-2025.md", ChunkIndex = 4, Score = 0.44,
                    Text = "Key Metrics — Gross Margin 52.3%, Operating Margin 14.7%, NPS 72, Return Rate 3.1%." }
            ],
            ExpectedAnswer = "I don't have enough information to answer that. The available documents contain financial metrics (revenue, margins) but no stock price information.",
            GroundedInDocuments = []
        },
        new RagDemoScenario
        {
            Id = "rag-h2",
            Category = "Hallucination Test",
            Title = "CTO Name (Not in documents)",
            Question = "Who is the CTO of Contoso Electronics?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "quarterly-report-q4-2025.md", ChunkIndex = 6, Score = 0.48,
                    Text = "Report approved by Maria van der Berg, CEO and Thomas Eriksson, CFO." }
            ],
            ExpectedAnswer = "I don't have enough information to answer that. The documents mention the CEO (Maria van der Berg) and CFO (Thomas Eriksson), but no CTO is referenced.",
            GroundedInDocuments = []
        },
        new RagDemoScenario
        {
            Id = "rag-h3",
            Category = "Hallucination Test",
            Title = "Television Products (Not in catalog)",
            Question = "Does Contoso sell televisions?",
            RetrievedChunks =
            [
                new RetrievedChunk { SourceFile = "product-catalog.json", ChunkIndex = 0, Score = 0.41,
                    Text = "Product categories: Smart Home Devices, Industrial IoT Sensors, Consumer Audio." },
                new RetrievedChunk { SourceFile = "product-catalog.json", ChunkIndex = 7, Score = 0.38,
                    Text = "SoundPillar S5 — Floor-standing wireless speaker, 360° sound, 120W, €599.99." }
            ],
            ExpectedAnswer = "Based on the product catalog, Contoso Electronics does not sell televisions. Their product categories are: Smart Home Devices, Industrial IoT Sensors, and Consumer Audio (product-catalog.json).",
            GroundedInDocuments = ["product-catalog.json"]
        }
    ];
}

/// <summary>
/// Represents a document ingested into the RAG pipeline.
/// </summary>
public class DemoDocument
{
    public string Id { get; set; } = "";
    public string FileName { get; set; } = "";
    public string FileType { get; set; } = "";
    public string Description { get; set; } = "";
    public int WordCount { get; set; }
    public int ChunkCount { get; set; }
    public string Icon { get; set; } = "📄";
}

/// <summary>
/// Represents a chunk of text from a document.
/// </summary>
public class DemoChunk
{
    public string Id { get; set; } = "";
    public string DocumentId { get; set; } = "";
    public int ChunkIndex { get; set; }
    public string Preview { get; set; } = "";
    public int TokenCount { get; set; }
}

/// <summary>
/// A pre-built RAG demo scenario with question, retrieved chunks, and expected answer.
/// </summary>
public class RagDemoScenario
{
    public string Id { get; set; } = "";
    public string Category { get; set; } = "";
    public string Title { get; set; } = "";
    public string Question { get; set; } = "";
    public List<RetrievedChunk> RetrievedChunks { get; set; } = [];
    public string ExpectedAnswer { get; set; } = "";
    public List<string> GroundedInDocuments { get; set; } = [];
}

/// <summary>
/// A chunk retrieved by vector similarity search with a relevance score.
/// </summary>
public class RetrievedChunk
{
    public string SourceFile { get; set; } = "";
    public int ChunkIndex { get; set; }
    public double Score { get; set; }
    public string Text { get; set; } = "";
}
