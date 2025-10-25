namespace Location.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for Location information
/// </summary>
public class LocationDto
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new location
/// </summary>
public class CreateLocationDto
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

/// <summary>
/// Data Transfer Object for updating a location
/// </summary>
public class UpdateLocationDto
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

/// <summary>
/// Data Transfer Object for location search with distance
/// </summary>
public class LocationSearchDto
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
    public double DistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
