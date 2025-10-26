using Discord;
using Discord.WebSocket;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
    }

    public async Task SendRaidEmbedAsync(IUserMessage message, string title, string description, string tier, string? pokemon = null, bool isHatched = false)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithColor(isHatched ? Color.Green : Color.Orange)
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await message.ModifyAsync(m => m.Embed = embed);
    }

    public async Task UpdateRaidEmbedAsync(IUserMessage message, string description)
    {
        if (message.Embeds.Count > 0)
        {
            var embed = message.Embeds.First().ToEmbedBuilder()
                .WithDescription(description)
                .Build();

            await message.ModifyAsync(m => m.Embed = embed);
        }
    }

    public async Task HandleRaidStartAsync(IUserMessage message)
    {
        _logger.LogInformation("Handling raid start for message {MessageId}", message.Id);
        // Implementation for raid start handling
        await Task.CompletedTask;
    }

    public async Task HandleRankRequestAsync(SocketUserMessage message)
    {
        _logger.LogInformation("Handling rank request from user {User}", message.Author.Username);
        // Implementation for rank request handling
        await Task.CompletedTask;
    }

    public async Task HandleLevelUpRequestAsync(SocketUserMessage message)
    {
        _logger.LogInformation("Handling level up request from user {User}", message.Author.Username);
        // Implementation for level up request handling
        await Task.CompletedTask;
    }
}
