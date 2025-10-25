using Player.Service.Application.DTOs;
using Pogo.Shared.Application;

namespace Player.Service.Application.Queries;

/// <summary>
/// Query to get player by ID
/// </summary>
public class GetPlayerByIdQuery : IQuery<PlayerDto?>
{
    public int Id { get; set; }
}
