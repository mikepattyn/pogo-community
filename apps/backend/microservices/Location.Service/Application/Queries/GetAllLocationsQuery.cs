using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Queries;

/// <summary>
/// Query to get all locations
/// </summary>
public class GetAllLocationsQuery : IQuery<IEnumerable<LocationDto>>
{
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
