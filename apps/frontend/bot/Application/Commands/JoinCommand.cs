using Discord;
using Discord.Commands;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class JoinCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<JoinCommandModule> _logger;
    private readonly IRaidService _raidService;

    public JoinCommandModule(
        ILogger<JoinCommandModule> logger,
        IRaidService raidService)
    {
        _logger = logger;
        _raidService = raidService;
    }

    [Command("join")]
    [Summary("Join a raid by message ID")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task JoinAsync([Remainder] string messageId)
    {
        try
        {
            _logger.LogInformation("Join command executed by {User} for raid {MessageId}", Context.User.Username, messageId);

            if (string.IsNullOrEmpty(messageId))
            {
                await ReplyAsync("❌ Please provide a raid message ID to join.");
                return;
            }

            await _raidService.AddPlayerToRaidAsync(
                messageId,
                Context.User.Id.ToString(),
                Context.User.Username
            );

            await ReplyAsync($"✅ You've joined the raid!");
            _logger.LogInformation("User {User} joined raid {MessageId}", Context.User.Username, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing join command");
            await ReplyAsync("❌ An error occurred while joining the raid. Please try again.");
        }
    }
}
