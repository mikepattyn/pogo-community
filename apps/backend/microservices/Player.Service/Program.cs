using Player.Service.Application.Commands;
using Player.Service.Application.Interfaces;
using Player.Service.Application.Queries;
using Player.Service.Infrastructure.Data;
using Player.Service.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Pogo.Shared.API;
using Pogo.Shared.Application;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Npgsql;
using Pogo.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add Entity Framework
builder.Services.AddDbContext<PlayerDbContext>(options =>
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
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

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
app.RunMigrationsAsync<PlayerDbContext>();

app.Run();
