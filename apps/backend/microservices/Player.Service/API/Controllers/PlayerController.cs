using Player.Service.Application.Commands;
using Player.Service.Application.DTOs;
using Player.Service.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pogo.Shared.Kernel;

namespace Player.Service.API.Controllers;

/// <summary>
/// Controller for player operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new player
    /// </summary>
    /// <param name="request">Player creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created player information</returns>
    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto request, CancellationToken cancellationToken)
    {
        var command = new CreatePlayerCommand
        {
            Username = request.Username,
            Level = request.Level,
            Team = request.Team,
            FriendCode = request.FriendCode,
            Timezone = request.Timezone,
            Language = request.Language,
            DiscordUserId = request.DiscordUserId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetPlayerById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing player
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="request">Player update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated player information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(int id, [FromBody] UpdatePlayerDto request, CancellationToken cancellationToken)
    {
        var command = new UpdatePlayerCommand
        {
            Id = id,
            Username = request.Username,
            Level = request.Level,
            Team = request.Team,
            FriendCode = request.FriendCode,
            IsActive = request.IsActive,
            Timezone = request.Timezone,
            Language = request.Language,
            DiscordUserId = request.DiscordUserId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deactivates a player
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivatePlayer(int id, CancellationToken cancellationToken)
    {
        var command = new DeactivatePlayerCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets player by ID
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(int id, CancellationToken cancellationToken)
    {
        var query = new GetPlayerByIdQuery { Id = id };
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
    /// Gets player by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player information</returns>
    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetPlayerByUsername(string username, CancellationToken cancellationToken)
    {
        var query = new GetPlayerByUsernameQuery { Username = username };
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
    /// Gets player by Discord user ID
    /// </summary>
    /// <param name="discordUserId">Discord user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Player information</returns>
    [HttpGet("by-discord/{discordUserId}")]
    public async Task<IActionResult> GetPlayerByDiscordId(string discordUserId, CancellationToken cancellationToken)
    {
        var query = new GetPlayerByDiscordIdQuery { DiscordUserId = discordUserId };
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
    /// Gets all players
    /// </summary>
    /// <param name="activeOnly">Whether to return only active players</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of players</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPlayers(
        [FromQuery] bool activeOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllPlayersQuery
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
