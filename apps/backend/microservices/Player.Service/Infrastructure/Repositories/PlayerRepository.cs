using Player.Service.Application.Interfaces;
using Player.Service.Domain.Entities;
using Player.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PlayerEntity = Player.Service.Domain.Entities.Player;

namespace Player.Service.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Player entity
/// </summary>
public class PlayerRepository : IPlayerRepository
{
    private readonly PlayerDbContext _context;

    public PlayerRepository(PlayerDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PlayerEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Username == username, cancellationToken);
    }

    public async Task<PlayerEntity?> GetByDiscordIdAsync(string discordUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.DiscordUserId == discordUserId, cancellationToken);
    }

    public async Task<IEnumerable<PlayerEntity>> GetAllAsync(bool activeOnly = true, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Players.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query
            .OrderBy(p => p.Username)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PlayerEntity player, CancellationToken cancellationToken = default)
    {
        await _context.Players.AddAsync(player, cancellationToken);
    }

    public async Task UpdateAsync(PlayerEntity player, CancellationToken cancellationToken = default)
    {
        _context.Players.Update(player);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
