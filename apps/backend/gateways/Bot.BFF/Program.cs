using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// OCR Service endpoint - proxy to OCR microservice
app.MapPost("/api/v1/scans", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        // Read the request body
        using var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        // Get OCR service URL from configuration
        var ocrServiceUrl = builder.Configuration["OCRServiceUrl"] ?? "http://ocr-service:5001";

        // Forward request to OCR service
        var httpClient = httpClientFactory.CreateClient();
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{ocrServiceUrl}/api/v1/scans", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Return the response from OCR service
        context.Response.StatusCode = (int)response.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseContent);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
    }
})
.WithName("ScanImage");

app.Run();
