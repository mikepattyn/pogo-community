using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Bot.BFF.Services;

public interface IRaidServiceClient
{
    Task<RaidServiceRaidResponse?> GetByDiscordMessageIdAsync(string discordMessageId, CancellationToken cancellationToken);
    Task<RaidServiceRaidResponse> CreateAsync(RaidServiceCreateRequest request, CancellationToken cancellationToken);
    Task JoinRaidAsync(string discordMessageId, RaidServiceJoinRequest request, CancellationToken cancellationToken);
}

internal sealed class RaidServiceClient : IRaidServiceClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<RaidServiceClient> _logger;

    public RaidServiceClient(HttpClient httpClient, ILogger<RaidServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RaidServiceRaidResponse?> GetByDiscordMessageIdAsync(string discordMessageId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(discordMessageId))
        {
            return null;
        }

        var response = await _httpClient.GetAsync($"/api/v1/raids/by-message/{Uri.EscapeDataString(discordMessageId)}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Raid service returned non-success status {StatusCode} for Discord message {DiscordMessageId}: {Body}",
                response.StatusCode,
                discordMessageId,
                body);
            throw new HttpRequestException($"Raid service error ({response.StatusCode}) while fetching raid.");
        }

        return await response.Content.ReadFromJsonAsync<RaidServiceRaidResponse>(SerializerOptions, cancellationToken);
    }

    public async Task<RaidServiceRaidResponse> CreateAsync(RaidServiceCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/raids", request, SerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Raid service failed to create raid for Discord message {DiscordMessageId}. Status: {StatusCode}. Body: {Body}",
                request.DiscordMessageId,
                response.StatusCode,
                body);
            throw new HttpRequestException($"Raid service error ({response.StatusCode}) while creating raid.");
        }

        var created = await response.Content.ReadFromJsonAsync<RaidServiceRaidResponse>(SerializerOptions, cancellationToken);

        if (created is null)
        {
            throw new InvalidOperationException("Raid service returned an empty payload when creating a raid.");
        }

        return created;
    }

    public async Task JoinRaidAsync(string discordMessageId, RaidServiceJoinRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/raids/by-message/{Uri.EscapeDataString(discordMessageId)}/join", request, SerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Raid service failed to join raid for Discord message {DiscordMessageId}. Status: {StatusCode}. Body: {Body}",
                discordMessageId,
                response.StatusCode,
                body);
            throw new HttpRequestException($"Raid service error ({response.StatusCode}) while joining raid.");
        }
    }
}

#region Service DTOs

public sealed class RaidServiceCreateRequest
{
    public string DiscordMessageId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public string Difficulty { get; set; } = "Medium";
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
}

public sealed class RaidServiceJoinRequest
{
    public int PlayerId { get; set; }
}

public sealed class RaidServiceRaidResponse
{
    public int Id { get; set; }
    public string DiscordMessageId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCancelled { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

#endregion
