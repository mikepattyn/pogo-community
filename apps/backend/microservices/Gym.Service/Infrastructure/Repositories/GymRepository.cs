using Gym.Service.Application.Interfaces;
using Gym.Service.Domain.Entities;
using Gym.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using GymEntity = Gym.Service.Domain.Entities.Gym;

namespace Gym.Service.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Gym entity
/// </summary>
public class GymRepository : IGymRepository
{
    private readonly GymDbContext _context;

    public GymRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<GymEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Gyms
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<GymEntity>> GetByLocationIdAsync(int locationId, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Gyms
            .Where(g => g.LocationId == locationId);

        if (activeOnly)
        {
            query = query.Where(g => g.IsActive);
        }

        return await query
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GymEntity>> GetByTeamAsync(string team, bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Gyms
            .Where(g => g.ControllingTeam == team);

        if (activeOnly)
        {
            query = query.Where(g => g.IsActive);
        }

        return await query
            .OrderBy(g => g.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GymEntity>> GetByLocationIdsAsync(IEnumerable<int> locationIds, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Gyms
            .Where(g => locationIds.Contains(g.LocationId));

        if (activeOnly)
        {
            query = query.Where(g => g.IsActive);
        }

        return await query
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GymEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Gyms.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(g => g.IsActive);
        }

        return await query
            .OrderBy(g => g.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GymEntity gym, CancellationToken cancellationToken = default)
    {
        await _context.Gyms.AddAsync(gym, cancellationToken);
    }

    public async Task UpdateAsync(GymEntity gym, CancellationToken cancellationToken = default)
    {
        _context.Gyms.Update(gym);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
