using Account.Service.Application.DTOs;
using Account.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Account.Service.Application.Commands;

/// <summary>
/// Handler for user login
/// </summary>
public class LoginCommandHandler : CommandHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<LoginCommandHandler> logger) : base(logger)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    protected override async Task<Result<LoginResponseDto>> HandleCommand(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get account by email
        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (account == null)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password");
        }

        // Check if account is locked
        if (account.IsLocked)
        {
            return Result<LoginResponseDto>.Failure("Account is locked. Please try again later");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, account.Password))
        {
            account.IncrementWrongAttempts();
            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _accountRepository.SaveChangesAsync(cancellationToken);
            return Result<LoginResponseDto>.Failure("Invalid email or password");
        }

        // Reset wrong attempts on successful login
        account.ResetWrongAttempts();
        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtTokenGenerator.GenerateToken(account.Email, account.Id.ToString());
        var expiresAt = DateTime.UtcNow.AddHours(12);

        var response = new LoginResponseDto
        {
            Token = token,
            Account = MapToDto(account),
            ExpiresAt = expiresAt
        };

        return Result<LoginResponseDto>.Success(response);
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
