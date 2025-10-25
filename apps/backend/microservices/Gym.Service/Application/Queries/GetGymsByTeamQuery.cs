using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Queries;

/// <summary>
/// Query to get gyms by controlling team
/// </summary>
public class GetGymsByTeamQuery : IQuery<IEnumerable<GymDto>>
{
    public string Team { get; set; } = string.Empty;
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
