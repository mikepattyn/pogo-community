using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Queries;

/// <summary>
/// Query to search for gyms near a specific point
/// </summary>
public class SearchGymsNearbyQuery : IQuery<IEnumerable<GymSearchDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public string? Team { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public int MaxResults { get; set; } = 50;
}
