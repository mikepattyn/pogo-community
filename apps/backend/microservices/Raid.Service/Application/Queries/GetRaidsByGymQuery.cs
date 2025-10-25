using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to get raids by gym ID
/// </summary>
public class GetRaidsByGymQuery : IQuery<IEnumerable<RaidDto>>
{
    public int GymId { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
