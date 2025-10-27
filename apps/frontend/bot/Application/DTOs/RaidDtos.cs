namespace Bot.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for raid response from Raid Service
/// </summary>
public class RaidResponseDto
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

/// <summary>
/// Data Transfer Object for creating a new raid via BFF
/// </summary>
public class CreateRaidRequestDto
{
    public string DiscordMessageId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public string Difficulty { get; set; } = "Medium";
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
}

