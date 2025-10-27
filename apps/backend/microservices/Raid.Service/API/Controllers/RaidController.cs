using Raid.Service.Application.Commands;
using Raid.Service.Application.DTOs;
using Raid.Service.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pogo.Shared.Kernel;

namespace Raid.Service.API.Controllers;

/// <summary>
/// Controller for raid operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RaidController : ControllerBase
{
    private readonly IMediator _mediator;

    public RaidController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new raid
    /// </summary>
    /// <param name="request">Raid creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created raid information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRaid([FromBody] CreateRaidDto request, CancellationToken cancellationToken)
    {
        var command = new CreateRaidCommand
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

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetRaidById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing raid
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="request">Raid update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated raid information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRaid(int id, [FromBody] UpdateRaidDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateRaidCommand
        {
            Id = id,
            GymId = request.GymId,
            PokemonSpecies = request.PokemonSpecies,
            Level = request.Level,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsActive = request.IsActive,
            IsCompleted = request.IsCompleted,
            IsCancelled = request.IsCancelled,
            MaxParticipants = request.MaxParticipants,
            Difficulty = request.Difficulty,
            WeatherBoost = request.WeatherBoost,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Joins a raid
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="playerId">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinRaid(int id, [FromQuery] int playerId, CancellationToken cancellationToken)
    {
        var command = new JoinRaidCommand { RaidId = id, PlayerId = playerId };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Leaves a raid
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="playerId">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveRaid(int id, [FromQuery] int playerId, CancellationToken cancellationToken)
    {
        var command = new LeaveRaidCommand { RaidId = id, PlayerId = playerId };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Completes a raid
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteRaid(int id, CancellationToken cancellationToken)
    {
        var command = new CompleteRaidCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Cancels a raid
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRaid(int id, CancellationToken cancellationToken)
    {
        var command = new CancelRaidCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Gets raid by ID
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raid information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRaidById(int id, CancellationToken cancellationToken)
    {
        var query = new GetRaidByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        if (result.Value == null)
        {
            return NotFound();
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets currently active raids
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active raids</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRaids(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveRaidsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets raids by gym ID
    /// </summary>
    /// <param name="gymId">Gym ID</param>
    /// <param name="activeOnly">Whether to return only active raids</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raids</returns>
    [HttpGet("by-gym/{gymId}")]
    public async Task<IActionResult> GetRaidsByGym(int gymId, [FromQuery] bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = new GetRaidsByGymQuery { GymId = gymId, ActiveOnly = activeOnly };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets raid participants
    /// </summary>
    /// <param name="id">Raid ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raid participants</returns>
    [HttpGet("{id}/participants")]
    public async Task<IActionResult> GetRaidParticipants(int id, CancellationToken cancellationToken)
    {
        var query = new GetRaidParticipantsQuery { RaidId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Searches for raids nearby
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="level">Optional raid level filter</param>
    /// <param name="pokemonSpecies">Optional Pokemon species filter</param>
    /// <param name="activeOnly">Whether to return only active raids</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of raids with location information</returns>
    [HttpGet("search/nearby")]
    public async Task<IActionResult> SearchRaidsNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0,
        [FromQuery] int? level = null,
        [FromQuery] string? pokemonSpecies = null,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchRaidsNearbyQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            Level = level,
            PokemonSpecies = pokemonSpecies,
            ActiveOnly = activeOnly,
            MaxResults = maxResults
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}

