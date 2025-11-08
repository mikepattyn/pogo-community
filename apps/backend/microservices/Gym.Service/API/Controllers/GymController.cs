using Gym.Service.Application.Commands;
using Gym.Service.Application.DTOs;
using Gym.Service.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pogo.Shared.Kernel;

namespace Gym.Service.API.Controllers;

/// <summary>
/// Controller for gym operations
/// </summary>
[ApiController]
[Route("api/v1/gyms")]
public class GymController : ControllerBase
{
    private readonly IMediator _mediator;

    public GymController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new gym
    /// </summary>
    /// <param name="request">Gym creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created gym information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateGym([FromBody] CreateGymDto request, CancellationToken cancellationToken)
    {
        var command = new CreateGymCommand
        {
            Name = request.Name,
            LocationId = request.LocationId,
            Level = request.Level,
            ControllingTeam = request.ControllingTeam,
            MotivationLevel = request.MotivationLevel,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetGymById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing gym
    /// </summary>
    /// <param name="id">Gym ID</param>
    /// <param name="request">Gym update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated gym information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGym(int id, [FromBody] UpdateGymDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateGymCommand
        {
            Id = id,
            Name = request.Name,
            LocationId = request.LocationId,
            Level = request.Level,
            ControllingTeam = request.ControllingTeam,
            IsUnderAttack = request.IsUnderAttack,
            IsInRaid = request.IsInRaid,
            MotivationLevel = request.MotivationLevel,
            IsActive = request.IsActive,
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
    /// Updates gym status
    /// </summary>
    /// <param name="id">Gym ID</param>
    /// <param name="request">Gym status update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateGymStatus(int id, [FromBody] UpdateGymStatusCommand request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Deactivates a gym
    /// </summary>
    /// <param name="id">Gym ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateGym(int id, CancellationToken cancellationToken)
    {
        var command = new DeactivateGymCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets gym by ID
    /// </summary>
    /// <param name="id">Gym ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Gym information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGymById(int id, CancellationToken cancellationToken)
    {
        var query = new GetGymByIdQuery { Id = id };
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
    /// Gets all gyms
    /// </summary>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gyms</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllGyms(
        [FromQuery] bool activeOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllGymsQuery
        {
            ActiveOnly = activeOnly,
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
    /// Gets gyms by location ID
    /// </summary>
    /// <param name="locationId">Location ID</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gyms</returns>
    [HttpGet("by-location/{locationId}")]
    public async Task<IActionResult> GetGymsByLocation(int locationId, [FromQuery] bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = new GetGymsByLocationQuery { LocationId = locationId, ActiveOnly = activeOnly };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets gyms by controlling team
    /// </summary>
    /// <param name="team">Controlling team</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gyms</returns>
    [HttpGet("by-team/{team}")]
    public async Task<IActionResult> GetGymsByTeam(
        string team,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGymsByTeamQuery
        {
            Team = team,
            ActiveOnly = activeOnly,
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
    /// Searches for gyms nearby
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="activeOnly">Whether to return only active gyms</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of gyms with distances</returns>
    [HttpGet("search/nearby")]
    public async Task<IActionResult> SearchGymsNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchGymsNearbyQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
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

