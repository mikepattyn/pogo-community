using Gym.Service.Application.Commands;
using Gym.Service.Application.Interfaces;
using Gym.Service.Application.Queries;
using Gym.Service.Infrastructure.Data;
using Gym.Service.Infrastructure.Repositories;
using Gym.Service.Infrastructure.Services;
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
builder.Services.AddDbContext<GymDbContext>(options =>
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
builder.Services.AddScoped<IGymRepository, GymRepository>();

// Add HTTP client for Location service
builder.Services.AddHttpClient<ILocationServiceClient, LocationServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LocationService:BaseUrl"] ?? "http://localhost:5003");
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
    var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
