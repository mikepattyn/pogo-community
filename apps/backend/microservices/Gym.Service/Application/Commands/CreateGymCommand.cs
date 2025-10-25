using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Commands;

/// <summary>
/// Command to create a new gym
/// </summary>
public class CreateGymCommand : ICommand<GymDto>
{
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; } = 1;
    public string? ControllingTeam { get; set; }
    public int MotivationLevel { get; set; } = 100;
    public string? Notes { get; set; }
}
