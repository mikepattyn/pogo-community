namespace Bot.Service.Application.DTOs;

public class ScanImageRequest
{
    public string Url { get; set; } = string.Empty;
}

public class ScanImageResponse
{
    public RaidDataDto RaidData { get; set; } = new();
}

public class RaidDataDto
{
    public string PokemonName { get; set; } = string.Empty;
    public int Tier { get; set; }
    public string GymName { get; set; } = string.Empty;
    public int CombatPower { get; set; }
    public string TimeRemaining { get; set; } = string.Empty;
    public string GroupType { get; set; } = string.Empty;
}
