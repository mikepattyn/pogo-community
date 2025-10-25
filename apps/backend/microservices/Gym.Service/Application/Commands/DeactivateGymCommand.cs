using Pogo.Shared.Application;

namespace Gym.Service.Application.Commands;

/// <summary>
/// Command to deactivate a gym
/// </summary>
public class DeactivateGymCommand : ICommand
{
    public int Id { get; set; }
}
