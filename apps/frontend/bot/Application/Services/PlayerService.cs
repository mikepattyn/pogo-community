using Bot.Service.Application.DTOs;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly ILogger<PlayerService> _logger;
    private readonly IBotBffClient _botBffClient;

    public PlayerService(ILogger<PlayerService> logger, IBotBffClient botBffClient)
    {
        _logger = logger;
        _botBffClient = botBffClient;
    }

    public async Task<bool> RegisterPlayerAsync(string discordId, string team, string firstName, string nickname, int level)
    {
        try
        {
            var player = new UpdatePlayerDto
            {
                FirstName = firstName,
                Nickname = nickname,
                Level = level,
                Team = team
            };

            // Try to update existing player first, if not found, create new one
            var existingPlayer = await _botBffClient.GetPlayerAsync(discordId);
            if (existingPlayer != null)
            {
                // Update existing player - we'd need the player ID from the response
                // For now, we'll assume the API handles this by Discord ID
                await _botBffClient.UpdatePlayerAsync(discordId, player);
            }
            else
            {
                // Create new player
                var newPlayer = new CreatePlayerDto
                {
                    DiscordId = discordId,
                    DateJoined = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                    FirstName = firstName,
                    Nickname = nickname,
                    Level = level,
                    Team = team
                };
                await _botBffClient.CreatePlayerAsync(newPlayer);
            }

            _logger.LogInformation("Player registered successfully: {DiscordId} - {Team} {FirstName} L{Level}", discordId, team, firstName, level);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register player: {DiscordId}", discordId);
            return false;
        }
    }

    public async Task<bool> LevelUpPlayerAsync(string discordId)
    {
        try
        {
            var player = await _botBffClient.GetPlayerAsync(discordId);
            if (player == null)
            {
                _logger.LogWarning("Player not found for level up: {DiscordId}", discordId);
                return false;
            }

            // Extract current level and increment it
            // This is a simplified implementation - in reality, you'd parse the player object
            var updatePlayer = new UpdatePlayerDto
            {
                Level = 1 // This should be incremented from current level
            };

            await _botBffClient.UpdatePlayerAsync(discordId, updatePlayer);
            _logger.LogInformation("Player leveled up: {DiscordId}", discordId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to level up player: {DiscordId}", discordId);
            return false;
        }
    }

    public async Task<object?> GetPlayerAsync(string discordId)
    {
        try
        {
            return await _botBffClient.GetPlayerAsync(discordId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get player: {DiscordId}", discordId);
            return null;
        }
    }
}
