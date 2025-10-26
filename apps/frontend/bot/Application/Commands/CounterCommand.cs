using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class CounterCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<CounterCommandModule> _logger;

    public CounterCommandModule(ILogger<CounterCommandModule> logger)
    {
        _logger = logger;
    }

    [Command("counter")]
    [Summary("Get counter information for Pokemon")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task CounterAsync([Remainder] string pokemonName)
    {
        try
        {
            _logger.LogInformation("Counter command executed by {User} for Pokemon: {Pokemon}", Context.User.Username, pokemonName);

            if (string.IsNullOrEmpty(pokemonName))
            {
                await ReplyAsync("❌ Please provide a Pokemon name.\nExample: `!counter Mewtwo`");
                return;
            }

            // This is a placeholder implementation
            // In a real bot, you'd have a Pokemon database with counter information
            var embed = new EmbedBuilder()
                .WithTitle($"🔍 Counters for {pokemonName}")
                .WithDescription($"**Best Counters:**\n• Darkrai\n• Giratina (Origin)\n• Chandelure\n\n**Weather Boost:** Fog")
                .WithColor(Color.Purple)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter($"Requested by {Context.User.Username}", Context.User.GetAvatarUrl())
                .Build();

            await ReplyAsync(embed: embed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing counter command");
            await ReplyAsync("❌ An error occurred while getting counter information. Please try again.");
        }
    }
}
