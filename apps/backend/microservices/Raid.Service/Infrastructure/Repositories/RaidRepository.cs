using Raid.Service.Application.Interfaces;
using Raid.Service.Domain.Entities;
using Raid.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using RaidEntity = Raid.Service.Domain.Entities.Raid;

namespace Raid.Service.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Raid entity
/// </summary>
public class RaidRepository : IRaidRepository
{
    private readonly RaidDbContext _context;

    public RaidRepository(RaidDbContext context)
    {
        _context = context;
    }

    public async Task<RaidEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Raids
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<RaidEntity>> GetByGymIdAsync(int gymId, bool activeOnly = true, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Raids
            .Where(r => r.GymId == gymId);

        if (activeOnly)
        {
            query = query.Where(r => r.IsActive);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(r => r.StartTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(r => r.EndTime <= toDate.Value);
        }

        return await query
            .OrderBy(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RaidEntity>> GetActiveRaidsAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await _context.Raids
            .Where(r => r.IsActive && !r.IsCompleted && !r.IsCancelled && r.StartTime <= now && r.EndTime >= now)
            .OrderBy(r => r.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RaidEntity>> GetByGymIdsAsync(IEnumerable<int> gymIds, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Raids
            .Where(r => gymIds.Contains(r.GymId));

        if (activeOnly)
        {
            query = query.Where(r => r.IsActive);
        }

        return await query
            .OrderBy(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RaidEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Raids.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(r => r.IsActive);
        }

        return await query
            .OrderBy(r => r.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RaidEntity raid, CancellationToken cancellationToken = default)
    {
        await _context.Raids.AddAsync(raid, cancellationToken);
    }

    public async Task UpdateAsync(RaidEntity raid, CancellationToken cancellationToken = default)
    {
        _context.Raids.Update(raid);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
