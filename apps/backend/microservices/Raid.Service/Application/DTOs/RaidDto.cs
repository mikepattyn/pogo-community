namespace Raid.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for Raid information
/// </summary>
public class RaidDto
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
/// Data Transfer Object for creating a new raid
/// </summary>
public class CreateRaidDto
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

/// <summary>
/// Data Transfer Object for updating a raid
/// </summary>
public class UpdateRaidDto
{
    public int Id { get; set; }
    public string? DiscordMessageId { get; set; }
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCancelled { get; set; }
    public int MaxParticipants { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Data Transfer Object for raid search with gym and location information
/// </summary>
public class RaidSearchDto
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
    
    // Gym information from Gym service
    public string? GymName { get; set; }
    public int? GymLevel { get; set; }
    public string? ControllingTeam { get; set; }
    public bool? IsGymUnderAttack { get; set; }
    
    // Location information from Location service
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// Data Transfer Object for raid participation
/// </summary>
public class RaidParticipationDto
{
    public int RaidId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Data Transfer Object for joining a raid
/// </summary>
public class JoinRaidDto
{
    public int PlayerId { get; set; }
}
