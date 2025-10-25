using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to leave a raid
/// </summary>
public class LeaveRaidCommand : ICommand
{
    public int RaidId { get; set; }
    public int PlayerId { get; set; }
}
