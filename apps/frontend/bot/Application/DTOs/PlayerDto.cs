namespace Bot.Service.Application.DTOs;

public class CreatePlayerDto
{
    public string DiscordId { get; set; } = string.Empty;
    public string DateJoined { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? Nickname { get; set; }
    public int? Level { get; set; }
    public string? Team { get; set; }
}

public class UpdatePlayerDto
{
    public string? FirstName { get; set; }
    public string? Nickname { get; set; }
    public int? Level { get; set; }
    public string? Team { get; set; }
}
