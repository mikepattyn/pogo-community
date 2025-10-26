using OCR.Service.Application.Interfaces;
using System.Drawing;
using System.Net.Http;
using Tesseract;

namespace OCR.Service.Application.Services;

public class OCRService : IOCRService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OCRService> _logger;

    public OCRService(HttpClient httpClient, ILogger<OCRService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string[]> ExtractTextFromImageAsync(string imageUrl, string[] languageHints)
    {
        try
        {
            _logger.LogInformation("Starting OCR processing for image: {ImageUrl}", imageUrl);

            // Download image
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

            // Convert to bitmap
            using var memoryStream = new MemoryStream(imageBytes);
            using var bitmap = new Bitmap(memoryStream);

            // Initialize Tesseract engine
            using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);

            // Set language hints if provided
            if (languageHints?.Length > 0)
            {
                var languages = string.Join("+", languageHints.Select(hint =>
                    hint.ToLower() switch
                    {
                        "en" or "en-gb" => "eng",
                        "nl" or "nl-be" => "nld",
                        _ => "eng"
                    }));

                engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ");
            }

            // Process image
            using var img = PixConverter.ToPix(bitmap);
            using var page = engine.Process(img);

            var text = page.GetText();
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToArray();

            _logger.LogInformation("OCR processing completed. Extracted {LineCount} lines", lines.Length);

            return lines;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OCR for image: {ImageUrl}", imageUrl);
            throw;
        }
    }
}
