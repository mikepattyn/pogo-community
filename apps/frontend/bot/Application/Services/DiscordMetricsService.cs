using Prometheus;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

/// <summary>
/// Service for tracking Discord bot metrics using Prometheus
/// </summary>
public class DiscordMetricsService
{
    private readonly ILogger<DiscordMetricsService> _logger;

    // Event counters
    private static readonly Counter DiscordEventsTotal = Metrics
        .CreateCounter("discord_events_total", "Total number of Discord events", new[] { "event_type" });

    // Gateway metrics
    private static readonly Gauge DiscordGatewayLatency = Metrics
        .CreateGauge("discord_gateway_latency_milliseconds", "Discord gateway latency in milliseconds");

    private static readonly Gauge DiscordConnectionState = Metrics
        .CreateGauge("discord_connection_state", "Discord connection state (0=disconnected, 1=connecting, 2=connected)");

    // Command metrics
    private static readonly Counter DiscordCommandsTotal = Metrics
        .CreateCounter("discord_commands_total", "Total number of Discord commands executed", new[] { "command_name", "status" });

    private static readonly Histogram DiscordCommandDuration = Metrics
        .CreateHistogram("discord_command_duration_seconds", "Discord command execution duration", new[] { "command_name" });

    // Guild metrics
    private static readonly Gauge DiscordGuildsTotal = Metrics
        .CreateGauge("discord_guilds_total", "Total number of Discord guilds");

    private static readonly Gauge DiscordGuildMembersTotal = Metrics
        .CreateGauge("discord_guild_members_total", "Total number of members in Discord guilds", new[] { "guild_id", "guild_name" });

    // Bot status metrics
    private static readonly Gauge DiscordBotUptime = Metrics
        .CreateGauge("discord_bot_uptime_seconds", "Discord bot uptime in seconds");

    private static readonly Gauge DiscordBotUsersTotal = Metrics
        .CreateGauge("discord_bot_users_total", "Total number of unique users across all guilds");

    private DateTime _startTime = DateTime.UtcNow;

    public DiscordMetricsService(ILogger<DiscordMetricsService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Track Discord events (messages, reactions, etc.)
    /// </summary>
    public void TrackEvent(string eventType)
    {
        DiscordEventsTotal.WithLabels(eventType).Inc();
        _logger.LogDebug("Tracked Discord event: {EventType}", eventType);
    }

    /// <summary>
    /// Update gateway latency metric
    /// </summary>
    public void UpdateGatewayLatency(int latencyMs)
    {
        DiscordGatewayLatency.Set(latencyMs);
    }

    /// <summary>
    /// Update connection state metric
    /// </summary>
    public void UpdateConnectionState(ConnectionState state)
    {
        var stateValue = state switch
        {
            ConnectionState.Disconnected => 0,
            ConnectionState.Connecting => 1,
            ConnectionState.Connected => 2,
            _ => 0
        };
        DiscordConnectionState.Set(stateValue);
    }

    /// <summary>
    /// Track command execution
    /// </summary>
    public void TrackCommand(string commandName, bool success, TimeSpan duration)
    {
        var status = success ? "success" : "failure";
        DiscordCommandsTotal.WithLabels(commandName, status).Inc();
        DiscordCommandDuration.WithLabels(commandName).Observe(duration.TotalSeconds);
        
        _logger.LogDebug("Tracked command: {CommandName}, Status: {Status}, Duration: {Duration}ms", 
            commandName, status, duration.TotalMilliseconds);
    }

    /// <summary>
    /// Update guild metrics
    /// </summary>
    public void UpdateGuildMetrics(IReadOnlyCollection<SocketGuild> guilds)
    {
        DiscordGuildsTotal.Set(guilds.Count);
        
        var totalUsers = 0;
        foreach (var guild in guilds)
        {
            DiscordGuildMembersTotal.WithLabels(guild.Id.ToString(), guild.Name).Set(guild.MemberCount);
            totalUsers += guild.MemberCount;
        }
        
        DiscordBotUsersTotal.Set(totalUsers);
        
        _logger.LogDebug("Updated guild metrics: {GuildCount} guilds, {TotalUsers} users", guilds.Count, totalUsers);
    }

    /// <summary>
    /// Update bot uptime metric
    /// </summary>
    public void UpdateUptime()
    {
        var uptime = DateTime.UtcNow - _startTime;
        DiscordBotUptime.Set(uptime.TotalSeconds);
    }

    /// <summary>
    /// Track message received event
    /// </summary>
    public void TrackMessageReceived()
    {
        TrackEvent("message_received");
    }

    /// <summary>
    /// Track reaction added event
    /// </summary>
    public void TrackReactionAdded()
    {
        TrackEvent("reaction_added");
    }

    /// <summary>
    /// Track reaction removed event
    /// </summary>
    public void TrackReactionRemoved()
    {
        TrackEvent("reaction_removed");
    }

    /// <summary>
    /// Track user joined event
    /// </summary>
    public void TrackUserJoined()
    {
        TrackEvent("user_joined");
    }

    /// <summary>
    /// Track command executed event
    /// </summary>
    public void TrackCommandExecuted()
    {
        TrackEvent("command_executed");
    }
}
