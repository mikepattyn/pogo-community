using Bot.Service.Application.DTOs;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class RaidService : IRaidService
{
    private readonly ILogger<RaidService> _logger;
    private readonly IBotBffClient _botBffClient;

    public RaidService(ILogger<RaidService> logger, IBotBffClient botBffClient)
    {
        _logger = logger;
        _botBffClient = botBffClient;
    }

    public async Task<bool> CreateRaidAsync(string messageId, string title, DateTime raidTime, string startedBy, string guildId, string channelId)
    {
        try
        {
            // For now, just log the raid creation
            // The actual raid will be created by the ScanSlashCommand when it posts to the backend
            _logger.LogInformation("Raid created in memory: {MessageId} - {Title} at {RaidTime}", messageId, title, raidTime);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create raid: {MessageId}", messageId);
            return false;
        }
    }

    public async Task<RaidDto?> GetRaidAsync(string messageId)
    {
        try
        {
            var raidResponse = await _botBffClient.GetRaidByMessageIdAsync(messageId);
            if (raidResponse == null)
            {
                return null;
            }

            // Convert backend raid to bot's RaidDto format
            var raid = new RaidDto
            {
                MessageId = raidResponse.DiscordMessageId,
                MessageTitle = $"T{raidResponse.Level} {raidResponse.PokemonSpecies}",
                RaidTime = raidResponse.StartTime,
                Closed = !raidResponse.IsActive || raidResponse.IsCompleted || raidResponse.IsCancelled,
                StartedBy = "", // Not available from backend
                Players = new List<PlayerDto>() // TODO: Get from participants endpoint
            };

            return raid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting raid by message ID: {MessageId}", messageId);
            return null;
        }
    }

    public Task UpdateRaidAsync(RaidDto raid)
    {
        // Updates are now handled by backend
        return Task.CompletedTask;
    }

    public async Task AddPlayerToRaidAsync(string messageId, string userId, string playerName)
    {
        try
        {
            var raid = await GetRaidAsync(messageId);
            if (raid == null)
            {
                _logger.LogWarning("No raid found for message {MessageId}", messageId);
                return;
            }

            // Get player ID from backend first
            var playerData = await _botBffClient.GetPlayerAsync(userId);
            if (playerData == null)
            {
                _logger.LogWarning("No player found for Discord ID {UserId}", userId);
                return;
            }

            // For now, just track locally since we don't have participant management yet
            var existingPlayer = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (existingPlayer == null)
            {
                raid.Players.Add(new PlayerDto
                {
                    Id = userId,
                    Name = playerName,
                    Additions = 0
                });
                _logger.LogInformation("Player {PlayerName} added to raid {MessageId}", playerName, messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player to raid {MessageId}", messageId);
        }
    }

    public async Task RemovePlayerFromRaidAsync(string messageId, string userId)
    {
        try
        {
            var raid = await GetRaidAsync(messageId);
            if (raid == null)
            {
                _logger.LogWarning("No raid found for message {MessageId}", messageId);
                return;
            }

            var player = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null)
            {
                raid.Players.Remove(player);
                _logger.LogInformation("Player {PlayerName} removed from raid {MessageId}", player.Name, messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing player from raid {MessageId}", messageId);
        }
    }

    public async Task AddPlayerExtraAsync(string messageId, string userId, int extraCount)
    {
        try
        {
            var raid = await GetRaidAsync(messageId);
            if (raid == null)
            {
                _logger.LogWarning("No raid found for message {MessageId}", messageId);
                return;
            }

            var player = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null)
            {
                player.Additions = extraCount;
                _logger.LogInformation("Player {PlayerName} set extra count to {ExtraCount} for raid {MessageId}", player.Name, extraCount, messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player extras for raid {MessageId}", messageId);
        }
    }
}
