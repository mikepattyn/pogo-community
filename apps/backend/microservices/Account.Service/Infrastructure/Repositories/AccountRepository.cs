using Account.Service.Application.Interfaces;
using Account.Service.Domain.Entities;
using Account.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AccountEntity = Account.Service.Domain.Entities.Account;

namespace Account.Service.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Account entity
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _context;

    public AccountRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<AccountEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<AccountEntity?> GetByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.PlayerId == playerId, cancellationToken);
    }

    public async Task<AccountEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(AccountEntity account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
