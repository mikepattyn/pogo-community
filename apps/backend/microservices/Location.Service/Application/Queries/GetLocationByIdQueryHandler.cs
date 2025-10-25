using Location.Service.Application.DTOs;
using Location.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Queries;

/// <summary>
/// Handler for getting location by ID
/// </summary>
public class GetLocationByIdQueryHandler : QueryHandler<GetLocationByIdQuery, LocationDto?>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdQueryHandler(
        ILocationRepository locationRepository,
        ILogger<GetLocationByIdQueryHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result<LocationDto?>> HandleQuery(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (location == null)
        {
            return Result<LocationDto?>.Success(null);
        }

        var dto = MapToDto(location);
        return Result<LocationDto?>.Success(dto);
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
