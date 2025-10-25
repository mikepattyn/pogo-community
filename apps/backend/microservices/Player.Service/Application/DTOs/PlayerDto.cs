namespace Player.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for Player information
/// </summary>
public class PlayerDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? DiscordUserId { get; set; }
    public DateTime? LastActivity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new player
/// </summary>
public class CreatePlayerDto
{
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string? DiscordUserId { get; set; }
}

/// <summary>
/// Data Transfer Object for updating a player
/// </summary>
public class UpdatePlayerDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? DiscordUserId { get; set; }
}
