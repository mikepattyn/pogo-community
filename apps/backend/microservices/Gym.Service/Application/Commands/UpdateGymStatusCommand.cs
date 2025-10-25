using Pogo.Shared.Application;

namespace Gym.Service.Application.Commands;

/// <summary>
/// Command to update gym status (under attack, in raid, etc.)
/// </summary>
public class UpdateGymStatusCommand : ICommand
{
    public int Id { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInRaid { get; set; }
    public int? MotivationLevel { get; set; }
}
