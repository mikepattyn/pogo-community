using OCR.Service.Application.Interfaces;
using OCR.Service.Application.Services;
using Pogo.Shared.API;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks();

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

app.MapControllers();
app.MapMetrics();

// Map health checks
app.MapHealthChecks();

app.Run();
