using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to get currently active raids
/// </summary>
public class GetActiveRaidsQuery : IQuery<IEnumerable<RaidDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
