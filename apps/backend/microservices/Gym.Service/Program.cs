using System.Linq;
using FluentValidation;
using Gym.Service.Application.Commands;
using Gym.Service.Application.Interfaces;
using Gym.Service.Application.Queries;
using Gym.Service.Infrastructure.Data;
using Gym.Service.Infrastructure.Repositories;
using Gym.Service.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using Pogo.Shared.API;
using Pogo.Shared.Application;
using Pogo.Shared.Infrastructure;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add Entity Framework
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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
builder.Services.AddScoped<IGymRepository, GymRepository>();

// Add HTTP client for Location service
builder.Services.AddHttpClient<ILocationServiceClient, LocationServiceClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["LocationService:BaseUrl"] ?? "http://localhost:5003"
    );
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Map health checks BEFORE HTTPS redirection to avoid redirect issues with K8s probes
app.MapHealthChecks();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

// Run database migrations asynchronously to avoid blocking startup
// This allows the HTTP server to start immediately while migrations run in background
app.RunMigrationsAsync<GymDbContext>();

app.Run();
