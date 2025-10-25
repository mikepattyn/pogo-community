using Account.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Account.Service.Application.Commands;

/// <summary>
/// Command to login a user
/// </summary>
public class LoginCommand : ICommand<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
