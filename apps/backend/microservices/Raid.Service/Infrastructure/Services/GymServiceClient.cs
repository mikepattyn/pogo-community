using Raid.Service.Application.Interfaces;
using System.Text.Json;

namespace Raid.Service.Infrastructure.Services;

/// <summary>
/// HTTP client for Gym service
/// </summary>
public class GymServiceClient : IGymServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GymServiceClient> _logger;

    public GymServiceClient(HttpClient httpClient, ILogger<GymServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GymInfoDto?> GetGymByIdAsync(int gymId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/gym/{gymId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var gym = JsonSerializer.Deserialize<GymInfoDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return gym;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning("Failed to get gym {GymId}: {StatusCode}", gymId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gym {GymId}", gymId);
            return null;
        }
    }

    public async Task<Dictionary<int, GymInfoDto>> GetGymsByIdsAsync(IEnumerable<int> gymIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<int, GymInfoDto>();

        // For now, get gyms one by one
        // In a real implementation, you might want to batch these requests
        foreach (var gymId in gymIds)
        {
            var gym = await GetGymByIdAsync(gymId, cancellationToken);
            if (gym != null)
            {
                result[gymId] = gym;
            }
        }

        return result;
    }
}
