using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Queries;

/// <summary>
/// Query to get player by username
/// </summary>
public class GetPlayerByUsernameQuery : IQuery<PlayerDto?>
{
    public string Username { get; set; } = string.Empty;
}
