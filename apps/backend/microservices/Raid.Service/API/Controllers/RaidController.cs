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
[Route("api/v1/raids")]
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

        return CreatedAtAction(nameof(GetRaidByMessageId), new { messageId = result.Value!.DiscordMessageId }, result.Value);
    }


    /// <summary>
    /// Joins a raid
    /// </summary>
    /// <param name="messageId">Discord message ID</param>
    /// <param name="request">Join raid request with player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("by-message/{messageId}/join")]
    public async Task<IActionResult> JoinRaid(string messageId, [FromBody] JoinRaidDto request, CancellationToken cancellationToken)
    {
        var command = new JoinRaidCommand { MessageId = messageId, PlayerId = request.PlayerId };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Gets raid by Discord message ID
    /// </summary>
    /// <param name="messageId">Discord message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raid information</returns>
    [HttpGet("by-message/{messageId}")]
    public async Task<IActionResult> GetRaidByMessageId(string messageId, CancellationToken cancellationToken)
    {
        var query = new GetRaidByMessageIdQuery { MessageId = messageId };
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

}

