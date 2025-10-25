using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Queries;

/// <summary>
/// Query to get locations by name
/// </summary>
public class GetLocationsByNameQuery : IQuery<IEnumerable<LocationDto>>
{
    public string Name { get; set; } = string.Empty;
    public bool ActiveOnly { get; set; } = true;
}
