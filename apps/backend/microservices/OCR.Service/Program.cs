using MediatR;
using OCR.Service.Application.Commands;
using OCR.Service.Application.DTOs;
using OCR.Service.Application.Interfaces;
using OCR.Service.Application.Services;
using Pogo.Shared.API;
using Pogo.Shared.Kernel;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add HttpClient for downloading images
builder.Services.AddHttpClient<OCRService>();

// Add OCR service
builder.Services.AddScoped<IOCRService, OCRService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

app.MapMetrics();

// Map health checks
app.MapHealthChecks();

// Minimal API endpoints
app.MapPost("/api/v1/scans", async (ScanImageRequest request, IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    try
    {
        if (string.IsNullOrEmpty(request.Url))
        {
            logger.LogWarning("Scan request received with empty URL");
            return Results.BadRequest(new { error = "URL is required" });
        }

        logger.LogInformation("Processing scan request for URL: {Url}", request.Url);

        var command = new ExtractRaidDataCommand
        {
            ImageUrl = request.Url
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to extract raid data: {Error}", result.Error);
            return Results.BadRequest(new { error = result.Error ?? "Failed to extract raid data" });
        }

        var response = new ScanImageResponse
        {
            RaidData = result.Value!
        };

        logger.LogInformation("Successfully processed scan request for URL: {Url}", request.Url);
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing scan request for URL: {Url}", request.Url);
        return Results.StatusCode(500);
    }
});

app.Run();
