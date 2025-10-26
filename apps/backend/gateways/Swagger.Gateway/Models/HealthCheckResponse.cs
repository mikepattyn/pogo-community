using System.Text.Json.Serialization;

namespace Swagger.Gateway.Models;

/// <summary>
/// Response model for health check endpoints
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Overall health status (Healthy, Degraded, Unhealthy)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Individual health check results
    /// </summary>
    [JsonPropertyName("checks")]
    public HealthCheckEntry[] Checks { get; set; } = Array.Empty<HealthCheckEntry>();

    /// <summary>
    /// Total duration of health checks in milliseconds
    /// </summary>
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
}

/// <summary>
/// Individual health check entry
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// Name of the health check
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Status of this specific check (Healthy, Degraded, Unhealthy)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Description of the check result
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Duration of this check in milliseconds
    /// </summary>
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
}
