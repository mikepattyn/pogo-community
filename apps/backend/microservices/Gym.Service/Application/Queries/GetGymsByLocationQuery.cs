using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Queries;

/// <summary>
/// Query to get gyms by location ID
/// </summary>
public class GetGymsByLocationQuery : IQuery<IEnumerable<GymDto>>
{
    public int LocationId { get; set; }
    public bool ActiveOnly { get; set; } = true;
}
