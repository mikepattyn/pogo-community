using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to cancel a raid
/// </summary>
public class CancelRaidCommand : ICommand
{
    public int Id { get; set; }
}
