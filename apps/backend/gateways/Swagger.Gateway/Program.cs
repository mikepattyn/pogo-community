using Pogo.Shared.API;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://account-service:5001/swagger/v1/swagger.json", "Account Service");
        c.SwaggerEndpoint("http://player-service:5002/swagger/v1/swagger.json", "Player Service");
        c.SwaggerEndpoint("http://location-service:5003/swagger/v1/swagger.json", "Location Service");
        c.SwaggerEndpoint("http://gym-service:5004/swagger/v1/swagger.json", "Gym Service");
        c.SwaggerEndpoint("http://raid-service:5005/swagger/v1/swagger.json", "Raid Service");
        c.SwaggerEndpoint("http://bot-bff:6001/swagger/v1/swagger.json", "Bot BFF");
        c.SwaggerEndpoint("http://app-bff:6002/swagger/v1/swagger.json", "App BFF");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Prometheus metrics
app.UseHttpMetrics();

app.MapControllers();

// Map health checks
app.MapHealthChecks();

// Map Prometheus metrics
app.MapMetrics();

app.Run();

