using Discord;
using Discord.Commands;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Bot.Service.Application.Commands;

public class RaidCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<RaidCommandModule> _logger;
    private readonly IRaidService _raidService;
    private readonly IMessageService _messageService;

    public RaidCommandModule(
        ILogger<RaidCommandModule> logger,
        IRaidService raidService,
        IMessageService messageService)
    {
        _logger = logger;
        _raidService = raidService;
        _messageService = messageService;
    }

    [Command("raid")]
    [Summary("Create a raid with tier, pokemon, and time")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task RaidAsync([Remainder] string args)
    {
        try
        {
            _logger.LogInformation("Raid command executed by {User} with args: {Args}", Context.User.Username, args);

            // Parse raid command arguments
            // Expected format: !raid start TIER POKEMON_NAME TIME
            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 4 || parts[0].ToLower() != "start")
            {
                await ReplyAsync("❌ Invalid format. Use: `!raid start TIER POKEMON_NAME TIME`\nExample: `!raid start 5 Mewtwo 19:30`");
                return;
            }

            if (!int.TryParse(parts[1], out int tier) || tier < 1 || tier > 5)
            {
                await ReplyAsync("❌ Invalid tier. Tier must be between 1 and 5.");
                return;
            }

            var pokemonName = parts[2];
            var timeString = parts[3];

            // Parse time (HH:MM format)
            if (!TimeSpan.TryParse(timeString, out var raidTime))
            {
                await ReplyAsync("❌ Invalid time format. Use HH:MM format (e.g., 19:30).");
                return;
            }

            // Calculate raid datetime
            var raidDateTime = DateTime.Today.Add(raidTime);
            if (raidDateTime <= DateTime.Now)
            {
                raidDateTime = raidDateTime.AddDays(1); // If time has passed today, schedule for tomorrow
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

            var message = await ReplyAsync(embed: embed);

            // Add reaction emojis
            await message.AddReactionAsync(new Emoji("👍")); // Join raid
            await message.AddReactionAsync(new Emoji("1⃣")); // Extra players
            await message.AddReactionAsync(new Emoji("2⃣"));
            await message.AddReactionAsync(new Emoji("3⃣"));
            await message.AddReactionAsync(new Emoji("4⃣"));
            await message.AddReactionAsync(new Emoji("5⃣"));

            // Create raid in service
            await _raidService.CreateRaidAsync(
                message.Id.ToString(),
                $"T{tier} {pokemonName}",
                raidDateTime,
                Context.User.Id.ToString(),
                Context.Guild.Id.ToString(),
                Context.Channel.Id.ToString()
            );

            _logger.LogInformation("Raid created successfully: T{tier} {pokemon} at {time}", tier, pokemonName, raidDateTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing raid command");
            await ReplyAsync("❌ An error occurred while creating the raid. Please try again.");
        }
    }
}
