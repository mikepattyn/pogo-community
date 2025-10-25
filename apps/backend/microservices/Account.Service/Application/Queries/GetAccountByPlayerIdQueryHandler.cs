using Account.Service.Application.DTOs;
using Account.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Account.Service.Application.Queries;

/// <summary>
/// Handler for getting account by player ID
/// </summary>
public class GetAccountByPlayerIdQueryHandler : QueryHandler<GetAccountByPlayerIdQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByPlayerIdQueryHandler(
        IAccountRepository accountRepository,
        ILogger<GetAccountByPlayerIdQueryHandler> logger) : base(logger)
    {
        _accountRepository = accountRepository;
    }

    protected override async Task<Result<AccountDto?>> HandleQuery(GetAccountByPlayerIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByPlayerIdAsync(request.PlayerId, cancellationToken);
        
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
