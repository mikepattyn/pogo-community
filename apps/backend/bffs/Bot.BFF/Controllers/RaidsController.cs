using Bot.BFF.Dtos;
using Bot.BFF.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bot.BFF.Controllers;

/// <summary>
/// Facade endpoints to orchestrate raid persistence on behalf of the Discord bot.
/// </summary>
[ApiController]
[Route("api/raid/v1/raids")]
public class RaidsController : ControllerBase
{
    private readonly IRaidServiceClient _raidServiceClient;
    private readonly ILogger<RaidsController> _logger;

    public RaidsController(IRaidServiceClient raidServiceClient, ILogger<RaidsController> logger)
    {
        _raidServiceClient = raidServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a raid using the Discord message identifier.
    /// </summary>
    [HttpGet("by-message/{messageId}")]
    public async Task<IActionResult> GetByDiscordMessageId(string messageId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(messageId))
        {
            return BadRequest("Message ID is required.");
        }

        var raid = await _raidServiceClient.GetByDiscordMessageIdAsync(messageId, cancellationToken);
        if (raid is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(raid));
    }

    /// <summary>
    /// Creates a raid record using OCR-parsed metadata.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(RaidCreationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var existing = await _raidServiceClient.GetByDiscordMessageIdAsync(request.DiscordMessageId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Raid creation skipped because message {MessageId} already exists.", request.DiscordMessageId);
            return Conflict(new
            {
                message = "Raid already exists for this Discord message.",
                raid = MapToResponse(existing)
            });
        }

        var createRequest = new RaidServiceCreateRequest
        {
            DiscordMessageId = request.DiscordMessageId,
            GymId = request.GymId,
            PokemonSpecies = request.PokemonSpecies,
            Level = request.Level,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxParticipants = request.MaxParticipants,
            Difficulty = request.Difficulty,
            WeatherBoost = request.WeatherBoost,
            Notes = request.Notes
        };

        var created = await _raidServiceClient.CreateAsync(createRequest, cancellationToken);
        _logger.LogInformation("Raid created via Bot BFF for message {MessageId} with backend ID {RaidId}", request.DiscordMessageId, created.Id);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetByDiscordMessageId), new { messageId = response.DiscordMessageId }, response);
    }

    /// <summary>
    /// Adds a participant to a raid tracked by Discord message ID.
    /// </summary>
    [HttpPost("by-message/{messageId}/join")]
    public async Task<IActionResult> JoinRaid(string messageId, RaidJoinRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(messageId))
        {
            return BadRequest("Message ID is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _raidServiceClient.JoinRaidAsync(messageId, new RaidServiceJoinRequest
        {
            PlayerId = request.PlayerId
        }, cancellationToken);

        _logger.LogInformation("Player {PlayerId} joined raid {MessageId} via Bot BFF.", request.PlayerId, messageId);
        return Ok();
    }

    private static RaidProfileResponse MapToResponse(RaidServiceRaidResponse raid)
    {
        return new RaidProfileResponse
        {
            Id = raid.Id,
            DiscordMessageId = raid.DiscordMessageId,
            GymId = raid.GymId,
            PokemonSpecies = raid.PokemonSpecies,
            Level = raid.Level,
            StartTime = raid.StartTime,
            EndTime = raid.EndTime,
            IsActive = raid.IsActive,
            IsCompleted = raid.IsCompleted,
            IsCancelled = raid.IsCancelled,
            MaxParticipants = raid.MaxParticipants,
            CurrentParticipants = raid.CurrentParticipants,
            Difficulty = raid.Difficulty,
            WeatherBoost = raid.WeatherBoost,
            Notes = raid.Notes,
            CreatedAt = raid.CreatedAt,
            UpdatedAt = raid.UpdatedAt
        };
    }
}
