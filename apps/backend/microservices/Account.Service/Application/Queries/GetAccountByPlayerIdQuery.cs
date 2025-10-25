using Account.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Account.Service.Application.Queries;

/// <summary>
/// Query to get account by player ID
/// </summary>
public class GetAccountByPlayerIdQuery : IQuery<AccountDto?>
{
    public int PlayerId { get; set; }
}
