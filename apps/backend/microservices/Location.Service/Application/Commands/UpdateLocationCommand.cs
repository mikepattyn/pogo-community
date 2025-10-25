using Location.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Location.Service.Application.Commands;

/// <summary>
/// Command to update an existing location
/// </summary>
public class UpdateLocationCommand : ICommand<LocationDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; }
    public string LocationType { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
