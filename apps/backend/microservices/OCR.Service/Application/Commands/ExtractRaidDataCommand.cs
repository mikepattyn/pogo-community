using MediatR;
using OCR.Service.Application.DTOs;
using Pogo.Shared.Kernel;

namespace OCR.Service.Application.Commands;

/// <summary>
/// Command to extract structured raid data from an image URL using Gemini Vision API
/// </summary>
public class ExtractRaidDataCommand : IRequest<Result<RaidDataDto>>
{
    public string ImageUrl { get; set; } = string.Empty;
}

