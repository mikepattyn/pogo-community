using Discord;
using Discord.Commands;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Logging;

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
            System.Text.RegularExpressions.Regex tierRegex = new(@"T([1-5])", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var tierMatch = tierRegex.Match(args);
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

                if (scanResponse.RaidData == null || string.IsNullOrEmpty(scanResponse.RaidData.PokemonName))
                {
                    await processingMessage.ModifyAsync(m => m.Content = "❌ No data could be extracted from the image. Please try with a clearer image.");
                    return;
                }

                // Use structured data from OCR service
                var raidInfo = new RaidScanResult
                {
                    Tier = scanResponse.RaidData.Tier > 0 ? scanResponse.RaidData.Tier : tier,
                    PokemonName = scanResponse.RaidData.PokemonName,
                    GymName = scanResponse.RaidData.GymName,
                    TimeInfo = scanResponse.RaidData.TimeRemaining,
                    IsHatched = !string.IsNullOrEmpty(scanResponse.RaidData.PokemonName) &&
                                scanResponse.RaidData.PokemonName.ToLower() != "unknown"
                };

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

    private class RaidScanResult
    {
        public int Tier { get; set; }
        public string PokemonName { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string TimeInfo { get; set; } = string.Empty;
        public bool IsHatched { get; set; }
    }
}
