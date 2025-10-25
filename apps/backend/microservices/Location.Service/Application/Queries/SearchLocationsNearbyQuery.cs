using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Queries;

/// <summary>
/// Query to search for locations near a specific point
/// </summary>
public class SearchLocationsNearbyQuery : IQuery<IEnumerable<LocationSearchDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public string? LocationType { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public int MaxResults { get; set; } = 50;
}
