using Location.Service.Application.DTOs;
using Location.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Queries;

/// <summary>
/// Handler for searching locations nearby
/// </summary>
public class SearchLocationsNearbyQueryHandler : QueryHandler<SearchLocationsNearbyQuery, IEnumerable<LocationSearchDto>>
{
    private readonly ILocationRepository _locationRepository;

    public SearchLocationsNearbyQueryHandler(
        ILocationRepository locationRepository,
        ILogger<SearchLocationsNearbyQueryHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result<IEnumerable<LocationSearchDto>>> HandleQuery(SearchLocationsNearbyQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.SearchNearbyAsync(
            request.Latitude, 
            request.Longitude, 
            request.RadiusKm, 
            request.LocationType, 
            request.ActiveOnly, 
            request.MaxResults, 
            cancellationToken);
        
        var dtos = locations.Select(location => MapToSearchDto(location, request.Latitude, request.Longitude)).ToList();
        return Result<IEnumerable<LocationSearchDto>>.Success(dtos);
    }

    private static LocationSearchDto MapToSearchDto(Domain.Entities.Location location, double searchLatitude, double searchLongitude)
    {
        var searchLocation = new Domain.Entities.Location
        {
            Latitude = searchLatitude,
            Longitude = searchLongitude
        };

        return new LocationSearchDto
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
            DistanceKm = location.CalculateDistance(searchLocation),
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}
