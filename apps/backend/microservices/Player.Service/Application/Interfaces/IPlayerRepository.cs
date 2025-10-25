using Player.Service.Domain.Entities;
using PlayerEntity = Player.Service.Domain.Entities.Player;

namespace Player.Service.Application.Interfaces;

/// <summary>
/// Repository interface for Player entity
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Gets a player by ID
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player entity or null if not found</returns>
    Task<PlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a player by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player entity or null if not found</returns>
    Task<PlayerEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a player by Discord user ID
    /// </summary>
    /// <param name="discordUserId">Discord user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player entity or null if not found</returns>
    Task<PlayerEntity?> GetByDiscordIdAsync(string discordUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all players with pagination
    /// </summary>
    /// <param name="activeOnly">Whether to return only active players</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of player entities</returns>
    Task<IEnumerable<PlayerEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new player
    /// </summary>
    /// <param name="player">Player entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(PlayerEntity player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player
    /// </summary>
    /// <param name="player">Player entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(PlayerEntity player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
