using Discord;
using Discord.Interactions;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class RaidSlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<RaidSlashCommandModule> _logger;
    private readonly IRaidService _raidService;

    public RaidSlashCommandModule(
        ILogger<RaidSlashCommandModule> logger,
        IRaidService raidService)
    {
        _logger = logger;
        _raidService = raidService;
    }

    [SlashCommand("raid", "Create a raid with tier, pokemon, and time")]
    public async Task RaidAsync(
        [Summary("tier", "Raid tier (1-5)")] int tier,
        [Summary("pokemon", "Pokemon name")] string pokemonName,
        [Summary("time", "Raid time (HH:MM format)")] string timeString)
    {
        try
        {
            _logger.LogInformation("Slash raid command executed by {User}: T{tier} {pokemon} at {time}",
                Context.User.Username, tier, pokemonName, timeString);

            if (tier < 1 || tier > 5)
            {
                await RespondAsync("❌ Invalid tier. Tier must be between 1 and 5.", ephemeral: true);
                return;
            }

            if (!TimeSpan.TryParse(timeString, out var raidTime))
            {
                await RespondAsync("❌ Invalid time format. Use HH:MM format (e.g., 19:30).", ephemeral: true);
                return;
            }

            // Calculate raid datetime
            var raidDateTime = DateTime.Today.Add(raidTime);
            if (raidDateTime <= DateTime.Now)
            {
                raidDateTime = raidDateTime.AddDays(1);
            }

            // Create raid embed
            var embed = new EmbedBuilder()
                .WithTitle($"🗡️ T{tier} {pokemonName}")
                .WithDescription($"Gym: TBD\nIt starts at {raidDateTime:HH:mm}")
                .WithColor(Color.Orange)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter($"Started by {Context.User.Username}", Context.User.GetAvatarUrl())
                .AddField("Participants", "No one yet", true)
                .AddField("Extra Players", "0", true)
                .Build();

            await RespondAsync(embed: embed);

            // Get the response message to add reactions
            var response = await GetOriginalResponseAsync();
            await response.AddReactionAsync(new Emoji("👍"));
            await response.AddReactionAsync(new Emoji("1⃣"));
            await response.AddReactionAsync(new Emoji("2⃣"));
            await response.AddReactionAsync(new Emoji("3⃣"));
            await response.AddReactionAsync(new Emoji("4⃣"));
            await response.AddReactionAsync(new Emoji("5⃣"));

            // Create raid in service
            await _raidService.CreateRaidAsync(
                response.Id.ToString(),
                $"T{tier} {pokemonName}",
                raidDateTime,
                Context.User.Id.ToString(),
                Context.Guild.Id.ToString(),
                Context.Channel.Id.ToString()
            );

            _logger.LogInformation("Slash raid created successfully: T{tier} {pokemon} at {time}", tier, pokemonName, raidDateTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing slash raid command");
            await RespondAsync("❌ An error occurred while creating the raid. Please try again.", ephemeral: true);
        }
    }
}
