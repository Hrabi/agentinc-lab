using AgenticLab.Core.Abstractions;
using System.Diagnostics;

namespace AgenticLab.Web.Services;

/// <summary>
/// Manages chat sessions and conversation history.
/// </summary>
public class ChatService
{
    private readonly AgentFactoryService _agentFactory;
    private readonly ILogger<ChatService> _logger;
    private readonly List<ChatSession> _sessions = [];

    public ChatService(AgentFactoryService agentFactory, ILogger<ChatService> logger)
    {
        _agentFactory = agentFactory;
        _logger = logger;
    }

    /// <summary>
    /// Gets all chat sessions.
    /// </summary>
    public IReadOnlyList<ChatSession> GetSessions() => _sessions.AsReadOnly();

    /// <summary>
    /// Creates a new chat session for a specific agent.
    /// </summary>
    public ChatSession CreateSession(string agentConfigId)
    {
        var session = new ChatSession
        {
            AgentConfigId = agentConfigId
        };
        _sessions.Add(session);
        return session;
    }

    /// <summary>
    /// Sends a message in a chat session and returns the response.
    /// </summary>
    public async Task<ChatEntry> SendMessageAsync(string sessionId, string message, CancellationToken cancellationToken = default)
    {
        var session = _sessions.Find(s => s.Id == sessionId)
            ?? throw new InvalidOperationException($"Session '{sessionId}' not found.");

        var agentConfig = _agentFactory.GetConfig(session.AgentConfigId)
            ?? throw new InvalidOperationException($"Agent config '{session.AgentConfigId}' not found.");

        var agent = _agentFactory.CreateAgent(agentConfig)
            ?? throw new InvalidOperationException($"Failed to create agent from config '{agentConfig.Id}'.");

        // Add user message
        var userEntry = new ChatEntry
        {
            Role = "user",
            Content = message
        };
        session.Entries.Add(userEntry);

        // Build request with history
        var request = new AgentRequest
        {
            Message = message,
            History = session.Entries
                .Where(e => e.Role is "user" or "assistant")
                .Select(e => new ChatMessage { Role = e.Role, Content = e.Content })
                .ToList(),
            Metadata = new Dictionary<string, object>
            {
                ["systemPrompt"] = agentConfig.SystemPromptOverride ?? "",
                ["temperature"] = agentConfig.TemperatureOverride ?? 0.7,
                ["maxTokens"] = agentConfig.MaxTokensOverride ?? 1000
            }
        };

        var sw = Stopwatch.StartNew();
        var response = await agent.ProcessAsync(request, cancellationToken);
        sw.Stop();

        var assistantEntry = new ChatEntry
        {
            Role = "assistant",
            Content = response.Message,
            AgentName = response.AgentName,
            DurationMs = sw.ElapsedMilliseconds,
            PromptTokens = response.Metadata?.TryGetValue("promptTokens", out var pt) == true ? Convert.ToInt32(pt) : 0,
            CompletionTokens = response.Metadata?.TryGetValue("completionTokens", out var ct) == true ? Convert.ToInt32(ct) : 0,
            ModelName = response.Metadata?.TryGetValue("model", out var mn) == true ? mn.ToString() : null,
            Success = response.Success
        };
        session.Entries.Add(assistantEntry);

        return assistantEntry;
    }

    /// <summary>
    /// Clears a session's history.
    /// </summary>
    public void ClearSession(string sessionId)
    {
        var session = _sessions.Find(s => s.Id == sessionId);
        session?.Entries.Clear();
    }

    /// <summary>
    /// Removes a session.
    /// </summary>
    public bool RemoveSession(string sessionId) => _sessions.RemoveAll(s => s.Id == sessionId) > 0;
}

/// <summary>
/// Represents a chat session with an agent.
/// </summary>
public class ChatSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string AgentConfigId { get; set; } = "";
    public List<ChatEntry> Entries { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// A single entry in a chat conversation.
/// </summary>
public class ChatEntry
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public string? AgentName { get; set; }
    public string? ModelName { get; set; }
    public long DurationMs { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public bool Success { get; set; } = true;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
