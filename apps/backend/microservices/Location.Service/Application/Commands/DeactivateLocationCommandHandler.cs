using Location.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Application;
using Pogo.Shared.Kernel;

namespace Location.Service.Application.Commands;

/// <summary>
/// Handler for deactivating a location
/// </summary>
public class DeactivateLocationCommandHandler : CommandHandler<DeactivateLocationCommand>
{
    private readonly ILocationRepository _locationRepository;

    public DeactivateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILogger<DeactivateLocationCommandHandler> logger) : base(logger)
    {
        _locationRepository = locationRepository;
    }

    protected override async Task<Result> HandleCommand(DeactivateLocationCommand request, CancellationToken cancellationToken)
    {
        // Get existing location
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (location == null)
        {
            return Result.Failure("Location not found");
        }

        // Deactivate location
        location.Deactivate();

        await _locationRepository.UpdateAsync(location, cancellationToken);
        await _locationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
