using Account.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Account.Service.Application.Queries;

/// <summary>
/// Query to get account by email
/// </summary>
public class GetAccountByEmailQuery : IQuery<AccountDto?>
{
    public string Email { get; set; } = string.Empty;
}
