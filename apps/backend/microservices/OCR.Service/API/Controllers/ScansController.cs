using Microsoft.AspNetCore.Mvc;
using OCR.Service.Application.DTOs;
using OCR.Service.Application.Interfaces;

namespace OCR.Service.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ScansController : ControllerBase
{
    private readonly IOCRService _ocrService;
    private readonly ILogger<ScansController> _logger;

    public ScansController(IOCRService ocrService, ILogger<ScansController> logger)
    {
        _ocrService = ocrService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ScanImage([FromBody] ScanImageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Url))
            {
                _logger.LogWarning("Scan request received with empty URL");
                return BadRequest(new { error = "URL is required" });
            }

            _logger.LogInformation("Processing scan request for URL: {Url}", request.Url);

            // Language hints matching the original implementation
            var languageHints = new[] { "en", "en-GB", "nl", "nl-BE" };

            var textResults = await _ocrService.ExtractTextFromImageAsync(request.Url, languageHints);

            if (textResults == null || textResults.Length == 0)
            {
                _logger.LogWarning("No text extracted from image: {Url}", request.Url);
                return BadRequest(new { error = "No text could be extracted from the image" });
            }

            var response = new ScanImageResponse
            {
                TextResults = textResults
            };

            _logger.LogInformation("Successfully processed scan request. Extracted {Count} text lines", textResults.Length);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scan request for URL: {Url}", request.Url);
            return StatusCode(500, new { error = "Internal server error during OCR processing" });
        }
    }
}
