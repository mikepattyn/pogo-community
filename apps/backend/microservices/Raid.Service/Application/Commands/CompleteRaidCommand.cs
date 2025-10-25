using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to complete a raid
/// </summary>
public class CompleteRaidCommand : ICommand
{
    public int Id { get; set; }
}
