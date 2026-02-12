using AgenticLab.Models.Ollama;
using AgenticLab.Runtime;
using AgenticLab.Web.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

// Register AgenticLab services
builder.Services.AddSingleton<ModelRegistryService>();
builder.Services.AddSingleton<AgentFactoryService>();
builder.Services.AddSingleton<ExampleService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<CompareService>();
builder.Services.AddSingleton<ExportService>();
builder.Services.AddSingleton<RagDemoService>();
builder.Services.AddSingleton<AgentRuntime>();

// Register HttpClient for Ollama — no retries, generous timeout for LLM inference
var ollamaTimeout = TimeSpan.FromMinutes(
    builder.Configuration.GetValue<int>("Ollama:TimeoutMinutes", 5));

builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("Ollama:Endpoint") ?? "http://localhost:11434");
    client.Timeout = ollamaTimeout;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<AgenticLab.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
