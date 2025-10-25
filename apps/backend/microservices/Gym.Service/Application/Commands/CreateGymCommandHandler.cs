using Gym.Service.Application.DTOs;
using Gym.Service.Application.Interfaces;
using Gym.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Gym.Service.Application.Commands;

/// <summary>
/// Handler for creating a new gym
/// </summary>
public class CreateGymCommandHandler : CommandHandler<CreateGymCommand, GymDto>
{
    private readonly IGymRepository _gymRepository;
    private readonly ILocationServiceClient _locationServiceClient;

    public CreateGymCommandHandler(
        IGymRepository gymRepository,
        ILocationServiceClient locationServiceClient,
        ILogger<CreateGymCommandHandler> logger) : base(logger)
    {
        _gymRepository = gymRepository;
        _locationServiceClient = locationServiceClient;
    }

    protected override async Task<Result<GymDto>> HandleCommand(CreateGymCommand request, CancellationToken cancellationToken)
    {
        // Validate gym level
        if (request.Level < 1 || request.Level > 6)
        {
            return Result<GymDto>.Failure("Gym level must be between 1 and 6");
        }

        // Validate motivation level
        if (request.MotivationLevel < 0 || request.MotivationLevel > 100)
        {
            return Result<GymDto>.Failure("Motivation level must be between 0 and 100");
        }

        // Verify location exists
        var location = await _locationServiceClient.GetLocationByIdAsync(request.LocationId, cancellationToken);
        if (location == null)
        {
            return Result<GymDto>.Failure("Location not found");
        }

        if (!location.IsActive)
        {
            return Result<GymDto>.Failure("Location is not active");
        }

        // Create new gym
        var gym = new Domain.Entities.Gym
        {
            Name = request.Name,
            LocationId = request.LocationId,
            Level = request.Level,
            ControllingTeam = request.ControllingTeam,
            MotivationLevel = request.MotivationLevel,
            Notes = request.Notes,
            IsActive = true
        };

        await _gymRepository.AddAsync(gym, cancellationToken);
        await _gymRepository.SaveChangesAsync(cancellationToken);

        return Result<GymDto>.Success(MapToDto(gym));
    }

    private static GymDto MapToDto(Domain.Entities.Gym gym)
    {
        return new GymDto
        {
            Id = gym.Id,
            Name = gym.Name,
            LocationId = gym.LocationId,
            Level = gym.Level,
            ControllingTeam = gym.ControllingTeam,
            IsUnderAttack = gym.IsUnderAttack,
            IsInRaid = gym.IsInRaid,
            MotivationLevel = gym.MotivationLevel,
            LastUpdated = gym.LastUpdated,
            IsActive = gym.IsActive,
            Notes = gym.Notes,
            CreatedAt = gym.CreatedAt,
            UpdatedAt = gym.UpdatedAt
        };
    }
}
