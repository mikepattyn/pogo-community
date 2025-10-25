using Raid.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Raid.Service.Application.Queries;

/// <summary>
/// Query to get raid by ID
/// </summary>
public class GetRaidByIdQuery : IQuery<RaidDto?>
{
    public int Id { get; set; }
}
