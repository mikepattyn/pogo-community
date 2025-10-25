using Player.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Player.Service.Application.Commands;

/// <summary>
/// Handler for deactivating a player
/// </summary>
public class DeactivatePlayerCommandHandler : CommandHandler<DeactivatePlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public DeactivatePlayerCommandHandler(
        IPlayerRepository playerRepository,
        ILogger<DeactivatePlayerCommandHandler> logger) : base(logger)
    {
        _playerRepository = playerRepository;
    }

    protected override async Task<Result> HandleCommand(DeactivatePlayerCommand request, CancellationToken cancellationToken)
    {
        // Get existing player
        var player = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (player == null)
        {
            return Result.Failure("Player not found");
        }

        // Deactivate player
        player.Deactivate();

        await _playerRepository.UpdateAsync(player, cancellationToken);
        await _playerRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
