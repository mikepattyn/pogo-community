using OCR.Service.Application.DTOs;
using Pogo.Shared.Kernel;

namespace OCR.Service.Application.Interfaces;

public interface IOCRService
{
    Task<Result<RaidDataDto>> ExtractRaidDataAsync(string imageUrl, CancellationToken cancellationToken = default);
}
