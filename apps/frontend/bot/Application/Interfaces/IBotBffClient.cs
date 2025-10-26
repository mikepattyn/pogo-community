using Bot.Service.Application.DTOs;

namespace Bot.Service.Application.Interfaces;

public interface IBotBffClient
{
    Task CreatePlayerAsync(CreatePlayerDto player, CancellationToken cancellationToken = default);
    Task<object?> GetPlayerAsync(string discordId, CancellationToken cancellationToken = default);
    Task UpdatePlayerAsync(string playerId, UpdatePlayerDto player, CancellationToken cancellationToken = default);
    Task CreateRaidAsync(object raidData, CancellationToken cancellationToken = default);
    Task<object?> GetRaidsAsync(CancellationToken cancellationToken = default);
    Task UpdateRaidAsync(string raidId, object raidData, CancellationToken cancellationToken = default);
    Task<ScanImageResponse> ScanImageAsync(ScanImageRequest request, CancellationToken cancellationToken = default);
}