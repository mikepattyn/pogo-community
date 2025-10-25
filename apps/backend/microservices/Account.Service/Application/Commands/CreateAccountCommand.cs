using Account.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Account.Service.Application.Commands;

/// <summary>
/// Command to create a new account
/// </summary>
public class CreateAccountCommand : ICommand<AccountDto>
{
    public int PlayerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
