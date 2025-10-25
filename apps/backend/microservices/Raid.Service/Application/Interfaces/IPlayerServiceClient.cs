namespace Raid.Service.Application.Interfaces;

/// <summary>
/// Client interface for Player service
/// </summary>
public interface IPlayerServiceClient
{
    /// <summary>
    /// Gets player information by ID
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player information or null if not found</returns>
    Task<PlayerInfoDto?> GetPlayerByIdAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple players by IDs
    /// </summary>
    /// <param name="playerIds">Player IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of player ID to player information</returns>
    Task<Dictionary<int, PlayerInfoDto>> GetPlayersByIdsAsync(IEnumerable<int> playerIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// Player information DTO from Player service
/// </summary>
public class PlayerInfoDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string? FriendCode { get; set; }
    public string? DiscordId { get; set; }
    public bool IsActive { get; set; }
}
