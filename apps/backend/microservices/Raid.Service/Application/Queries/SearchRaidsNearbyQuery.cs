using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to search for raids near a specific point
/// </summary>
public class SearchRaidsNearbyQuery : IQuery<IEnumerable<RaidSearchDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public int? Level { get; set; }
    public string? PokemonSpecies { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public int MaxResults { get; set; } = 50;
}
