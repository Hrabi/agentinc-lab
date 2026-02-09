using AgenticLab.Agents;
using AgenticLab.Core.Abstractions;
using AgenticLab.Models.Ollama;
using AgenticLab.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ──────────────────────────────────────────────
// AgenticLab Demo — Simple Question Agent
// ──────────────────────────────────────────────

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║        AgenticLab — Demo             ║");
Console.WriteLine("║   Simple Question Agent              ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.WriteLine();

// Set up dependency injection
var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Register Ollama model (ensure Ollama is running locally)
services.AddSingleton<IModel>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<OllamaModel>>();
    return new OllamaModel(
        endpoint: "http://localhost:11434",
        modelName: "llama3.2",
        logger: logger
    );
});

// Register agent
services.AddTransient<SimpleQuestionAgent>();

// Register runtime
services.AddSingleton<AgentRuntime>();

var provider = services.BuildServiceProvider();

// Set up runtime
var runtime = provider.GetRequiredService<AgentRuntime>();
var agent = provider.GetRequiredService<SimpleQuestionAgent>();
runtime.RegisterAgent(agent);

Console.WriteLine($"Registered agents: {string.Join(", ", runtime.GetRegisteredAgents())}");
Console.WriteLine();

// Interactive loop
Console.WriteLine("Type a question (or 'quit' to exit):");
Console.WriteLine("─────────────────────────────────────");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    try
    {
        var response = await runtime.SendAsync("SimpleQuestion", new AgentRequest { Message = input });
        Console.WriteLine($"\n[{response.AgentName}]: {response.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError: {ex.Message}");
    }
}
