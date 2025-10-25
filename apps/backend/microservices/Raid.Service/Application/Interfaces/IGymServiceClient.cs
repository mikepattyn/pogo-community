namespace Raid.Service.Application.Interfaces;

/// <summary>
/// Client interface for Gym service
/// </summary>
public interface IGymServiceClient
{
    /// <summary>
    /// Gets gym information by ID
    /// </summary>
    /// <param name="gymId">Gym ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Gym information or null if not found</returns>
    Task<GymInfoDto?> GetGymByIdAsync(int gymId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple gyms by IDs
    /// </summary>
    /// <param name="gymIds">Gym IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of gym ID to gym information</returns>
    Task<Dictionary<int, GymInfoDto>> GetGymsByIdsAsync(IEnumerable<int> gymIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// Gym information DTO from Gym service
/// </summary>
public class GymInfoDto
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
}
