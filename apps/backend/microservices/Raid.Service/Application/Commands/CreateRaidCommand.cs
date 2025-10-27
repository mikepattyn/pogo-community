using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Command to create a new raid
/// </summary>
public class CreateRaidCommand : ICommand<RaidDto>
{
    public string DiscordMessageId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string PokemonSpecies { get; set; } = string.Empty;
    public int Level { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public string Difficulty { get; set; } = "Medium";
    public string? WeatherBoost { get; set; }
    public string? Notes { get; set; }
}
