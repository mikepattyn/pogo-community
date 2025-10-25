using Player.Service.Application.DTOs;
using Player.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Player.Service.Application.Queries;

/// <summary>
/// Handler for getting player by ID
/// </summary>
public class GetPlayerByIdQueryHandler : QueryHandler<GetPlayerByIdQuery, PlayerDto?>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerByIdQueryHandler(
        IPlayerRepository playerRepository,
        ILogger<GetPlayerByIdQueryHandler> logger) : base(logger)
    {
        _playerRepository = playerRepository;
    }

    protected override async Task<Result<PlayerDto?>> HandleQuery(GetPlayerByIdQuery request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (player == null)
        {
            return Result<PlayerDto?>.Success(null);
        }

        var dto = MapToDto(player);
        return Result<PlayerDto?>.Success(dto);
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
