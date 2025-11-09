using Bot.BFF.Services;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Pogo.Shared.API;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot with configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Typed HTTP clients for downstream microservices
builder.Services.AddHttpClient<IPlayerServiceClient, PlayerServiceClient>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Services:PlayerService:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("Player service base URL is not configured.");
    }

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHttpClient<IRaidServiceClient, RaidServiceClient>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Services:RaidService:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("Raid service base URL is not configured.");
    }

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Add health checks using custom extension
builder.Services.AddHealthChecks("");

// Add CORS for Bot
builder.Services.AddCors(options =>
{
    options.AddPolicy("BotPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000") // Bot frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("BotPolicy");

// Prometheus metrics
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

// Map health checks BEFORE Ocelot
app.MapHealthChecks();

app.MapControllers();

// Map Prometheus metrics
app.MapMetrics();

// Use Ocelot only for non-health check and non-metrics paths
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/health") &&
                      !context.Request.Path.StartsWithSegments("/metrics"), appBuilder =>
{
    appBuilder.UseOcelot();
});

app.Run();
