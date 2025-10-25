using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Commands;

/// <summary>
/// Command to create a new location
/// </summary>
public class CreateLocationCommand : ICommand<LocationDto>
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string LocationType { get; set; } = "Landmark";
    public string? Notes { get; set; }
}
