using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Queries;

/// <summary>
/// Query to get locations by type
/// </summary>
public class GetLocationsByTypeQuery : IQuery<IEnumerable<LocationDto>>
{
    public string LocationType { get; set; } = string.Empty;
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
