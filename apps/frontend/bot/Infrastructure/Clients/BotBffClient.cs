using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bot.Service.Application.DTOs;
using Bot.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Infrastructure.Clients;

public class BotBffClient : IBotBffClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<BotBffClient> _logger;
    private readonly string _baseUrl;

    public BotBffClient(HttpClient httpClient, ILogger<BotBffClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["BotBff:BaseUrl"] ?? "http://bot-bff:6001";
    }

    public async Task<PlayerProfileDto?> CreatePlayerAsync(CreatePlayerDto player, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/player/v1/players", player, SerializerOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<PlayerProfileDto>(SerializerOptions, cancellationToken);
                _logger.LogInformation("Successfully created player for Discord ID: {DiscordId}", player.DiscordId);
                return created;
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var conflict = await response.Content.ReadFromJsonAsync<PlayerConflictResponse>(SerializerOptions, cancellationToken);
                _logger.LogWarning("Player already existed for Discord ID: {DiscordId}", player.DiscordId);
                return conflict?.Player;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Bot BFF create player failed with status {Status} and body {Body}", response.StatusCode, body);
            response.EnsureSuccessStatusCode();
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create player for Discord ID: {DiscordId}", player.DiscordId);
            throw;
        }
    }

    public async Task<PlayerProfileDto?> GetPlayerAsync(string discordId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/player/v1/players/{discordId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PlayerProfileDto>(SerializerOptions, cancellationToken);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get player for Discord ID: {DiscordId}", discordId);
            throw;
        }
    }

    public async Task<PlayerProfileDto?> UpdatePlayerAsync(string playerId, UpdatePlayerDto player, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/player/v1/players/{playerId}", player, SerializerOptions, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var updated = await response.Content.ReadFromJsonAsync<PlayerProfileDto>(SerializerOptions, cancellationToken);
                _logger.LogInformation("Successfully updated player: {PlayerId}", playerId);
                return updated;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Bot BFF update player failed with status {Status} and body {Body}", response.StatusCode, body);
            response.EnsureSuccessStatusCode();
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update player: {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<RaidResponseDto?> GetRaidByMessageIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/raid/v1/raids/by-message/{messageId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var raid = await response.Content.ReadFromJsonAsync<RaidResponseDto>(SerializerOptions, cancellationToken);
                _logger.LogInformation("Successfully retrieved raid by message ID: {MessageId}", messageId);
                return raid;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get raid by message ID: {MessageId}", messageId);
            throw;
        }
    }

    public async Task<RaidResponseDto> CreateRaidAsync(CreateRaidRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/raid/v1/raids", request, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var raid = await response.Content.ReadFromJsonAsync<RaidResponseDto>(SerializerOptions, cancellationToken);
            _logger.LogInformation("Successfully created raid with message ID: {MessageId}", request.DiscordMessageId);
            return raid ?? throw new InvalidOperationException("Failed to deserialize created raid response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create raid");
            throw;
        }
    }

    public async Task JoinRaidAsync(string messageId, int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var joinRequest = new { PlayerId = playerId };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/raid/v1/raids/by-message/{messageId}/join", joinRequest, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully joined raid {MessageId} as player {PlayerId}", messageId, playerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to join raid {MessageId} as player {PlayerId}", messageId, playerId);
            throw;
        }
    }


    public async Task<ScanImageResponse> ScanImageAsync(ScanImageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ocr/v1/scans", request, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ScanImageResponse>(SerializerOptions, cancellationToken);
            _logger.LogInformation("Successfully scanned image");
            return result ?? new ScanImageResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan image");
            throw;
        }
    }

    private sealed class PlayerConflictResponse
    {
        public string? Message { get; set; }
        public PlayerProfileDto? Player { get; set; }
    }
}
