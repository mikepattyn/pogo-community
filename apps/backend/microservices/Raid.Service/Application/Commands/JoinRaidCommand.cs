using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to join a raid
/// </summary>
public class JoinRaidCommand : ICommand
{
    public int RaidId { get; set; }
    public int PlayerId { get; set; }
}
