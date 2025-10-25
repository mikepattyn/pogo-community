using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Queries;

/// <summary>
/// Query to get all players
/// </summary>
public class GetAllPlayersQuery : IQuery<IEnumerable<PlayerDto>>
{
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
