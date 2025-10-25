using Account.Service.Application.Commands;
using Account.Service.Application.DTOs;
using Account.Service.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pogo.Shared.Kernel;

namespace Account.Service.API.Controllers;

/// <summary>
/// Controller for account operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="request">Account creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created account information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto request, CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand
        {
            PlayerId = request.PlayerId,
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetAccountByEmail), new { email = result.Value!.Email }, result.Value);
    }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login response with JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets account by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account information</returns>
    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetAccountByEmail(string email, CancellationToken cancellationToken)
    {
        var query = new GetAccountByEmailQuery { Email = email };
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
    /// Gets account by player ID
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account information</returns>
    [HttpGet("by-player/{playerId}")]
    public async Task<IActionResult> GetAccountByPlayerId(int playerId, CancellationToken cancellationToken)
    {
        var query = new GetAccountByPlayerIdQuery { PlayerId = playerId };
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
