using Bot.Service.Application.DTOs;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class RaidService : IRaidService
{
    private readonly ILogger<RaidService> _logger;
    private readonly IBotBffClient _botBffClient;
    private readonly Dictionary<string, RaidDto> _raids = new();

    public RaidService(ILogger<RaidService> logger, IBotBffClient botBffClient)
    {
        _logger = logger;
        _botBffClient = botBffClient;
    }

    public async Task<bool> CreateRaidAsync(string messageId, string title, DateTime raidTime, string startedBy, string guildId, string channelId)
    {
        try
        {
            var raid = new RaidDto
            {
                MessageId = messageId,
                MessageTitle = title,
                RaidTime = raidTime,
                StartedBy = startedBy,
                Closed = false,
                Players = new List<PlayerDto>()
            };

            _raids[messageId] = raid;

            // Optionally save to backend via Bot.BFF
            // await _botBffClient.CreateRaidAsync(raid);

            _logger.LogInformation("Raid created: {MessageId} - {Title} at {RaidTime}", messageId, title, raidTime);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create raid: {MessageId}", messageId);
            return false;
        }
    }

    public Task<RaidDto?> GetRaidAsync(string messageId)
    {
        _raids.TryGetValue(messageId, out var raid);
        return Task.FromResult(raid);
    }

    public Task UpdateRaidAsync(RaidDto raid)
    {
        _raids[raid.MessageId] = raid;
        return Task.CompletedTask;
    }

    public async Task AddPlayerToRaidAsync(string messageId, string userId, string playerName)
    {
        var raid = await GetRaidAsync(messageId);
        if (raid != null)
        {
            var existingPlayer = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (existingPlayer == null)
            {
                raid.Players.Add(new PlayerDto
                {
                    Id = userId,
                    Name = playerName,
                    Additions = 0
                });
                await UpdateRaidAsync(raid);
                _logger.LogInformation("Player {PlayerName} added to raid {MessageId}", playerName, messageId);
            }
        }
    }

    public async Task RemovePlayerFromRaidAsync(string messageId, string userId)
    {
        var raid = await GetRaidAsync(messageId);
        if (raid != null)
        {
            var player = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null)
            {
                raid.Players.Remove(player);
                await UpdateRaidAsync(raid);
                _logger.LogInformation("Player {PlayerName} removed from raid {MessageId}", player.Name, messageId);
            }
        }
    }

    public async Task AddPlayerExtraAsync(string messageId, string userId, int extraCount)
    {
        var raid = await GetRaidAsync(messageId);
        if (raid != null)
        {
            var player = raid.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null)
            {
                player.Additions = extraCount;
                await UpdateRaidAsync(raid);
                _logger.LogInformation("Player {PlayerName} set extra count to {ExtraCount} for raid {MessageId}", player.Name, extraCount, messageId);
            }
        }
    }
}
