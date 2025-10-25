namespace Gym.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for Gym information
/// </summary>
public class GymDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; }
    public string? ControllingTeam { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInRaid { get; set; }
    public int MotivationLevel { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new gym
/// </summary>
public class CreateGymDto
{
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; } = 1;
    public string? ControllingTeam { get; set; }
    public int MotivationLevel { get; set; } = 100;
    public string? Notes { get; set; }
}

/// <summary>
/// Data Transfer Object for updating a gym
/// </summary>
public class UpdateGymDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; }
    public string? ControllingTeam { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInRaid { get; set; }
    public int MotivationLevel { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Data Transfer Object for gym search with location information
/// </summary>
public class GymSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; }
    public string? ControllingTeam { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInRaid { get; set; }
    public int MotivationLevel { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Location information from Location service
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}
