using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class TestCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<TestCommandModule> _logger;

    public TestCommandModule(ILogger<TestCommandModule> logger)
    {
        _logger = logger;
    }

    [Command("test")]
    [Summary("Test command to verify bot is working")]
    public async Task TestAsync()
    {
        _logger.LogInformation("Test command executed by {User}", Context.User.Username);
        await ReplyAsync("Bot is working! 🤖");
    }
}
