using Player.Service.Application.DTOs;
using Player.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Player.Service.Application.Queries;

/// <summary>
/// Handler for getting all players
/// </summary>
public class GetAllPlayersQueryHandler : QueryHandler<GetAllPlayersQuery, IEnumerable<PlayerDto>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetAllPlayersQueryHandler(
        IPlayerRepository playerRepository,
        ILogger<GetAllPlayersQueryHandler> logger) : base(logger)
    {
        _playerRepository = playerRepository;
    }

    protected override async Task<Result<IEnumerable<PlayerDto>>> HandleQuery(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        var players = await _playerRepository.GetAllAsync(request.ActiveOnly, request.PageNumber, request.PageSize, cancellationToken);
        
        var dtos = players.Select(MapToDto).ToList();
        return Result<IEnumerable<PlayerDto>>.Success(dtos);
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
