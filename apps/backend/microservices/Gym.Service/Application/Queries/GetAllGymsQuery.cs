using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Queries;

/// <summary>
/// Query to get all gyms
/// </summary>
public class GetAllGymsQuery : IQuery<IEnumerable<GymDto>>
{
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
