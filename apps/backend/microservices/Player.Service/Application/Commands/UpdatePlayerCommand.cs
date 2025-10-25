using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Commands;

/// <summary>
/// Command to update an existing player
/// </summary>
public class UpdatePlayerCommand : ICommand<PlayerDto>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? DiscordUserId { get; set; }
}
