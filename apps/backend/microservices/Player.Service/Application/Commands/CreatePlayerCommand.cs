using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Commands;

/// <summary>
/// Command to create a new player
/// </summary>
public class CreatePlayerCommand : ICommand<PlayerDto>
{
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public string Team { get; set; } = string.Empty;
    public string FriendCode { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string? DiscordUserId { get; set; }
}
