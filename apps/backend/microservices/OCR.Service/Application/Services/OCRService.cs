using Google.GenAI;
using OCR.Service.Application.DTOs;
using OCR.Service.Application.Interfaces;
using Pogo.Shared.Kernel;
using System.Net.Http;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace OCR.Service.Application.Services;

public class OCRService : IOCRService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OCRService> _logger;
    private readonly IConfiguration _configuration;

    public OCRService(HttpClient httpClient, ILogger<OCRService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result<RaidDataDto>> ExtractRaidDataAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting OCR processing for image: {ImageUrl}", imageUrl);

            // Get API key from environment
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("GOOGLE_API_KEY environment variable is not set");
                return Result<RaidDataDto>.Failure("Google API key is not configured");
            }

            // Download image
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl, cancellationToken);
            _logger.LogInformation("Downloaded image, size: {Size} bytes", imageBytes.Length);

            // Resize image if needed (max height 1500px maintaining aspect ratio)
            var originalSize = imageBytes.Length;
            string originalDimensions;
            using (var originalImage = Image.Load(imageBytes))
            {
                originalDimensions = $"{originalImage.Width}x{originalImage.Height}";
            }

            imageBytes = await ResizeImageIfNeededAsync(imageBytes, maxHeight: 1500);

            var resizedSize = imageBytes.Length;
            string resizedDimensions;
            using (var resizedImage = Image.Load(imageBytes))
            {
                resizedDimensions = $"{resizedImage.Width}x{resizedImage.Height}";
            }

            if (originalSize != resizedSize)
            {
                _logger.LogInformation("Image resized from {Original} ({OriginalBytes} bytes) to {Resized} ({ResizedBytes} bytes), reduction: {Reduction}%",
                    originalDimensions, originalSize, resizedDimensions, resizedSize,
                    (int)((originalSize - resizedSize) * 100.0 / originalSize));
            }
            else
            {
                _logger.LogInformation("Image dimensions {Dimensions} ({Size} bytes) - no resizing needed", originalDimensions, originalSize);
            }

            // Initialize Gemini client
            var client = new Client(apiKey: apiKey);

            // Create prompt for structured JSON extraction
            var prompt = @"Extract the following data from this Pokemon Go raid screenshot and return as valid JSON only:

{
  ""pokemon_name"": ""<string, lowercase, no spaces>"",
  ""tier"": <integer 1-5>,
  ""gym_name"": ""<string>"",
  ""combat_power"": <integer>,
  ""time_remaining"": ""<string in HH:MM:SS or MM:SS format>"",
  ""group_type"": ""<string: 'private' or 'public'>""
}

Rules:
- gym_name: Full gym name as displayed in between the circle with image and the circle with arrow >
- tier: Number from 1-5 indicated by skull icons
- combat_power: The combat power number (usually 4-5 digits) prefixed with CP and right below the tier skulls
- pokemon_name: Extract Pokemon name exactly as shown, lowercase, no spaces right below the combat power
- time_remaining: Time format HH:MM:SS or MM:SS
- group_type: Extract ""private"" or ""public"" based on visible indicators

Return ONLY the JSON object, no additional text.";

            _logger.LogInformation("Calling Gemini Flash Vision API...");

            // Convert image bytes to base64
            var base64Image = Convert.ToBase64String(imageBytes);

            // Call Gemini API with image - format as data URI
            var response = await client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash-exp",
                contents: $"{prompt}\n\n<image src=\"data:image/jpeg;base64,{base64Image}\" />"
            );

            var responseText = response.Candidates[0].Content.Parts[0].Text;

            if (string.IsNullOrEmpty(responseText))
            {
                _logger.LogWarning("Empty response from Gemini API");
                return Result<RaidDataDto>.Failure("Failed to extract data from image");
            }

            _logger.LogInformation("API Response received. Length: {Length}", responseText.Length);
            _logger.LogInformation("First 200 chars of response: {Response}",
                responseText.Length > 200 ? responseText.Substring(0, 200) : responseText);

            // Try to extract JSON from response (might be wrapped in markdown code blocks)
            if (responseText.Contains("```json"))
            {
                responseText = responseText.Split("```json")[1].Split("```")[0].Trim();
            }
            else if (responseText.Contains("```"))
            {
                responseText = responseText.Split("```")[1].Split("```")[0].Trim();
            }

            _logger.LogInformation("Extracted JSON: {Json}", responseText);

            // Parse JSON response
            RaidDataDto? raidData;
            try
            {
                // Configure JsonSerializer to handle snake_case
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                raidData = JsonSerializer.Deserialize<RaidDataDto>(responseText, options);

                if (raidData == null)
                {
                    return Result<RaidDataDto>.Failure("Failed to parse API response");
                }
            }
            catch (JsonException e)
            {
                _logger.LogError(
                    e,
                    "Failed to parse API response as JSON. Response: {Response}",
                    responseText);
                return Result<RaidDataDto>.Failure("Failed to parse API response as JSON");
            }

            // Validate required fields
            var requiredFields = new[]
            {
                "pokemon_name",
                "tier",
                "gym_name",
                "combat_power",
                "time_remaining",
                "group_type",
            };
            foreach (var field in requiredFields)
            {
                var property = typeof(RaidDataDto).GetProperty(field.Replace("_", ""));
                if (property != null)
                {
                    var value = property.GetValue(raidData);
                    if (value == null || (value is string str && string.IsNullOrEmpty(str)))
                    {
                        _logger.LogWarning("Missing or empty field: {Field}", field);
                    }
                }
            }

            _logger.LogInformation(
                "Pokemon: {Pokemon}, Tier: {Tier}, Gym: {Gym}, CP: {CP}, Time: {Time}, Group: {Group}",
                raidData.PokemonName,
                raidData.Tier,
                raidData.GymName,
                raidData.CombatPower,
                raidData.TimeRemaining,
                raidData.GroupType);

            return Result<RaidDataDto>.Success(raidData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OCR for image: {ImageUrl}", imageUrl);
            return Result<RaidDataDto>.Failure($"Error processing image: {ex.Message}");
        }
    }

    private async Task<byte[]> ResizeImageIfNeededAsync(byte[] imageBytes, int maxHeight = 1500)
    {
        using var image = Image.Load(imageBytes);

        if (image.Height <= maxHeight)
            return imageBytes; // No resize needed

        // Calculate new dimensions maintaining aspect ratio
        int newHeight = maxHeight;
        int newWidth = (int)(image.Width * (maxHeight / (double)image.Height));

        // Resize image with high quality
        image.Mutate(x => x.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));

        // Convert to byte array as JPEG
        using var outputMs = new MemoryStream();
        await image.SaveAsJpegAsync(outputMs, new JpegEncoder { Quality = 90 });
        return outputMs.ToArray();
    }
}
