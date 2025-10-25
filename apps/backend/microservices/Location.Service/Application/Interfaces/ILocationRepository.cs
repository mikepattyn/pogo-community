using Location.Service.Domain.Entities;
using LocationEntity = Location.Service.Domain.Entities.Location;

namespace Location.Service.Application.Interfaces;

/// <summary>
/// Repository interface for Location entity
/// </summary>
public interface ILocationRepository
{
    /// <summary>
    /// Gets a location by ID
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location entity or null if not found</returns>
    Task<LocationEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locations by name
    /// </summary>
    /// <param name="name">Location name</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of location entities</returns>
    Task<IEnumerable<LocationEntity>> GetByNameAsync(string name, bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locations by type
    /// </summary>
    /// <param name="locationType">Location type</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of location entities</returns>
    Task<IEnumerable<LocationEntity>> GetByTypeAsync(string locationType, bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for locations near a specific point
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="locationType">Optional location type filter</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of location entities</returns>
    Task<IEnumerable<LocationEntity>> SearchNearbyAsync(double latitude, double longitude, double radiusKm, string? locationType = null, bool activeOnly = true, int maxResults = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all locations with pagination
    /// </summary>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of location entities</returns>
    Task<IEnumerable<LocationEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new location
    /// </summary>
    /// <param name="location">Location entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(LocationEntity location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing location
    /// </summary>
    /// <param name="location">Location entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(LocationEntity location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
