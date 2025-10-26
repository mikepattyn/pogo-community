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

    // Add all microservice endpoints - using reverse proxy routes
    c.SwaggerEndpoint("/api/account-service/swagger/v1/swagger.json", "Account Service");
    c.SwaggerEndpoint("/api/player-service/swagger/v1/swagger.json", "Player Service");
    c.SwaggerEndpoint("/api/location-service/swagger/v1/swagger.json", "Location Service");
    c.SwaggerEndpoint("/api/gym-service/swagger/v1/swagger.json", "Gym Service");
    c.SwaggerEndpoint("/api/raid-service/swagger/v1/swagger.json", "Raid Service");
    c.SwaggerEndpoint("/api/bot-bff/swagger/v1/swagger.json", "Bot BFF");
    c.SwaggerEndpoint("/api/app-bff/swagger/v1/swagger.json", "App BFF");
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

// Reverse proxy routes for microservices
var serviceRoutes = new Dictionary<string, string>
{
    { "account-service", "http://account-service:5001" },
    { "player-service", "http://player-service:5002" },
    { "location-service", "http://location-service:5003" },
    { "gym-service", "http://gym-service:5004" },
    { "raid-service", "http://raid-service:5005" },
    { "bot-bff", "http://bot-bff:6001" },
    { "app-bff", "http://app-bff:6002" }
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
        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        await response.Content.CopyToAsync(context.Response.Body);
    });
}

app.MapControllers();

// Map health checks
app.MapHealthChecks();

// Map Prometheus metrics
app.MapMetrics();

app.Run();

