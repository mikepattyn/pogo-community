using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Queries;

/// <summary>
/// Query to get player by Discord user ID
/// </summary>
public class GetPlayerByDiscordIdQuery : IQuery<PlayerDto?>
{
    public string DiscordUserId { get; set; } = string.Empty;
}
