using Discord;
using Discord.Interactions;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class RegisterSlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<RegisterSlashCommandModule> _logger;
    private readonly IPlayerService _playerService;

    public RegisterSlashCommandModule(
        ILogger<RegisterSlashCommandModule> logger,
        IPlayerService playerService)
    {
        _logger = logger;
        _playerService = playerService;
    }

    [SlashCommand("register", "Register a player with team, name, nickname, and level")]
    public async Task RegisterAsync(
        [Summary("team", "Player team")] string team,
        [Summary("firstname", "Your first name")] string firstName,
        [Summary("nickname", "Your in-game nickname")] string nickname,
        [Summary("level", "Your trainer level (1-50)")] int level)
    {
        try
        {
            _logger.LogInformation("Slash register command executed by {User}: {Team} {FirstName} L{Level}", 
                Context.User.Username, team, firstName, level);

            if (level < 1 || level > 50)
            {
                await RespondAsync("❌ Invalid level. Level must be between 1 and 50.", ephemeral: true);
                return;
            }

            // Validate team
            var validTeams = new[] { "Valor", "Mystic", "Instinct", "Harmony" };
            if (!validTeams.Contains(team, StringComparer.OrdinalIgnoreCase))
            {
                await RespondAsync("❌ Invalid team. Valid teams are: Valor, Mystic, Instinct, Harmony", ephemeral: true);
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

                await RespondAsync(embed: embed);
                _logger.LogInformation("Slash player registered successfully: {User} - {Team} {FirstName} L{Level}", 
                    Context.User.Username, team, firstName, level);
            }
            else
            {
                await RespondAsync("❌ Failed to register player. Please try again.", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing slash register command");
            await RespondAsync("❌ An error occurred while registering. Please try again.", ephemeral: true);
        }
    }

    [SlashCommand("levelup", "Update your trainer level")]
    public async Task LevelUpAsync()
    {
        try
        {
            _logger.LogInformation("Slash levelup command executed by {User}", Context.User.Username);

            var success = await _playerService.LevelUpPlayerAsync(Context.User.Id.ToString());

            if (success)
            {
                await RespondAsync("🎉 Congratulations on leveling up! Your level has been updated.");
                _logger.LogInformation("Slash player leveled up: {User}", Context.User.Username);
            }
            else
            {
                await RespondAsync("❌ Failed to update level. Please try again.", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing slash levelup command");
            await RespondAsync("❌ An error occurred while updating your level. Please try again.", ephemeral: true);
        }
    }
}
