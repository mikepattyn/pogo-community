using Location.Service.Application.DTOs;
using Location.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Queries;

/// <summary>
/// Handler for getting locations by type
/// </summary>
public class GetLocationsByTypeQueryHandler : QueryHandler<GetLocationsByTypeQuery, IEnumerable<LocationDto>>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationsByTypeQueryHandler(
        ILocationRepository locationRepository,
        ILogger<GetLocationsByTypeQueryHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result<IEnumerable<LocationDto>>> HandleQuery(GetLocationsByTypeQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetByTypeAsync(request.LocationType, request.ActiveOnly, request.PageNumber, request.PageSize, cancellationToken);
        
        var dtos = locations.Select(MapToDto).ToList();
        return Result<IEnumerable<LocationDto>>.Success(dtos);
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
