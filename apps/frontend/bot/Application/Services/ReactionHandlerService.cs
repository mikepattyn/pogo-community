using Discord;
using Discord.WebSocket;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class ReactionHandlerService
{
    private readonly ILogger<ReactionHandlerService> _logger;
    private readonly IRaidService _raidService;
    private readonly IPlayerService _playerService;

    public ReactionHandlerService(
        ILogger<ReactionHandlerService> logger,
        IRaidService raidService,
        IPlayerService playerService)
    {
        _logger = logger;
        _raidService = raidService;
        _playerService = playerService;
    }

    public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value?.IsBot == true)
            return;

        var emojiName = reaction.Emote.Name;
        var userId = reaction.User.Value?.Id.ToString() ?? "";
        var userName = reaction.User.Value?.Username ?? "Unknown";

        try
        {
            // Handle raid reactions
            var allowedEmojisRaid = new[] { "👍" };
            var allowedEmojisRaidExtra = new[] { "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣" };
            var allowedEmojisRank = new[] { "🔴", "🔵", "🟡", "⚪" };

            if (allowedEmojisRaid.Contains(emojiName))
            {
                await HandleJoiningRaidAsync(message.Id.ToString(), userId, userName);
            }
            else if (allowedEmojisRaidExtra.Contains(emojiName))
            {
                var extraCount = Array.IndexOf(allowedEmojisRaidExtra, emojiName) + 1;
                await HandleAddingExtraAsync(message.Id.ToString(), userId, extraCount);
            }
            else if (allowedEmojisRank.Contains(emojiName))
            {
                await HandleJoiningRankAsync(message.Id.ToString(), userId, emojiName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reaction added: {Emoji} by {User}", emojiName, userName);
        }
    }

    public async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value?.IsBot == true)
            return;

        var emojiName = reaction.Emote.Name;
        var userId = reaction.User.Value?.Id.ToString() ?? "";
        var userName = reaction.User.Value?.Username ?? "Unknown";

        try
        {
            // Handle raid reactions
            var allowedEmojisRaid = new[] { "👍" };
            var allowedEmojisRaidExtra = new[] { "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣" };
            var allowedEmojisRank = new[] { "🔴", "🔵", "🟡", "⚪" };

            if (allowedEmojisRaid.Contains(emojiName))
            {
                await HandleLeavingRaidAsync(message.Id.ToString(), userId, userName);
            }
            else if (allowedEmojisRaidExtra.Contains(emojiName))
            {
                await HandleRemovingExtraAsync(message.Id.ToString(), userId);
            }
            else if (allowedEmojisRank.Contains(emojiName))
            {
                await HandleLeavingRankAsync(message.Id.ToString(), userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reaction removed: {Emoji} by {User}", emojiName, userName);
        }
    }

    private async Task HandleJoiningRaidAsync(string messageId, string userId, string userName)
    {
        try
        {
            await _raidService.AddPlayerToRaidAsync(messageId, userId, userName);
            _logger.LogInformation("User {UserName} joined raid {MessageId}", userName, messageId);
            
            // Update raid embed with new participant
            await UpdateRaidEmbedAsync(messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling raid join: {User} for raid {MessageId}", userName, messageId);
        }
    }

    private async Task HandleLeavingRaidAsync(string messageId, string userId, string userName)
    {
        try
        {
            await _raidService.RemovePlayerFromRaidAsync(messageId, userId);
            _logger.LogInformation("User {UserName} left raid {MessageId}", userName, messageId);
            
            // Update raid embed
            await UpdateRaidEmbedAsync(messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling raid leave: {User} for raid {MessageId}", userName, messageId);
        }
    }

    private async Task HandleAddingExtraAsync(string messageId, string userId, int extraCount)
    {
        try
        {
            await _raidService.AddPlayerExtraAsync(messageId, userId, extraCount);
            _logger.LogInformation("User {UserId} added {ExtraCount} extra players to raid {MessageId}", userId, extraCount, messageId);
            
            // Update raid embed
            await UpdateRaidEmbedAsync(messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling extra players: {User} +{ExtraCount} for raid {MessageId}", userId, extraCount, messageId);
        }
    }

    private async Task HandleRemovingExtraAsync(string messageId, string userId)
    {
        try
        {
            await _raidService.AddPlayerExtraAsync(messageId, userId, 0);
            _logger.LogInformation("User {UserId} removed extra players from raid {MessageId}", userId, messageId);
            
            // Update raid embed
            await UpdateRaidEmbedAsync(messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling extra players removal: {User} for raid {MessageId}", userId, messageId);
        }
    }

    private async Task HandleJoiningRankAsync(string messageId, string userId, string emojiName)
    {
        try
        {
            var team = emojiName switch
            {
                "🔴" => "Valor",
                "🔵" => "Mystic", 
                "🟡" => "Instinct",
                "⚪" => "Harmony",
                _ => "Unknown"
            };

            _logger.LogInformation("User {UserId} selected team {Team} for raid {MessageId}", userId, team, messageId);
            
            // Here you could update player's team preference
            // await _playerService.UpdatePlayerTeamAsync(userId, team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling rank selection: {User} {Team} for raid {MessageId}", userId, emojiName, messageId);
        }
    }

    private async Task HandleLeavingRankAsync(string messageId, string userId)
    {
        try
        {
            _logger.LogInformation("User {UserId} removed rank selection for raid {MessageId}", userId, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling rank removal: {User} for raid {MessageId}", userId, messageId);
        }
    }

    private async Task UpdateRaidEmbedAsync(string messageId)
    {
        try
        {
            var raid = await _raidService.GetRaidAsync(messageId);
            if (raid == null) return;

            // Build participant list
            var participantList = new List<string>();
            var totalExtras = 0;

            foreach (var player in raid.Players)
            {
                participantList.Add(player.Name);
                totalExtras += player.Additions;
            }

            var description = $"**Participants:** {participantList.Count}\n";
            if (participantList.Count > 0)
            {
                description += string.Join(", ", participantList);
                if (totalExtras > 0)
                {
                    description += $" (+{totalExtras} extras)";
                }
            }

            description += "\n\nReact with 👍 to join\nReact with 1⃣-9⃣ to add extra players";

            // This would need access to the Discord message to update the embed
            // For now, we'll just log the update
            _logger.LogInformation("Raid embed updated for {MessageId}: {ParticipantCount} participants, {ExtraCount} extras", 
                messageId, participantList.Count, totalExtras);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating raid embed for {MessageId}", messageId);
        }
    }
}
