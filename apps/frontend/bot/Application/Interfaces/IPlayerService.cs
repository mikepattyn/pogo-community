namespace Bot.Service.Application.Interfaces;

public interface IPlayerService
{
    Task<bool> RegisterPlayerAsync(string discordId, string team, string firstName, string nickname, int level);
    Task<bool> LevelUpPlayerAsync(string discordId);
    Task<object?> GetPlayerAsync(string discordId);
}
