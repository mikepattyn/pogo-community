using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Queries;

/// <summary>
/// Query to get location by ID
/// </summary>
public class GetLocationByIdQuery : IQuery<LocationDto?>
{
    public int Id { get; set; }
}
