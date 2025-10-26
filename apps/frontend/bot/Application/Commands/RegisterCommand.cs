using Discord;
using Discord.Commands;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Bot.Service.Application.Commands;

public class RegisterCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<RegisterCommandModule> _logger;
    private readonly IPlayerService _playerService;

    public RegisterCommandModule(
        ILogger<RegisterCommandModule> logger,
        IPlayerService playerService)
    {
        _logger = logger;
        _playerService = playerService;
    }

    [Command("register")]
    [Summary("Register a player with team, name, nickname, and level")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task RegisterAsync([Remainder] string args)
    {
        try
        {
            _logger.LogInformation("Register command executed by {User} with args: {Args}", Context.User.Username, args);

            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1 && parts[0].ToLower() == "levelup")
            {
                await HandleLevelUpAsync();
                return;
            }

            if (parts.Length < 4)
            {
                await ReplyAsync("❌ Invalid format. Use: `!register TEAM FIRSTNAME NICKNAME LEVEL`\nExample: `!register Valor Mike TrainerMike 40`\nOr: `!register levelup`");
                return;
            }

            var team = parts[0];
            var firstName = parts[1];
            var nickname = parts[2];

            if (!int.TryParse(parts[3], out int level) || level < 1 || level > 50)
            {
                await ReplyAsync("❌ Invalid level. Level must be between 1 and 50.");
                return;
            }

            // Validate team
            var validTeams = new[] { "Valor", "Mystic", "Instinct", "Harmony" };
            if (!validTeams.Contains(team, StringComparer.OrdinalIgnoreCase))
            {
                await ReplyAsync("❌ Invalid team. Valid teams are: Valor, Mystic, Instinct, Harmony");
                return;
            }

            var success = await _playerService.RegisterPlayerAsync(
                Context.User.Id.ToString(),
                team,
                firstName,
                nickname,
                level
            );

            if (success)
            {
                var embed = new EmbedBuilder()
                    .WithTitle("✅ Player Registered Successfully!")
                    .WithDescription($"**Team:** {team}\n**Name:** {firstName}\n**Nickname:** {nickname}\n**Level:** {level}")
                    .WithColor(Color.Green)
                    .WithTimestamp(DateTimeOffset.Now)
                    .Build();

                await ReplyAsync(embed: embed);
                _logger.LogInformation("Player registered successfully: {User} - {Team} {FirstName} L{Level}", Context.User.Username, team, firstName, level);
            }
            else
            {
                await ReplyAsync("❌ Failed to register player. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing register command");
            await ReplyAsync("❌ An error occurred while registering. Please try again.");
        }
    }

    private async Task HandleLevelUpAsync()
    {
        try
        {
            var success = await _playerService.LevelUpPlayerAsync(Context.User.Id.ToString());

            if (success)
            {
                await ReplyAsync("🎉 Congratulations on leveling up! Your level has been updated.");
                _logger.LogInformation("Player leveled up: {User}", Context.User.Username);
            }
            else
            {
                await ReplyAsync("❌ Failed to update level. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing level up command");
            await ReplyAsync("❌ An error occurred while updating your level. Please try again.");
        }
    }
}
