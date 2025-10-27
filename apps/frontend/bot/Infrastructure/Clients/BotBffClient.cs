using Bot.Service.Application.DTOs;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Infrastructure.Clients;

public class BotBffClient : IBotBffClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BotBffClient> _logger;
    private readonly string _baseUrl;

    public BotBffClient(HttpClient httpClient, ILogger<BotBffClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["BotBff:BaseUrl"] ?? "http://bot-bff:6001";
    }

    public async Task CreatePlayerAsync(CreatePlayerDto player, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/player/v1/players", player, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully created player for Discord ID: {DiscordId}", player.DiscordId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create player for Discord ID: {DiscordId}", player.DiscordId);
            throw;
        }
    }

    public async Task<object?> GetPlayerAsync(string discordId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/player/v1/players/{discordId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get player for Discord ID: {DiscordId}", discordId);
            throw;
        }
    }

    public async Task UpdatePlayerAsync(string playerId, UpdatePlayerDto player, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/player/v1/players/{playerId}", player, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully updated player: {PlayerId}", playerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update player: {PlayerId}", playerId);
            throw;
        }
    }

    public async Task CreateRaidAsync(object raidData, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/raid/v1/raids", raidData, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully created raid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create raid");
            throw;
        }
    }

    public async Task<object?> GetRaidsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/raid/v1/raids", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get raids");
            throw;
        }
    }

    public async Task UpdateRaidAsync(string raidId, object raidData, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/raid/v1/raids/{raidId}", raidData, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully updated raid: {RaidId}", raidId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update raid: {RaidId}", raidId);
            throw;
        }
    }

    public async Task<ScanImageResponse> ScanImageAsync(ScanImageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ocr/scans", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ScanImageResponse>(cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully scanned image");
            return result ?? new ScanImageResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan image");
            throw;
        }
    }
}
