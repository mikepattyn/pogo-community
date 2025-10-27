using Raid.Service.Application.DTOs;
using Raid.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Handler for getting raid by ID
/// </summary>
public class GetRaidByIdQueryHandler : QueryHandler<GetRaidByIdQuery, RaidDto?>
{
    private readonly IRaidRepository _raidRepository;

    public GetRaidByIdQueryHandler(
        IRaidRepository raidRepository,
        ILogger<GetRaidByIdQueryHandler> logger) : base(logger)
    {
        _raidRepository = raidRepository;
    }

    protected override async Task<Result<RaidDto?>> HandleQuery(GetRaidByIdQuery request, CancellationToken cancellationToken)
    {
        var raid = await _raidRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (raid == null)
        {
            return Result<RaidDto?>.Success(null);
        }

        var dto = MapToDto(raid);
        return Result<RaidDto?>.Success(dto);
    }

    private static RaidDto MapToDto(Domain.Entities.Raid raid)
    {
        return new RaidDto
        {
            Id = raid.Id,
            DiscordMessageId = raid.DiscordMessageId,
            GymId = raid.GymId,
            PokemonSpecies = raid.PokemonSpecies,
            Level = raid.Level,
            StartTime = raid.StartTime,
            EndTime = raid.EndTime,
            IsActive = raid.IsActive,
            IsCompleted = raid.IsCompleted,
            IsCancelled = raid.IsCancelled,
            MaxParticipants = raid.MaxParticipants,
            CurrentParticipants = raid.CurrentParticipants,
            Difficulty = raid.Difficulty,
            WeatherBoost = raid.WeatherBoost,
            Notes = raid.Notes,
            CreatedAt = raid.CreatedAt,
            UpdatedAt = raid.UpdatedAt
        };
    }
}

