using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Bot.BFF.Services;

public interface IPlayerServiceClient
{
    Task<PlayerServicePlayerResponse?> GetByDiscordIdAsync(string discordId, CancellationToken cancellationToken);
    Task<PlayerServicePlayerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PlayerServicePlayerResponse> CreateAsync(PlayerServiceCreateRequest request, CancellationToken cancellationToken);
    Task<PlayerServicePlayerResponse> UpdateAsync(PlayerServiceUpdateRequest request, CancellationToken cancellationToken);
}

internal sealed class PlayerServiceClient : IPlayerServiceClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<PlayerServiceClient> _logger;

    public PlayerServiceClient(HttpClient httpClient, ILogger<PlayerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PlayerServicePlayerResponse?> GetByDiscordIdAsync(string discordId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(discordId))
        {
            return null;
        }

        var response = await _httpClient.GetAsync($"/api/v1/players/by-discord/{Uri.EscapeDataString(discordId)}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Player service returned non-success status {StatusCode} for Discord ID {DiscordId}: {Body}", response.StatusCode, discordId, body);
            throw new HttpRequestException($"Player service error ({response.StatusCode}) while retrieving player by Discord ID.");
        }

        return await response.Content.ReadFromJsonAsync<PlayerServicePlayerResponse>(SerializerOptions, cancellationToken);
    }

    public async Task<PlayerServicePlayerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return null;
        }

        var response = await _httpClient.GetAsync($"/api/v1/players/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Player service returned non-success status {StatusCode} for player ID {PlayerId}: {Body}", response.StatusCode, id, body);
            throw new HttpRequestException($"Player service error ({response.StatusCode}) while retrieving player by ID.");
        }

        return await response.Content.ReadFromJsonAsync<PlayerServicePlayerResponse>(SerializerOptions, cancellationToken);
    }

    public async Task<PlayerServicePlayerResponse> CreateAsync(PlayerServiceCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/players", request, SerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Player service failed to create player for Discord ID {DiscordId}. Status: {StatusCode}. Body: {Body}",
                request.DiscordUserId,
                response.StatusCode,
                body);
            throw new HttpRequestException($"Player service error ({response.StatusCode}) while creating player.");
        }

        var created = await response.Content.ReadFromJsonAsync<PlayerServicePlayerResponse>(SerializerOptions, cancellationToken);

        if (created is null)
        {
            throw new InvalidOperationException("Player service returned an empty payload when creating a player.");
        }

        return created;
    }

    public async Task<PlayerServicePlayerResponse> UpdateAsync(PlayerServiceUpdateRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/v1/players/{request.Id}", request, SerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Player service failed to update player {PlayerId}. Status: {StatusCode}. Body: {Body}",
                request.Id,
                response.StatusCode,
                body);
            throw new HttpRequestException($"Player service error ({response.StatusCode}) while updating player.");
        }

        var updated = await response.Content.ReadFromJsonAsync<PlayerServicePlayerResponse>(SerializerOptions, cancellationToken);

        if (updated is null)
        {
            throw new InvalidOperationException("Player service returned an empty payload when updating a player.");
        }

        return updated;
    }
}

#region Service DTOs

public sealed class PlayerServiceCreateRequest
{
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string? DiscordUserId { get; set; }
}

public sealed class PlayerServiceUpdateRequest
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? DiscordUserId { get; set; }
}

public sealed class PlayerServicePlayerResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? DiscordUserId { get; set; }
    public DateTime? LastActivity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

#endregion
