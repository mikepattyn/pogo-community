using Raid.Service.Domain.Entities;
using RaidEntity = Raid.Service.Domain.Entities.Raid;

namespace Raid.Service.Application.Interfaces;

/// <summary>
/// Repository interface for Raid entity
/// </summary>
public interface IRaidRepository
{
    /// <summary>
    /// Gets a raid by ID
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raid entity or null if not found</returns>
    Task<RaidEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a raid by Discord message ID
    /// </summary>
    /// <param name="messageId">Discord message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raid entity or null if not found</returns>
    Task<RaidEntity?> GetByDiscordMessageIdAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets raids by gym ID
    /// </summary>
    /// <param name="gymId">Gym ID</param>
    /// <param name="activeOnly">Whether to return only active raids</param>
    /// <param name="fromDate">Optional start date filter</param>
    /// <param name="toDate">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raid entities</returns>
    Task<IEnumerable<RaidEntity>> GetByGymIdAsync(int gymId, bool activeOnly = true, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets currently active raids
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raid entities</returns>
    Task<IEnumerable<RaidEntity>> GetActiveRaidsAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets raids by gym IDs
    /// </summary>
    /// <param name="gymIds">Gym IDs</param>
    /// <param name="activeOnly">Whether to return only active raids</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raid entities</returns>
    Task<IEnumerable<RaidEntity>> GetByGymIdsAsync(IEnumerable<int> gymIds, bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all raids with pagination
    /// </summary>
    /// <param name="activeOnly">Whether to return only active raids</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raid entities</returns>
    Task<IEnumerable<RaidEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new raid
    /// </summary>
    /// <param name="raid">Raid entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(RaidEntity raid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing raid
    /// </summary>
    /// <param name="raid">Raid entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(RaidEntity raid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
