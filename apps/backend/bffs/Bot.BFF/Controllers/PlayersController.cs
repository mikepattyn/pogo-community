using Bot.BFF.Dtos;
using Bot.BFF.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bot.BFF.Controllers;

/// <summary>
/// Facade endpoints for player registration and profile management.
/// </summary>
[ApiController]
[Route("api/player/v1/players")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(IPlayerServiceClient playerServiceClient, ILogger<PlayersController> logger)
    {
        _playerServiceClient = playerServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a player profile by Discord user ID.
    /// </summary>
    [HttpGet("{discordId}")]
    public async Task<IActionResult> GetByDiscordId(string discordId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(discordId))
        {
            return BadRequest("Discord ID is required.");
        }

        var player = await _playerServiceClient.GetByDiscordIdAsync(discordId, cancellationToken);
        if (player is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(player));
    }

    /// <summary>
    /// Registers a player using Discord-sourced metadata.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(PlayerRegistrationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var existing = await _playerServiceClient.GetByDiscordIdAsync(request.DiscordId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Player registration skipped because Discord ID {DiscordId} already exists.", request.DiscordId);
            return Conflict(new
            {
                message = "Player already registered.",
                player = MapToResponse(existing)
            });
        }

        var username = ResolveUsername(request);

        var createRequest = new PlayerServiceCreateRequest
        {
            Username = username,
            Level = request.Level ?? 1,
            Team = string.IsNullOrWhiteSpace(request.Team) ? "Unspecified" : request.Team!,
            FriendCode = request.FriendCode ?? string.Empty,
            Timezone = request.Timezone ?? "UTC",
            Language = request.Language ?? "en",
            DiscordUserId = request.DiscordId
        };

        var created = await _playerServiceClient.CreateAsync(createRequest, cancellationToken);
        _logger.LogInformation("Player created via Bot BFF: {DiscordId} mapped to profile {PlayerId}", request.DiscordId, created.Id);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetByDiscordId), new { discordId = response.DiscordId }, response);
    }

    /// <summary>
    /// Updates player profile information sourced from Discord.
    /// </summary>
    [HttpPut("{discordId}")]
    public async Task<IActionResult> Update(string discordId, PlayerUpdateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(discordId))
        {
            return BadRequest("Discord ID is required.");
        }

        var existing = await _playerServiceClient.GetByDiscordIdAsync(discordId, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var username = ResolveUpdatedUsername(request, existing);

        var updateRequest = new PlayerServiceUpdateRequest
        {
            Id = existing.Id,
            Username = username,
            Level = request.Level ?? existing.Level,
            Team = request.Team ?? existing.Team,
            FriendCode = request.FriendCode ?? existing.FriendCode,
            IsActive = request.IsActive ?? existing.IsActive,
            Timezone = request.Timezone ?? existing.Timezone,
            Language = request.Language ?? existing.Language,
            DiscordUserId = existing.DiscordUserId ?? discordId
        };

        var updated = await _playerServiceClient.UpdateAsync(updateRequest, cancellationToken);
        _logger.LogInformation("Player profile updated via Bot BFF: {DiscordId} -> {PlayerId}", discordId, updated.Id);

        return Ok(MapToResponse(updated));
    }

    private static PlayerProfileResponse MapToResponse(PlayerServicePlayerResponse player)
    {
        return new PlayerProfileResponse
        {
            Id = player.Id,
            DiscordId = player.DiscordUserId ?? string.Empty,
            Username = player.Username,
            DisplayName = player.Username,
            Level = player.Level,
            Team = player.Team,
            FriendCode = player.FriendCode,
            IsActive = player.IsActive,
            Timezone = player.Timezone,
            Language = player.Language,
            LastActivity = player.LastActivity,
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt
        };
    }

    private static string ResolveUsername(PlayerRegistrationRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Nickname))
        {
            return request.Nickname!;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            return request.FirstName!;
        }

        return request.DiscordId;
    }

    private static string ResolveUpdatedUsername(PlayerUpdateRequest request, PlayerServicePlayerResponse existing)
    {
        if (!string.IsNullOrWhiteSpace(request.Nickname))
        {
            return request.Nickname!;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            return request.FirstName!;
        }

        return existing.Username;
    }
}
