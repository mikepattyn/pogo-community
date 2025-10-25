using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Pogo.Shared.API;

/// <summary>
/// Extensions for configuring Swagger/OpenAPI
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="title">API title</param>
    /// <param name="version">API version</param>
    /// <param name="description">API description</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services, string title, string version, string description)
    {
        services.AddEndpointsApiExplorer();
        // Swagger configuration will be added in individual microservices
        return services;
    }

    /// <summary>
    /// Configures Swagger middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="title">API title</param>
    /// <param name="version">API version</param>
    /// <returns>The web application</returns>
    public static WebApplication UseSwagger(this WebApplication app, string title, string version)
    {
        // Swagger configuration will be added in individual microservices
        return app;
    }
}