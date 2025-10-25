using Location.Service.Application.DTOs;
using Location.Service.Application.Interfaces;
using Location.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Commands;

/// <summary>
/// Handler for creating a new location
/// </summary>
public class CreateLocationCommandHandler : CommandHandler<CreateLocationCommand, LocationDto>
{
    private readonly ILocationRepository _locationRepository;

    public CreateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILogger<CreateLocationCommandHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result<LocationDto>> HandleCommand(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        // Validate coordinates
        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return Result<LocationDto>.Failure("Latitude must be between -90 and 90 degrees");
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return Result<LocationDto>.Failure("Longitude must be between -180 and 180 degrees");
        }

        // Create new location
        var location = new Domain.Entities.Location
        {
            Name = request.Name,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            LocationType = request.LocationType,
            Notes = request.Notes,
            IsActive = true
        };

        await _locationRepository.AddAsync(location, cancellationToken);
        await _locationRepository.SaveChangesAsync(cancellationToken);

        return Result<LocationDto>.Success(MapToDto(location));
    }

    private static LocationDto MapToDto(Domain.Entities.Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Address = location.Address,
            City = location.City,
            State = location.State,
            Country = location.Country,
            PostalCode = location.PostalCode,
            IsActive = location.IsActive,
            LocationType = location.LocationType,
            Notes = location.Notes,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}
