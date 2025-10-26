using Discord;

namespace Bot.Service.Application.Interfaces;

public interface IMessageService
{
    Task SendRaidEmbedAsync(IUserMessage message, string title, string description, string tier, string? pokemon = null, bool isHatched = false);
    Task UpdateRaidEmbedAsync(IUserMessage message, string description);
    Task HandleRaidStartAsync(IUserMessage message);
    Task HandleRankRequestAsync(SocketUserMessage message);
    Task HandleLevelUpRequestAsync(SocketUserMessage message);
}