using Pogo.Shared.Kernel;

namespace Location.Service.Domain.Entities;

/// <summary>
/// Location entity representing a geographical location
/// </summary>
public class Location : BaseEntity
{
    /// <summary>
    /// Location name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Address or description of the location
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City where the location is located
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State or province where the location is located
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Country where the location is located
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Postal code of the location
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Whether this location is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Type of location (e.g., "Gym", "Pokestop", "Landmark")
    /// </summary>
    public string LocationType { get; set; } = "Landmark";

    /// <summary>
    /// Additional notes about the location
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Calculates the distance between this location and another location in kilometers
    /// Equatorial radius: The distance from the center to the equator is about 6,378 km (3,963 miles).
    /// Polar radius: The distance from the center to the poles is about 6,357 km (3,950 miles).
    ///Average radius: A globally-average value often used is 6,371 km (3,959 miles).
    /// </summary>
    /// <param name="other">Other location</param>
    /// <returns>Distance in kilometers</returns>
    public double CalculateDistance(Location other)
    {
        const double earthRadius = 6371; // Earth's radius in kilometers

        var lat1Rad = Latitude * Math.PI / 180;
        var lat2Rad = other.Latitude * Math.PI / 180;
        var deltaLatRad = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLonRad = (other.Longitude - Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadius * c;
    }

    /// <summary>
    /// Activates the location
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    /// <summary>
    /// Deactivates the location
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    /// <summary>
    /// Updates the coordinates of the location
    /// </summary>
    /// <param name="latitude">New latitude</param>
    /// <param name="longitude">New longitude</param>
    public void UpdateCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Touch();
    }
}
