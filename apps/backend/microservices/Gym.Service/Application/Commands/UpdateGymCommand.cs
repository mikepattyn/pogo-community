using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Commands;

/// <summary>
/// Command to update an existing gym
/// </summary>
public class UpdateGymCommand : ICommand<GymDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int Level { get; set; }
    public string? ControllingTeam { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInRaid { get; set; }
    public int MotivationLevel { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
