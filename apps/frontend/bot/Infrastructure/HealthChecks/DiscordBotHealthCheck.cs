using Microsoft.Extensions.Diagnostics.HealthChecks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Infrastructure.HealthChecks;

/// <summary>
/// Health check for Discord bot connection and gateway latency
/// </summary>
public class DiscordBotHealthCheck : IHealthCheck
{
    private readonly DiscordSocketClient _discordClient;
    private readonly ILogger<DiscordBotHealthCheck> _logger;

    public DiscordBotHealthCheck(DiscordSocketClient discordClient, ILogger<DiscordBotHealthCheck> logger)
    {
        _discordClient = discordClient;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionState = _discordClient.ConnectionState;
            var latency = _discordClient.Latency;

            // Check connection state
            if (connectionState != ConnectionState.Connected)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Discord bot is not connected. Current state: {connectionState}",
                    data: new Dictionary<string, object>
                    {
                        ["connection_state"] = connectionState.ToString(),
                        ["latency_ms"] = latency
                    }));
            }

            // Check gateway latency
            if (latency > 2000)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Discord gateway latency is too high: {latency}ms",
                    data: new Dictionary<string, object>
                    {
                        ["connection_state"] = connectionState.ToString(),
                        ["latency_ms"] = latency
                    }));
            }

            if (latency > 500)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Discord gateway latency is elevated: {latency}ms",
                    data: new Dictionary<string, object>
                    {
                        ["connection_state"] = connectionState.ToString(),
                        ["latency_ms"] = latency
                    }));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Discord bot is connected and responsive",
                data: new Dictionary<string, object>
                {
                    ["connection_state"] = connectionState.ToString(),
                    ["latency_ms"] = latency,
                    ["guild_count"] = _discordClient.Guilds.Count,
                    ["user_count"] = _discordClient.Guilds.Sum(g => g.MemberCount)
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with exception");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Health check failed with exception",
                ex,
                data: new Dictionary<string, object>
                {
                    ["exception"] = ex.Message
                }));
        }
    }
}
