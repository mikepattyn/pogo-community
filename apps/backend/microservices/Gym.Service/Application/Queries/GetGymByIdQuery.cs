using Gym.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Gym.Service.Application.Queries;

/// <summary>
/// Query to get gym by ID
/// </summary>
public class GetGymByIdQuery : IQuery<GymDto?>
{
    public int Id { get; set; }
}
