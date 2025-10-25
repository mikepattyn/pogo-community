using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to update an existing raid
/// </summary>
public class UpdateRaidCommand : ICommand<RaidDto>
{
    public int Id { get; set; }
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCancelled { get; set; }
    public int MaxParticipants { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
}
