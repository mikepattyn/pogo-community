using Location.Service.Application.Interfaces;
using Location.Service.Domain.Entities;
using Location.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LocationEntity = Location.Service.Domain.Entities.Location;

namespace Location.Service.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Location entity
/// </summary>
public class LocationRepository : ILocationRepository
{
    private readonly LocationDbContext _context;

    public LocationRepository(LocationDbContext context)
    {
        _context = context;
    }

    public async Task<LocationEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<LocationEntity>> GetByNameAsync(string name, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Locations
            .Where(l => l.Name.Contains(name));

        if (activeOnly)
        {
            query = query.Where(l => l.IsActive);
        }

        return await query
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LocationEntity>> GetByTypeAsync(string locationType, bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Locations
            .Where(l => l.LocationType == locationType);

        if (activeOnly)
        {
            query = query.Where(l => l.IsActive);
        }

        return await query
            .OrderBy(l => l.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LocationEntity>> SearchNearbyAsync(double latitude, double longitude, double radiusKm, string? locationType = null, bool activeOnly = true, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        // Get all locations first (in a real implementation, you'd use spatial queries)
        var query = _context.Locations.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(l => l.IsActive);
        }

        if (!string.IsNullOrEmpty(locationType))
        {
            query = query.Where(l => l.LocationType == locationType);
        }

        var locations = await query.ToListAsync(cancellationToken);

        // Calculate distances and filter by radius
        var nearbyLocations = locations
            .Select(l => new { Location = l, Distance = CalculateDistance(latitude, longitude, l.Latitude, l.Longitude) })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Take(maxResults)
            .Select(x => x.Location)
            .ToList();

        return nearbyLocations;
    }

    public async Task<IEnumerable<LocationEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Locations.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(l => l.IsActive);
        }

        return await query
            .OrderBy(l => l.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LocationEntity location, CancellationToken cancellationToken = default)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
    }

    public async Task UpdateAsync(LocationEntity location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(location);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371; // Earth's radius in kilometers
        
        var lat1Rad = lat1 * Math.PI / 180;
        var lat2Rad = lat2 * Math.PI / 180;
        var deltaLatRad = (lat2 - lat1) * Math.PI / 180;
        var deltaLonRad = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadius * c;
    }
}
