using Bot.Service.Application.DTOs;

namespace Bot.Service.Application.Interfaces;

public interface IBotBffClient
{
    Task CreatePlayerAsync(CreatePlayerDto player, CancellationToken cancellationToken = default);
    Task<object?> GetPlayerAsync(string discordId, CancellationToken cancellationToken = default);
    Task UpdatePlayerAsync(string playerId, UpdatePlayerDto player, CancellationToken cancellationToken = default);
    
    Task<RaidResponseDto?> GetRaidByMessageIdAsync(string messageId, CancellationToken cancellationToken = default);
    Task<RaidResponseDto> CreateRaidAsync(CreateRaidRequestDto request, CancellationToken cancellationToken = default);
    Task JoinRaidAsync(string messageId, int playerId, CancellationToken cancellationToken = default);
    
    Task<ScanImageResponse> ScanImageAsync(ScanImageRequest request, CancellationToken cancellationToken = default);
}
