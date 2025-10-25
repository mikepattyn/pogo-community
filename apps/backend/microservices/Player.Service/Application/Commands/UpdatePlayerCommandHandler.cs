using Player.Service.Application.DTOs;
using Player.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Player.Service.Application.Commands;

/// <summary>
/// Handler for updating an existing player
/// </summary>
public class UpdatePlayerCommandHandler : CommandHandler<UpdatePlayerCommand, PlayerDto>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdatePlayerCommandHandler(
        IPlayerRepository playerRepository,
        ILogger<UpdatePlayerCommandHandler> logger) : base(logger)
    {
        _playerRepository = playerRepository;
    }

    protected override async Task<Result<PlayerDto>> HandleCommand(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        // Get existing player
        var player = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (player == null)
        {
            return Result<PlayerDto>.Failure("Player not found");
        }

        // Check if username is being changed and if it already exists
        if (player.Username != request.Username)
        {
            var existingPlayer = await _playerRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingPlayer != null && existingPlayer.Id != request.Id)
            {
                return Result<PlayerDto>.Failure("A player with this username already exists");
            }
        }

        // Check if Discord ID is being changed and if it already exists
        if (player.DiscordUserId != request.DiscordUserId && !string.IsNullOrEmpty(request.DiscordUserId))
        {
            var existingDiscordPlayer = await _playerRepository.GetByDiscordIdAsync(request.DiscordUserId, cancellationToken);
            if (existingDiscordPlayer != null && existingDiscordPlayer.Id != request.Id)
            {
                return Result<PlayerDto>.Failure("A player with this Discord ID already exists");
            }
        }

        // Update player properties
        player.Username = request.Username;
        player.Level = request.Level;
        player.Team = request.Team;
        player.FriendCode = request.FriendCode;
        player.IsActive = request.IsActive;
        player.Timezone = request.Timezone;
        player.Language = request.Language;
        player.DiscordUserId = request.DiscordUserId;

        if (request.IsActive)
        {
            player.UpdateLastActivity();
        }

        await _playerRepository.UpdateAsync(player, cancellationToken);
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
