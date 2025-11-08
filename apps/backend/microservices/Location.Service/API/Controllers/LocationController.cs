using Location.Service.Application.Commands;
using Location.Service.Application.DTOs;
using Location.Service.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pogo.Shared.Kernel;

namespace Location.Service.API.Controllers;

/// <summary>
/// Controller for location operations
/// </summary>
[ApiController]
[Route("api/v1/locations")]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new location
    /// </summary>
    /// <param name="request">Location creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created location information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto request, CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand
        {
            Name = request.Name,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            LocationType = request.LocationType,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetLocationById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing location
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="request">Location update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated location information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand
        {
            Id = id,
            Name = request.Name,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            IsActive = request.IsActive,
            LocationType = request.LocationType,
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
    /// Deactivates a location
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateLocation(int id, CancellationToken cancellationToken)
    {
        var command = new DeactivateLocationCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets location by ID
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById(int id, CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery { Id = id };
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
    /// Gets locations by name
    /// </summary>
    /// <param name="name">Location name</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of locations</returns>
    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetLocationsByName(string name, [FromQuery] bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = new GetLocationsByNameQuery { Name = name, ActiveOnly = activeOnly };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets locations by type
    /// </summary>
    /// <param name="locationType">Location type</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of locations</returns>
    [HttpGet("by-type/{locationType}")]
    public async Task<IActionResult> GetLocationsByType(
        string locationType,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLocationsByTypeQuery
        {
            LocationType = locationType,
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
    /// Searches for locations nearby
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="locationType">Optional location type filter</param>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of locations with distances</returns>
    [HttpGet("search/nearby")]
    public async Task<IActionResult> SearchLocationsNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0,
        [FromQuery] string? locationType = null,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchLocationsNearbyQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            LocationType = locationType,
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

    /// <summary>
    /// Gets all locations
    /// </summary>
    /// <param name="activeOnly">Whether to return only active locations</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of locations</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllLocations(
        [FromQuery] bool activeOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllLocationsQuery
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
}
