namespace Gym.Service.Application.Interfaces;

/// <summary>
/// Client interface for Location service
/// </summary>
public interface ILocationServiceClient
{
    /// <summary>
    /// Gets location information by ID
    /// </summary>
    /// <param name="locationId">Location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location information or null if not found</returns>
    Task<LocationInfoDto?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple locations by IDs
    /// </summary>
    /// <param name="locationIds">Location IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of location ID to location information</returns>
    Task<Dictionary<int, LocationInfoDto>> GetLocationsByIdsAsync(IEnumerable<int> locationIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// Location information DTO from Location service
/// </summary>
public class LocationInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; }
}
