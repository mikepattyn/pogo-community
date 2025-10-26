namespace OCR.Service.Application.DTOs;

public class ScanImageRequest
{
    public string Url { get; set; } = string.Empty;
}

public class ScanImageResponse
{
    public string[] TextResults { get; set; } = Array.Empty<string>();
}
