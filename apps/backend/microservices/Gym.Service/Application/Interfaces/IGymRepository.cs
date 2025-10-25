using Gym.Service.Domain.Entities;
using GymEntity = Gym.Service.Domain.Entities.Gym;

namespace Gym.Service.Application.Interfaces;

/// <summary>
/// Repository interface for Gym entity
/// </summary>
public interface IGymRepository
{
    /// <summary>
    /// Gets a gym by ID
    /// </summary>
    /// <param name="id">Gym ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Gym entity or null if not found</returns>
    Task<GymEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets gyms by location ID
    /// </summary>
    /// <param name="locationId">Location ID</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gym entities</returns>
    Task<IEnumerable<GymEntity>> GetByLocationIdAsync(int locationId, bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets gyms by controlling team
    /// </summary>
    /// <param name="team">Controlling team</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gym entities</returns>
    Task<IEnumerable<GymEntity>> GetByTeamAsync(string team, bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets gyms by location IDs
    /// </summary>
    /// <param name="locationIds">Location IDs</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gym entities</returns>
    Task<IEnumerable<GymEntity>> GetByLocationIdsAsync(IEnumerable<int> locationIds, bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all gyms with pagination
    /// </summary>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gym entities</returns>
    Task<IEnumerable<GymEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new gym
    /// </summary>
    /// <param name="gym">Gym entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(GymEntity gym, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing gym
    /// </summary>
    /// <param name="gym">Gym entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(GymEntity gym, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
