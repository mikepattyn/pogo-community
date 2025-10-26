namespace OCR.Service.Application.Interfaces;

public interface IOCRService
{
    Task<string[]> ExtractTextFromImageAsync(string imageUrl, string[] languageHints);
}
