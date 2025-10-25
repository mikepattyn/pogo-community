using Gym.Service.Application.Interfaces;
using System.Text.Json;

namespace Gym.Service.Infrastructure.Services;

/// <summary>
/// HTTP client for Location service
/// </summary>
public class LocationServiceClient : ILocationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocationServiceClient> _logger;

    public LocationServiceClient(HttpClient httpClient, ILogger<LocationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LocationInfoDto?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/location/{locationId}", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var location = JsonSerializer.Deserialize<LocationInfoDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return location;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning("Failed to get location {LocationId}: {StatusCode}", locationId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location {LocationId}", locationId);
            return null;
        }
    }

    public async Task<Dictionary<int, LocationInfoDto>> GetLocationsByIdsAsync(IEnumerable<int> locationIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<int, LocationInfoDto>();
        
        // For now, get locations one by one
        // In a real implementation, you might want to batch these requests
        foreach (var locationId in locationIds)
        {
            var location = await GetLocationByIdAsync(locationId, cancellationToken);
            if (location != null)
            {
                result[locationId] = location;
            }
        }

        return result;
    }
}
