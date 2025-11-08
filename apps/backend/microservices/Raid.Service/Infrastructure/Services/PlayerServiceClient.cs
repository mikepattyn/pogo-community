using Raid.Service.Application.Interfaces;
using System.Text.Json;

namespace Raid.Service.Infrastructure.Services;

/// <summary>
/// HTTP client for Player service
/// </summary>
public class PlayerServiceClient : IPlayerServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlayerServiceClient> _logger;

    public PlayerServiceClient(HttpClient httpClient, ILogger<PlayerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PlayerInfoDto?> GetPlayerByIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/player/{playerId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var player = JsonSerializer.Deserialize<PlayerInfoDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return player;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning("Failed to get player {PlayerId}: {StatusCode}", playerId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<Dictionary<int, PlayerInfoDto>> GetPlayersByIdsAsync(IEnumerable<int> playerIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<int, PlayerInfoDto>();

        // For now, get players one by one
        // In a real implementation, you might want to batch these requests
        foreach (var playerId in playerIds)
        {
            var player = await GetPlayerByIdAsync(playerId, cancellationToken);
            if (player != null)
            {
                result[playerId] = player;
            }
        }

        return result;
    }
}
