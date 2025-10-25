using Location.Service.Application.DTOs;
using Location.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Commands;

/// <summary>
/// Handler for updating an existing location
/// </summary>
public class UpdateLocationCommandHandler : CommandHandler<UpdateLocationCommand, LocationDto>
{
    private readonly ILocationRepository _locationRepository;

    public UpdateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILogger<UpdateLocationCommandHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result<LocationDto>> HandleCommand(UpdateLocationCommand request, CancellationToken cancellationToken)
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

        // Get existing location
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (location == null)
        {
            return Result<LocationDto>.Failure("Location not found");
        }

        // Update location properties
        location.Name = request.Name;
        location.Latitude = request.Latitude;
        location.Longitude = request.Longitude;
        location.Address = request.Address;
        location.City = request.City;
        location.State = request.State;
        location.Country = request.Country;
        location.PostalCode = request.PostalCode;
        location.IsActive = request.IsActive;
        location.LocationType = request.LocationType;
        location.Notes = request.Notes;

        await _locationRepository.UpdateAsync(location, cancellationToken);
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
