using MediatR;
using OCR.Service.Application.DTOs;
using OCR.Service.Application.Interfaces;
using Pogo.Shared.Kernel;

namespace OCR.Service.Application.Commands;

public class ExtractRaidDataCommandHandler : IRequestHandler<ExtractRaidDataCommand, Result<RaidDataDto>>
{
    private readonly IOCRService _ocrService;
    private readonly ILogger<ExtractRaidDataCommandHandler> _logger;

    public ExtractRaidDataCommandHandler(
        IOCRService ocrService,
        ILogger<ExtractRaidDataCommandHandler> logger)
    {
        _ocrService = ocrService;
        _logger = logger;
    }

    public async Task<Result<RaidDataDto>> Handle(ExtractRaidDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ImageUrl))
            {
                return Result<RaidDataDto>.Failure("Image URL is required");
            }

            _logger.LogInformation("Processing extract raid data command for URL: {ImageUrl}", request.ImageUrl);

            var result = await _ocrService.ExtractRaidDataAsync(request.ImageUrl, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to extract raid data: {Error}", result.Error);
                return Result<RaidDataDto>.Failure(result.Error ?? "Failed to extract raid data");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling extract raid data command");
            return Result<RaidDataDto>.Failure($"Error processing command: {ex.Message}");
        }
    }
}

