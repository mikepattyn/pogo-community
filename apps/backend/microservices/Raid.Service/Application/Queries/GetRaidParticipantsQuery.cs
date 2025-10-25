using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to get raid participants
/// </summary>
public class GetRaidParticipantsQuery : IQuery<IEnumerable<RaidParticipationDto>>
{
    public int RaidId { get; set; }
}
