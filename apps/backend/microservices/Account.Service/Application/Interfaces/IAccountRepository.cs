using Account.Service.Domain.Entities;
using AccountEntity = Account.Service.Domain.Entities.Account;

namespace Account.Service.Application.Interfaces;

/// <summary>
/// Repository interface for Account entity
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Gets an account by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account entity or null if not found</returns>
    Task<AccountEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an account by player ID
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account entity or null if not found</returns>
    Task<AccountEntity?> GetByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an account by ID
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account entity or null if not found</returns>
    Task<AccountEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account
    /// </summary>
    /// <param name="account">Account entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(AccountEntity account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account
    /// </summary>
    /// <param name="account">Account entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
