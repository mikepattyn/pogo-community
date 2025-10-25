using Account.Service.Application.DTOs;
using Account.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Account.Service.Application.Queries;

/// <summary>
/// Handler for getting account by email
/// </summary>
public class GetAccountByEmailQueryHandler : QueryHandler<GetAccountByEmailQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByEmailQueryHandler(
        IAccountRepository accountRepository,
        ILogger<GetAccountByEmailQueryHandler> logger) : base(logger)
    {
        _accountRepository = accountRepository;
    }

    protected override async Task<Result<AccountDto?>> HandleQuery(GetAccountByEmailQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (account == null)
        {
            return Result<AccountDto?>.Success(null);
        }

        var dto = MapToDto(account);
        return Result<AccountDto?>.Success(dto);
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
