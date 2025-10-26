using Pogo.Shared.API;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "POGO Swagger Gateway",
        Version = "v1",
        Description = "Unified Swagger documentation for all POGO microservices"
    });
});

// Add HttpClient for fetching Swagger specs
builder.Services.AddHttpClient();

// Add health checks using custom extension
builder.Services.AddHealthChecks("");

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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Gateway API");
    c.RoutePrefix = "swagger";
    
    // Add all microservice endpoints - using proxy endpoints to avoid CORS issues
    c.SwaggerEndpoint("/proxy/account-service/swagger/v1/swagger.json", "Account Service");
    c.SwaggerEndpoint("/proxy/player-service/swagger/v1/swagger.json", "Player Service");
    c.SwaggerEndpoint("/proxy/location-service/swagger/v1/swagger.json", "Location Service");
    c.SwaggerEndpoint("/proxy/gym-service/swagger/v1/swagger.json", "Gym Service");
    c.SwaggerEndpoint("/proxy/raid-service/swagger/v1/swagger.json", "Raid Service");
    c.SwaggerEndpoint("/proxy/bot-bff/swagger/v1/swagger.json", "Bot BFF");
    c.SwaggerEndpoint("/proxy/app-bff/swagger/v1/swagger.json", "App BFF");
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

// Proxy endpoints for Swagger specs (to avoid CORS issues)
app.MapGet("/proxy/account-service/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://account-service:5001/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyAccountService");

app.MapGet("/proxy/player-service/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://player-service:5002/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyPlayerService");

app.MapGet("/proxy/location-service/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://location-service:5003/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyLocationService");

app.MapGet("/proxy/gym-service/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://gym-service:5004/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyGymService");

app.MapGet("/proxy/raid-service/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://raid-service:5005/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyRaidService");

app.MapGet("/proxy/bot-bff/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://bot-bff:6001/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyBotBff");

app.MapGet("/proxy/app-bff/swagger/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync("http://app-bff:6002/swagger/v1/swagger.json");
    return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
}).WithName("ProxyAppBff");

app.MapControllers();

// Map health checks
app.MapHealthChecks();

// Map Prometheus metrics
app.MapMetrics();

app.Run();

