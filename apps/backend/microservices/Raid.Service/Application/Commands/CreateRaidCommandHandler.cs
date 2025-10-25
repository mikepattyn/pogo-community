using Raid.Service.Application.DTOs;
using Raid.Service.Application.Interfaces;
using Raid.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Handler for creating a new raid
/// </summary>
public class CreateRaidCommandHandler : CommandHandler<CreateRaidCommand, RaidDto>
{
    private readonly IRaidRepository _raidRepository;
    private readonly IGymServiceClient _gymServiceClient;

    public CreateRaidCommandHandler(
        IRaidRepository raidRepository,
        IGymServiceClient gymServiceClient,
        ILogger<CreateRaidCommandHandler> logger) : base(logger)
    {
        _raidRepository = raidRepository;
        _gymServiceClient = gymServiceClient;
    }

    protected override async Task<Result<RaidDto>> HandleCommand(CreateRaidCommand request, CancellationToken cancellationToken)
    {
        // Validate raid level
        if (request.Level < 1 || request.Level > 5)
        {
            return Result<RaidDto>.Failure("Raid level must be between 1 and 5");
        }

        // Validate time range
        if (request.StartTime >= request.EndTime)
        {
            return Result<RaidDto>.Failure("Start time must be before end time");
        }

        // Validate max participants
        if (request.MaxParticipants <= 0)
        {
            return Result<RaidDto>.Failure("Max participants must be greater than 0");
        }

        // Verify gym exists and is available
        var gym = await _gymServiceClient.GetGymByIdAsync(request.GymId, cancellationToken);
        if (gym == null)
        {
            return Result<RaidDto>.Failure("Gym not found");
        }

        if (!gym.IsActive)
        {
            return Result<RaidDto>.Failure("Gym is not active");
        }

        if (gym.IsInRaid)
        {
            return Result<RaidDto>.Failure("Gym is already in a raid");
        }

        // Create new raid
        var raid = new Domain.Entities.Raid
        {
            GymId = request.GymId,
            PokemonSpecies = request.PokemonSpecies,
            Level = request.Level,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxParticipants = request.MaxParticipants,
            Difficulty = request.Difficulty,
            WeatherBoost = request.WeatherBoost,
            Notes = request.Notes,
            IsActive = true
        };

        await _raidRepository.AddAsync(raid, cancellationToken);
        await _raidRepository.SaveChangesAsync(cancellationToken);

        return Result<RaidDto>.Success(MapToDto(raid));
    }

    private static RaidDto MapToDto(Domain.Entities.Raid raid)
    {
        return new RaidDto
        {
            Id = raid.Id,
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
