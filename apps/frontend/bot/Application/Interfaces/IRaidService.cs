using Bot.Service.Application.DTOs;

namespace Bot.Service.Application.Interfaces;

public interface IRaidService
{
    Task<bool> CreateRaidAsync(string messageId, string title, DateTime raidTime, string startedBy, string guildId, string channelId);
    Task<RaidDto?> GetRaidAsync(string messageId);
    Task UpdateRaidAsync(RaidDto raid);
    Task AddPlayerToRaidAsync(string messageId, string userId, string playerName);
    Task RemovePlayerFromRaidAsync(string messageId, string userId);
    Task AddPlayerExtraAsync(string messageId, string userId, int extraCount);
}