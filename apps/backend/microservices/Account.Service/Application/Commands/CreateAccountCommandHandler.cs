using Account.Service.Application.DTOs;
using Account.Service.Application.Interfaces;
using Account.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Account.Service.Application.Commands;

/// <summary>
/// Handler for creating a new account
/// </summary>
public class CreateAccountCommandHandler : CommandHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        ILogger<CreateAccountCommandHandler> logger) : base(logger)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
    }

    protected override async Task<Result<AccountDto>> HandleCommand(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // Check if account with email already exists
        var existingAccount = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingAccount != null)
        {
            return Result<AccountDto>.Failure("An account with this email already exists");
        }

        // Hash the password
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        // Create new account
        var account = new Domain.Entities.Account
        {
            PlayerId = request.PlayerId,
            Email = request.Email,
            Password = hashedPassword,
            DateJoined = DateTime.UtcNow
        };

        await _accountRepository.AddAsync(account, cancellationToken);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        return Result<AccountDto>.Success(MapToDto(account));
    }

    private static AccountDto MapToDto(Domain.Entities.Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            PlayerId = account.PlayerId,
            Email = account.Email,
            DateJoined = account.DateJoined,
            WrongAttempts = account.WrongAttempts,
            LockedOut = account.LockedOut,
            IsLocked = account.IsLocked
        };
    }
}
