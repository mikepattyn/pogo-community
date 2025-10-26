using Discord;
using Discord.Commands;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Bot.Service.Application.Commands;

public class ScanCommandModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<ScanCommandModule> _logger;
    private readonly IBotBffClient _botBffClient;

    public ScanCommandModule(
        ILogger<ScanCommandModule> logger,
        IBotBffClient botBffClient)
    {
        _logger = logger;
        _botBffClient = botBffClient;
    }

    [Command("scan")]
    [Summary("Scan a raid image to extract information")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task ScanAsync([Remainder] string args)
    {
        try
        {
            _logger.LogInformation("Scan command executed by {User} with args: {Args}", Context.User.Username, args);

            // Parse tier argument (T1-T5)
            var tierMatch = Regex.Match(args, @"T([1-5])", RegexOptions.IgnoreCase);
            if (!tierMatch.Success)
            {
                await ReplyAsync("❌ Invalid format. Use: `!scan T1-5`\nExample: `!scan T5`");
                return;
            }

            var tier = int.Parse(tierMatch.Groups[1].Value);

            // Check if message has attachment
            if (Context.Message.Attachments.Count == 0)
            {
                await ReplyAsync("❌ Please attach an image to scan.");
                return;
            }

            var attachment = Context.Message.Attachments.First();
            if (!IsValidImageUrl(attachment.Url))
            {
                await ReplyAsync("❌ Please attach a valid image file.");
                return;
            }

            // Delete the original command message
            await Context.Message.DeleteAsync();

            // Send processing message
            var processingMessage = await ReplyAsync("🔍 Processing image... Please wait.");

            try
            {
                // Call OCR service through Bot.BFF
                var scanRequest = new ScanImageRequest
                {
                    Url = attachment.Url
                };

                var scanResponse = await _botBffClient.ScanImageAsync(scanRequest);

                if (scanResponse.TextResults.Length == 0)
                {
                    await processingMessage.ModifyAsync(m => m.Content = "❌ No text could be extracted from the image. Please try with a clearer image.");
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

                await processingMessage.ModifyAsync(m =>
                {
                    m.Content = "";
                    m.Embed = embed;
                });

                // Add reaction emojis
                await processingMessage.AddReactionAsync(new Emoji("👍")); // Join raid
                await processingMessage.AddReactionAsync(new Emoji("1⃣")); // Extra players
                await processingMessage.AddReactionAsync(new Emoji("2⃣"));
                await processingMessage.AddReactionAsync(new Emoji("3⃣"));
                await processingMessage.AddReactionAsync(new Emoji("4⃣"));
                await processingMessage.AddReactionAsync(new Emoji("5⃣"));

                _logger.LogInformation("Raid scan completed successfully: T{tier} {pokemon} at {gym}", tier, raidInfo.PokemonName, raidInfo.GymName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scan request");
                await processingMessage.ModifyAsync(m => m.Content = "❌ An error occurred while processing the image. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scan command");
            await ReplyAsync("❌ An error occurred while scanning. Please try again.");
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
            if (Regex.IsMatch(text, @"\d{1,2}:\d{2}"))
            {
                result.TimeInfo = text;
            }

            // Look for gym names (usually longer text without numbers)
            if (text.Length > 5 && !Regex.IsMatch(text, @"\d") && !text.Contains(":"))
            {
                result.GymName = text;
            }

            // Look for Pokemon names (this would need a Pokemon database lookup)
            // For now, we'll use a simple heuristic
            if (text.Length > 3 && text.Length < 20 && !Regex.IsMatch(text, @"\d"))
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
