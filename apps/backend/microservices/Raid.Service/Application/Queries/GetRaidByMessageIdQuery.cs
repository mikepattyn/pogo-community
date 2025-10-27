using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to get raid by Discord message ID
/// </summary>
public class GetRaidByMessageIdQuery : IQuery<RaidDto?>
{
    public string MessageId { get; set; } = string.Empty;
}

