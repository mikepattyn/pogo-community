using Raid.Service.Application.Commands;
using Raid.Service.Application.Interfaces;
using Raid.Service.Application.Queries;
using Raid.Service.Infrastructure.Data;
using Raid.Service.Infrastructure.Repositories;
using Raid.Service.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Pogo.Shared.API;
using Pogo.Shared.Application;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks (explicitly configure to avoid automatic database checks)
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "self" });

// Add Entity Framework
builder.Services.AddDbContext<RaidDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Add repositories and services
builder.Services.AddScoped<IRaidRepository, RaidRepository>();

// Add HTTP clients for other services
builder.Services.AddHttpClient<IGymServiceClient, GymServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GymService:BaseUrl"] ?? "http://localhost:5004");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPlayerServiceClient, PlayerServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PlayerService:BaseUrl"] ?? "http://localhost:5002");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add health checks
builder.Services.AddHealthChecks(builder.Configuration.GetConnectionString("DefaultConnection")!);

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

// Add health check middleware
app.UseHealthChecks("/health");

app.MapControllers();
app.MapMetrics();

// Map health checks
app.MapHealthChecks();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RaidDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
