using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to join a raid
/// </summary>
public class JoinRaidCommand : ICommand
{
    public string MessageId { get; set; } = string.Empty;
    public int PlayerId { get; set; }
}
