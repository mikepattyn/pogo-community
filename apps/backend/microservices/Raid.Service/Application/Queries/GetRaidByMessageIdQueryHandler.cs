using Raid.Service.Application.DTOs;
using Raid.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Handler for getting raid by Discord message ID
/// </summary>
public class GetRaidByMessageIdQueryHandler : QueryHandler<GetRaidByMessageIdQuery, RaidDto?>
{
    private readonly IRaidRepository _raidRepository;

    public GetRaidByMessageIdQueryHandler(
        IRaidRepository raidRepository,
        ILogger<GetRaidByMessageIdQueryHandler> logger) : base(logger)
    {
        _raidRepository = raidRepository;
    }

    protected override async Task<Result<RaidDto?>> HandleQuery(GetRaidByMessageIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MessageId))
        {
            return Result<RaidDto?>.Failure("Message ID cannot be empty");
        }

        var raid = await _raidRepository.GetByDiscordMessageIdAsync(request.MessageId, cancellationToken);
        
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

