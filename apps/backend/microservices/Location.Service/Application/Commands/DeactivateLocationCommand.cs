using Pogo.Shared.Application;

namespace Location.Service.Application.Commands;

/// <summary>
/// Command to deactivate a location
/// </summary>
public class DeactivateLocationCommand : ICommand
{
    public int Id { get; set; }
}
