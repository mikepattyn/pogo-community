using Player.Service.Application.DTOs;
using Player.Service.Application.Interfaces;
using Player.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Player.Service.Application.Commands;

/// <summary>
/// Handler for creating a new player
/// </summary>
public class CreatePlayerCommandHandler : CommandHandler<CreatePlayerCommand, PlayerDto>
{
    private readonly IPlayerRepository _playerRepository;

    public CreatePlayerCommandHandler(
        IPlayerRepository playerRepository,
        ILogger<CreatePlayerCommandHandler> logger) : base(logger)
    {
        _playerRepository = playerRepository;
    }

    protected override async Task<Result<PlayerDto>> HandleCommand(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        // Check if player with username already exists
        var existingPlayer = await _playerRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingPlayer != null)
        {
            return Result<PlayerDto>.Failure("A player with this username already exists");
        }

        // Check if player with Discord ID already exists
        if (!string.IsNullOrEmpty(request.DiscordUserId))
        {
            var existingDiscordPlayer = await _playerRepository.GetByDiscordIdAsync(request.DiscordUserId, cancellationToken);
            if (existingDiscordPlayer != null)
            {
                return Result<PlayerDto>.Failure("A player with this Discord ID already exists");
            }
        }

        // Create new player
        var player = new Domain.Entities.Player
        {
            Username = request.Username,
            Level = request.Level,
            Team = request.Team,
            FriendCode = request.FriendCode,
            Timezone = request.Timezone,
            Language = request.Language,
            DiscordUserId = request.DiscordUserId,
            IsActive = true,
            LastActivity = DateTime.UtcNow
        };

        await _playerRepository.AddAsync(player, cancellationToken);
        await _playerRepository.SaveChangesAsync(cancellationToken);

        return Result<PlayerDto>.Success(MapToDto(player));
    }

    private static PlayerDto MapToDto(Domain.Entities.Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            Username = player.Username,
            Level = player.Level,
            Team = player.Team,
            FriendCode = player.FriendCode,
            IsActive = player.IsActive,
            Timezone = player.Timezone,
            Language = player.Language,
            DiscordUserId = player.DiscordUserId,
            LastActivity = player.LastActivity,
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt
        };
    }
}
