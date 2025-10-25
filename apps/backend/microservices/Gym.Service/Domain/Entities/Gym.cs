using Pogo.Shared.Kernel;

namespace Gym.Service.Domain.Entities;

/// <summary>
/// Gym entity representing a Pokemon Gym
/// </summary>
public class Gym : BaseEntity
{
    /// <summary>
    /// Gym name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Location ID (references Location service)
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Gym level (1-6)
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Current controlling team (Valor, Mystic, Instinct, or null if neutral)
    /// </summary>
    public string? ControllingTeam { get; set; }

    /// <summary>
    /// Whether the gym is currently under attack
    /// </summary>
    public bool IsUnderAttack { get; set; } = false;

    /// <summary>
    /// Whether the gym is currently in a raid
    /// </summary>
    public bool IsInRaid { get; set; } = false;

    /// <summary>
    /// Current motivation level (0-100)
    /// </summary>
    public int MotivationLevel { get; set; } = 100;

    /// <summary>
    /// Last time the gym was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the gym is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional notes about the gym
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Updates the gym's controlling team
    /// </summary>
    /// <param name="team">New controlling team</param>
    public void UpdateControllingTeam(string? team)
    {
        ControllingTeam = team;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Updates the gym's level
    /// </summary>
    /// <param name="level">New gym level (1-6)</param>
    public void UpdateLevel(int level)
    {
        if (level < 1 || level > 6)
        {
            throw new ArgumentException("Gym level must be between 1 and 6", nameof(level));
        }

        Level = level;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Updates the gym's motivation level
    /// </summary>
    /// <param name="motivation">New motivation level (0-100)</param>
    public void UpdateMotivation(int motivation)
    {
        if (motivation < 0 || motivation > 100)
        {
            throw new ArgumentException("Motivation level must be between 0 and 100", nameof(motivation));
        }

        MotivationLevel = motivation;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Sets the gym as under attack
    /// </summary>
    public void SetUnderAttack()
    {
        IsUnderAttack = true;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Clears the under attack status
    /// </summary>
    public void ClearUnderAttack()
    {
        IsUnderAttack = false;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Sets the gym as in raid
    /// </summary>
    public void SetInRaid()
    {
        IsInRaid = true;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Clears the in raid status
    /// </summary>
    public void ClearInRaid()
    {
        IsInRaid = false;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Activates the gym
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Deactivates the gym
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastUpdated = DateTime.UtcNow;
        Touch();
    }
}
