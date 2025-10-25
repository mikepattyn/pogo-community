using Pogo.Shared.Application;

namespace Player.Service.Application.Commands;

/// <summary>
/// Command to deactivate a player
/// </summary>
public class DeactivatePlayerCommand : ICommand
{
    public int Id { get; set; }
}
