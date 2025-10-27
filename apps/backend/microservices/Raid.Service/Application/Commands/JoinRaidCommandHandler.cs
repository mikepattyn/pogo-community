using Raid.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Raid.Service.Application.Commands;

/// <summary>
/// Handler for joining a raid
/// </summary>
public class JoinRaidCommandHandler : CommandHandler<JoinRaidCommand>
{
    private readonly IRaidRepository _raidRepository;

    public JoinRaidCommandHandler(
        IRaidRepository raidRepository,
        ILogger<JoinRaidCommandHandler> logger) : base(logger)
    {
        _raidRepository = raidRepository;
    }

    protected override async Task<Result> HandleCommand(JoinRaidCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MessageId))
        {
            return Result.Failure("Message ID is required");
        }

        if (request.PlayerId <= 0)
        {
            return Result.Failure("Player ID must be greater than 0");
        }

        // Get raid by Discord message ID
        var raid = await _raidRepository.GetByDiscordMessageIdAsync(request.MessageId, cancellationToken);
        
        if (raid == null)
        {
            return Result.Failure("Raid not found");
        }

        // Check if raid is still active
        if (!raid.IsActive || raid.IsCompleted || raid.IsCancelled)
        {
            return Result.Failure("Raid is not active");
        }

        if (raid.HasEnded)
        {
            return Result.Failure("Raid has already ended");
        }

        // Add participant
        try
        {
            raid.AddParticipant();
            await _raidRepository.UpdateAsync(raid, cancellationToken);
            await _raidRepository.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }
}

