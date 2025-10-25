using Pogo.Shared.Kernel;

namespace Player.Service.Domain.Entities;

/// <summary>
/// Player entity representing a Pokémon GO player
/// </summary>
public class Player : BaseEntity
{
    /// <summary>
    /// Player's username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Player's level
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Player's team (Valor, Mystic, Instinct)
    /// </summary>
    public string Team { get; set; } = string.Empty;

    /// <summary>
    /// Player's friend code
    /// </summary>
    public string FriendCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether the player is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Player's timezone
    /// </summary>
    public string Timezone { get; set; } = "UTC";

    /// <summary>
    /// Player's preferred language
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Player's Discord user ID (if connected)
    /// </summary>
    public string? DiscordUserId { get; set; }

    /// <summary>
    /// Player's last activity date
    /// </summary>
    public DateTime? LastActivity { get; set; }

    /// <summary>
    /// Updates the player's last activity
    /// </summary>
    public void UpdateLastActivity()
    {
        LastActivity = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Activates the player
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdateLastActivity();
    }

    /// <summary>
    /// Deactivates the player
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
