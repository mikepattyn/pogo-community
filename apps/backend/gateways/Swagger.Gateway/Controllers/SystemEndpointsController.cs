using Microsoft.AspNetCore.Mvc;
using Swagger.Gateway.Models;

namespace Swagger.Gateway.Controllers;

/// <summary>
/// System endpoints for health checks and metrics across all services
/// </summary>
[ApiController]
[Route("api")]
[ApiExplorerSettings(GroupName = "system")]
public class SystemEndpointsController : ControllerBase
{
    /// <summary>
    /// Get health readiness status for Account Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("account-service/health/ready")]
    public IActionResult GetAccountServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Account Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("account-service/health/live")]
    public IActionResult GetAccountServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Account Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("account-service/metrics")]
    public IActionResult GetAccountServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for Player Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("player-service/health/ready")]
    public IActionResult GetPlayerServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Player Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("player-service/health/live")]
    public IActionResult GetPlayerServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Player Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("player-service/metrics")]
    public IActionResult GetPlayerServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for Location Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("location-service/health/ready")]
    public IActionResult GetLocationServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Location Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("location-service/health/live")]
    public IActionResult GetLocationServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Location Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("location-service/metrics")]
    public IActionResult GetLocationServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for Gym Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("gym-service/health/ready")]
    public IActionResult GetGymServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Gym Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("gym-service/health/live")]
    public IActionResult GetGymServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Gym Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("gym-service/metrics")]
    public IActionResult GetGymServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for Raid Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("raid-service/health/ready")]
    public IActionResult GetRaidServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Raid Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("raid-service/health/live")]
    public IActionResult GetRaidServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Raid Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("raid-service/metrics")]
    public IActionResult GetRaidServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for OCR Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("ocr-service/health/ready")]
    public IActionResult GetOCRServiceHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for OCR Service
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("ocr-service/health/live")]
    public IActionResult GetOCRServiceHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for OCR Service
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("ocr-service/metrics")]
    public IActionResult GetOCRServiceMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for Bot BFF
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("bot-bff/health/ready")]
    public IActionResult GetBotBFFHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for Bot BFF
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("bot-bff/health/live")]
    public IActionResult GetBotBFFHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for Bot BFF
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("bot-bff/metrics")]
    public IActionResult GetBotBFFMetrics() => Ok();

    /// <summary>
    /// Get health readiness status for App BFF
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("app-bff/health/ready")]
    public IActionResult GetAppBFFHealthReady() => Ok();

    /// <summary>
    /// Get health liveness status for App BFF
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet("app-bff/health/live")]
    public IActionResult GetAppBFFHealthLive() => Ok();

    /// <summary>
    /// Get Prometheus metrics for App BFF
    /// </summary>
    /// <returns>Prometheus metrics in text format</returns>
    [HttpGet("app-bff/metrics")]
    public IActionResult GetAppBFFMetrics() => Ok();
}
