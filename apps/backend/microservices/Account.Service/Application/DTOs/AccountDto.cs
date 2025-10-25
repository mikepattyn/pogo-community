namespace Account.Service.Application.DTOs;

/// <summary>
/// Data Transfer Object for Account information
/// </summary>
public class AccountDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; }
    public int WrongAttempts { get; set; }
    public DateTime? LockedOut { get; set; }
    public bool IsLocked { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new account
/// </summary>
public class CreateAccountDto
{
    public int PlayerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for login request
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for login response
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public AccountDto Account { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}
