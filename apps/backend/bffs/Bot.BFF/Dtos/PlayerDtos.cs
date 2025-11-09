using System.ComponentModel.DataAnnotations;

namespace Bot.BFF.Dtos;

/// <summary>
/// Player creation request payload expected from the Discord bot.
/// </summary>
public class PlayerRegistrationRequest
{
    [Required]
    [MinLength(1)]
    public string DiscordId { get; set; } = string.Empty;

    /// <summary>
    /// ISO-8601 timestamp string when the player joined the server.
    /// </summary>
    public string? DateJoined { get; set; }

    public string? FirstName { get; set; }

    /// <summary>
    /// Preferred in-game nickname.
    /// </summary>
    public string? Nickname { get; set; }

    public int? Level { get; set; }

    public string? Team { get; set; }

    public string? FriendCode { get; set; }

    public string? Timezone { get; set; }

    public string? Language { get; set; }
}

/// <summary>
/// Player update request payload expected from the Discord bot.
/// </summary>
public class PlayerUpdateRequest
{
    public string? FirstName { get; set; }

    public string? Nickname { get; set; }

    public int? Level { get; set; }

    public string? Team { get; set; }

    public string? FriendCode { get; set; }

    public string? Timezone { get; set; }

    public string? Language { get; set; }

    public bool? IsActive { get; set; }
}

/// <summary>
/// Player response returned to the Discord bot.
/// </summary>
public class PlayerProfileResponse
{
    public int Id { get; set; }

    public string DiscordId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public int Level { get; set; }

    public string Team { get; set; } = string.Empty;

    public string FriendCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string Timezone { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public DateTime? LastActivity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
