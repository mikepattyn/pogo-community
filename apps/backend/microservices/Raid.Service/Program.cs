using System.Linq;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using Pogo.Shared.API;
using Pogo.Shared.Application;
using Pogo.Shared.Infrastructure;
using Prometheus;
using Raid.Service.Application.Commands;
using Raid.Service.Application.Interfaces;
using Raid.Service.Application.Queries;
using Raid.Service.Infrastructure.Data;
using Raid.Service.Infrastructure.Repositories;
using Raid.Service.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add Entity Framework
builder.Services.AddDbContext<RaidDbContext>(options =>
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
builder.Services.AddScoped<IRaidRepository, RaidRepository>();

// Add HTTP clients for other services
builder.Services.AddHttpClient<IGymServiceClient, GymServiceClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["GymService:BaseUrl"] ?? "http://localhost:5004"
    );
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPlayerServiceClient, PlayerServiceClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["PlayerService:BaseUrl"] ?? "http://localhost:5002"
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
app.RunMigrationsAsync<RaidDbContext>();

app.Run();
