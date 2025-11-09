using System.ComponentModel.DataAnnotations;

namespace Bot.BFF.Dtos;

/// <summary>
/// Raid creation request payload expected from the Discord bot.
/// </summary>
public class RaidCreationRequest
{
    [Required]
    [MinLength(1)]
    public string DiscordMessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gym identifier resolved on the backend.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int GymId { get; set; }

    [Required]
    [MinLength(1)]
    public string PokemonSpecies { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Level { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, 50)]
    public int MaxParticipants { get; set; } = 20;

    [Required]
    public string Difficulty { get; set; } = "Medium";

    public string? WeatherBoost { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Raid join request payload expected from the Discord bot.
/// </summary>
public class RaidJoinRequest
{
    [Range(1, int.MaxValue)]
    public int PlayerId { get; set; }
}

/// <summary>
/// Raid representation returned to the Discord bot.
/// </summary>
public class RaidProfileResponse
{
    public int Id { get; set; }

    public string DiscordMessageId { get; set; } = string.Empty;

    public int GymId { get; set; }

    public string PokemonSpecies { get; set; } = string.Empty;

    public int Level { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool IsActive { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsCancelled { get; set; }

    public int MaxParticipants { get; set; }

    public int CurrentParticipants { get; set; }

    public string Difficulty { get; set; } = string.Empty;

    public string? WeatherBoost { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
