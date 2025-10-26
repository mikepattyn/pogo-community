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

    public ScanSlashCommandModule(
        ILogger<ScanSlashCommandModule> logger,
        IBotBffClient botBffClient)
    {
        _logger = logger;
        _botBffClient = botBffClient;
    }

    [SlashCommand("scan", "Scan a raid image to extract information")]
    public async Task ScanAsync(
        [Summary("tier", "Raid tier (1-5)")] int tier,
        [Summary("image", "Raid image to scan")] IAttachment image)
    {
        try
        {
            _logger.LogInformation("Slash scan command executed by {User}: T{tier} with image {ImageUrl}", 
                Context.User.Username, tier, image.Url);

            if (tier < 1 || tier > 5)
            {
                await RespondAsync("❌ Invalid tier. Tier must be between 1 and 5.", ephemeral: true);
                return;
            }

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

                if (scanResponse.TextResults.Length == 0)
                {
                    await ModifyOriginalResponseAsync(m => m.Content = "❌ No text could be extracted from the image. Please try with a clearer image.");
                    return;
                }

                // Parse OCR results
                var raidInfo = ParseRaidInfo(scanResponse.TextResults, tier);

                // Create raid embed
                var embed = new EmbedBuilder()
                    .WithTitle($"🗡️ T{tier} {raidInfo.PokemonName}")
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

                _logger.LogInformation("Slash raid scan completed successfully: T{tier} {pokemon} at {gym}", 
                    tier, raidInfo.PokemonName, raidInfo.GymName);
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

    private static RaidScanResult ParseRaidInfo(string[] textResults, int tier)
    {
        var result = new RaidScanResult
        {
            Tier = tier,
            PokemonName = "Unknown",
            GymName = "Unknown Gym",
            TimeInfo = "Unknown",
            IsHatched = false
        };

        // Simple parsing logic - in a real implementation, this would be more sophisticated
        foreach (var text in textResults)
        {
            // Look for time patterns (HH:MM)
            if (System.Text.RegularExpressions.Regex.IsMatch(text, @"\d{1,2}:\d{2}"))
            {
                result.TimeInfo = text;
            }

            // Look for gym names (usually longer text without numbers)
            if (text.Length > 5 && !System.Text.RegularExpressions.Regex.IsMatch(text, @"\d") && !text.Contains(":"))
            {
                result.GymName = text;
            }

            // Look for Pokemon names (this would need a Pokemon database lookup)
            if (text.Length > 3 && text.Length < 20 && !System.Text.RegularExpressions.Regex.IsMatch(text, @"\d"))
            {
                result.PokemonName = text;
            }
        }

        // Determine if hatched based on Pokemon name presence
        result.IsHatched = !string.IsNullOrEmpty(result.PokemonName) && result.PokemonName != "Unknown";

        return result;
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
