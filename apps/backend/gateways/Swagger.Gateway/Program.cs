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
        Title = "Please select a service from the dropdown",
        Version = "v1",
        Description = "Select a microservice from the 'Select a definition' dropdown in the top right to view its API documentation."
    });
});

// Add HttpClient for fetching Swagger specs with timeout configuration
builder.Services.AddHttpClient()
    .ConfigureHttpClientDefaults(b =>
    {
        b.ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    });

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

    // Add all microservice endpoints - using reverse proxy routes
    c.SwaggerEndpoint("/api/account-service/swagger/v1/swagger.json", "Account Service");
    c.SwaggerEndpoint("/api/player-service/swagger/v1/swagger.json", "Player Service");
    c.SwaggerEndpoint("/api/location-service/swagger/v1/swagger.json", "Location Service");
    c.SwaggerEndpoint("/api/gym-service/swagger/v1/swagger.json", "Gym Service");
    c.SwaggerEndpoint("/api/raid-service/swagger/v1/swagger.json", "Raid Service");
    c.SwaggerEndpoint("/api/ocr-service/swagger/v1/swagger.json", "OCR Service");
    c.SwaggerEndpoint("/api/bot-bff/swagger/v1/swagger.json", "Bot BFF");
    c.SwaggerEndpoint("/api/app-bff/swagger/v1/swagger.json", "App BFF");
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

// Reverse proxy routes for microservices - configurable via appsettings
var serviceRoutes = new Dictionary<string, string>
{
    { "account-service", app.Configuration["ServiceUrls:AccountService"] ?? "http://account-service:5001" },
    { "player-service", app.Configuration["ServiceUrls:PlayerService"] ?? "http://player-service:5002" },
    { "location-service", app.Configuration["ServiceUrls:LocationService"] ?? "http://location-service:5003" },
    { "gym-service", app.Configuration["ServiceUrls:GymService"] ?? "http://gym-service:5004" },
    { "raid-service", app.Configuration["ServiceUrls:RaidService"] ?? "http://raid-service:5005" },
    { "ocr-service", app.Configuration["ServiceUrls:OCRService"] ?? "http://ocr-service:5001" },
    { "bot-bff", app.Configuration["ServiceUrls:BotBFF"] ?? "http://bot-bff:6001" },
    { "app-bff", app.Configuration["ServiceUrls:AppBFF"] ?? "http://app-bff:6002" }
};

foreach (var (serviceName, serviceUrl) in serviceRoutes)
{
    app.Map($"/api/{serviceName}/{{**path}}", async (HttpContext context, IHttpClientFactory httpClientFactory, string path) =>
    {
        var client = httpClientFactory.CreateClient();
        var targetUrl = $"{serviceUrl}/{path}{context.Request.QueryString}";

        var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

        // Copy headers
        foreach (var header in context.Request.Headers)
        {
            if (!header.Key.StartsWith("Host", StringComparison.OrdinalIgnoreCase))
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        // Copy body for non-GET requests
        if (context.Request.Method != "GET" && context.Request.ContentLength > 0)
        {
            requestMessage.Content = new StreamContent(context.Request.Body);
            if (context.Request.ContentType != null)
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(context.Request.ContentType);
            }
        }

        var response = await client.SendAsync(requestMessage);

        context.Response.StatusCode = (int)response.StatusCode;

        // Copy response headers - preserve CORS and other important headers
        foreach (var header in response.Headers)
        {
            // Skip Transfer-Encoding as it's handled by ASP.NET Core
            if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) &&
                !header.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        // Copy content headers individually
        foreach (var header in response.Content.Headers)
        {
            if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
            }
            // Skip Content-Length and Transfer-Encoding to let ASP.NET Core handle
            else if (!header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) &&
                     !header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        // Read the response body as a byte array to avoid chunked encoding issues
        var responseBytes = await response.Content.ReadAsByteArrayAsync();
        await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
    });
}

app.MapControllers();

// Map health checks
app.MapHealthChecks();

// Map Prometheus metrics
app.MapMetrics();

app.Run();

