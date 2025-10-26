using Bot.Service.Application.Services;
using Bot.Service.Application.Interfaces;
using Bot.Service.Infrastructure.Clients;
using Bot.Service.Infrastructure.HealthChecks;
using Pogo.Shared.API;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks()
    .AddCheck<DiscordBotHealthCheck>("discord-bot", tags: new[] { "ready" });

// Add HttpClient for Bot.BFF communication
builder.Services.AddHttpClient<BotBffClient>();

// Add Discord bot services
builder.Services.AddSingleton<DiscordBotService>();
builder.Services.AddSingleton<IBotBffClient, BotBffClient>();
builder.Services.AddSingleton<DiscordMetricsService>();

// Add command and service registrations
builder.Services.AddScoped<IRaidService, RaidService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

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

// Register the hosted service
builder.Services.AddHostedService<DiscordBotService>(sp => sp.GetRequiredService<DiscordBotService>());

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
