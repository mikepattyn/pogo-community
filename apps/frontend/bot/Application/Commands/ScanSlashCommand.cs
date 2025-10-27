using Discord;
using Discord.Interactions;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Commands;

public class ScanSlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<ScanSlashCommandModule> _logger;
    private readonly IBotBffClient _botBffClient;
    private readonly IRaidService _raidService;

    public ScanSlashCommandModule(
        ILogger<ScanSlashCommandModule> logger,
        IBotBffClient botBffClient,
        IRaidService raidService)
    {
        _logger = logger;
        _botBffClient = botBffClient;
        _raidService = raidService;
    }

    [SlashCommand("scan", "Scan a raid image to extract information")]
    public async Task ScanAsync(
        [Summary("image", "Raid image to scan")] IAttachment image)
    {
        try
        {
            _logger.LogInformation("Slash scan command executed by {User} with image {ImageUrl}",
                Context.User.Username, image.Url);

            if (!IsValidImageUrl(image.Url))
            {
                await RespondAsync("❌ Please provide a valid image file.", ephemeral: true);
                return;
            }

            await RespondAsync("🔍 Processing image... Please wait.");

            try
            {
                // Call OCR service through Bot.BFF
                var scanRequest = new ScanImageRequest
                {
                    Url = image.Url
                };

                var scanResponse = await _botBffClient.ScanImageAsync(scanRequest);

                if (scanResponse.RaidData == null || string.IsNullOrEmpty(scanResponse.RaidData.PokemonName))
                {
                    await ModifyOriginalResponseAsync(m => m.Content = "❌ No data could be extracted from the image. Please try with a clearer image.");
                    return;
                }

                // Validate tier extraction
                if (scanResponse.RaidData.Tier <= 0 || scanResponse.RaidData.Tier > 5)
                {
                    await ModifyOriginalResponseAsync(m => m.Content = "❌ Could not extract raid tier from the image. Please try with a clearer image.");
                    return;
                }

                // Use structured data from OCR service
                var raidInfo = new RaidScanResult
                {
                    Tier = scanResponse.RaidData.Tier,
                    PokemonName = scanResponse.RaidData.PokemonName,
                    GymName = scanResponse.RaidData.GymName,
                    TimeInfo = scanResponse.RaidData.TimeRemaining,
                    IsHatched = !string.IsNullOrEmpty(scanResponse.RaidData.PokemonName) &&
                                scanResponse.RaidData.PokemonName.ToLower() != "unknown"
                };

                // Create raid embed
                var embed = new EmbedBuilder()
                    .WithTitle($"🗡️ T{raidInfo.Tier} {raidInfo.PokemonName}")
                    .WithDescription($"**Gym:** {raidInfo.GymName}\n**Time:** {raidInfo.TimeInfo}")
                    .WithColor(raidInfo.IsHatched ? Color.Green : Color.Orange)
                    .WithTimestamp(DateTimeOffset.Now)
                    .WithFooter($"Scanned by {Context.User.Username}", Context.User.GetAvatarUrl())
                    .AddField("Participants", "No one yet", true)
                    .AddField("Extra Players", "0", true)
                    .Build();

                await ModifyOriginalResponseAsync(m =>
                {
                    m.Content = "";
                    m.Embed = embed;
                });

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
                    $"T{raidInfo.Tier} {raidInfo.PokemonName}",
                    DateTime.Now, // For scanned raids, use current time
                    Context.User.Id.ToString(),
                    Context.Guild.Id.ToString(),
                    Context.Channel.Id.ToString()
                );

                _logger.LogInformation("Slash raid scan completed successfully: T{tier} {pokemon} at {gym}",
                    raidInfo.Tier, raidInfo.PokemonName, raidInfo.GymName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing slash scan request");
                await ModifyOriginalResponseAsync(m => m.Content = "❌ An error occurred while processing the image. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing slash scan command");
            await RespondAsync("❌ An error occurred while scanning. Please try again.", ephemeral: true);
        }
    }

    private static bool IsValidImageUrl(string url)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        return validExtensions.Any(ext => url.ToLower().Contains(ext));
    }

    private class RaidScanResult
    {
        public int Tier { get; set; }
        public string PokemonName { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string TimeInfo { get; set; } = string.Empty;
        public bool IsHatched { get; set; }
    }
}
