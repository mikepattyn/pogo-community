namespace Bot.Service.Application.DTOs;

public class RaidDto
{
    public string MessageId { get; set; } = string.Empty;
    public string MessageTitle { get; set; } = string.Empty;
    public List<PlayerDto> Players { get; set; } = new();
    public DateTime RaidTime { get; set; }
    public bool Closed { get; set; }
    public string StartedBy { get; set; } = string.Empty;
}

public class PlayerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Additions { get; set; }
}
