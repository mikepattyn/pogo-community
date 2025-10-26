using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Pogo.Shared.API;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot
builder.Services.AddOcelot();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks using custom extension
builder.Services.AddHealthChecks("");

// Add CORS for Mobile App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3001", "https://localhost:3001") // Mobile app
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
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

app.UseCors("AppPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Map health checks BEFORE Ocelot
app.MapHealthChecks();

app.MapControllers();

// Use Ocelot only for non-health check paths
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/health"), appBuilder =>
{
    appBuilder.UseOcelot();
});

app.Run();
