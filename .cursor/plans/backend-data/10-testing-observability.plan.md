# Testing and Observability

## Overview

Create comprehensive integration tests, add structured logging and metrics to track system health, and implement robust error handling for production reliability.

## Problem Statement

Current gaps:
- No integration tests for new BFF endpoints
- No tests for full scan flow (image → OCR → raid creation)
- Limited logging for debugging orchestration failures
- No metrics to track success rates and performance
- Error handling may not provide actionable feedback

## Tasks

### 1. Create Bruno Collection Tests

**Directory**: `bruno/bffs/bot-bff/`

**Test 1: Scan Flow**

**File**: `bruno/bffs/bot-bff/Scan Raid.bru`

```
meta {
  name: Scan Raid
  type: http
  seq: 1
}

post {
  url: {{baseUrl}}/api/v1/scan
  body: json
  auth: none
}

body:json {
  {
    "imageUrl": "https://example.com/raid-screenshot.png",
    "tier": 5,
    "discordMessageId": "123456789012345678",
    "discordChannelId": "987654321098765432",
    "discordGuildId": "111222333444555666",
    "discordUserId": "777888999000111222"
  }
}

assert {
  res.status: eq 201
  res.body.raidId: isDefined
  res.body.gymId: isDefined
  res.body.pokemonSpecies: isDefined
}
```

**Test 2: Player Registration with Emoji**

**File**: `bruno/bffs/bot-bff/Register Player with Emoji.bru`

```
meta {
  name: Register Player with Emoji
  type: http
  seq: 2
}

post {
  url: {{baseUrl}}/api/v1/player-registration
  body: json
  auth: none
}

body:json {
  {
    "discordUserId": "123456789012345678",
    "username": "TestUser",
    "nickname": "🔥RaidMaster🔥",
    "teamColor": "Mystic",
    "level": 40
  }
}

assert {
  res.status: eq 201
  res.body.playerId: isDefined
  res.body.nickname: eq 🔥RaidMaster🔥
}
```

**Test 3: Raid Participation Lifecycle**

**File**: `bruno/bffs/bot-bff/Raid Participation Lifecycle.bru`

Test sequence:
1. Create raid (via scan or direct creation)
2. Register player
3. Join raid
4. Verify participant list
5. Leave raid
6. Verify participant list updated
7. Join again
8. Complete raid
9. Verify raid status

### 2. Add Structured Logging

**Files**: All BFF controllers and services

**ScanController logging**:

```csharp
[HttpPost("scan")]
public async Task<IActionResult> ScanRaidAsync([FromBody] ScanRaidRequest request)
{
    _logger.LogInformation(
        "Starting raid scan. MessageId={MessageId}, Tier={Tier}, UserId={UserId}",
        request.DiscordMessageId,
        request.Tier,
        request.DiscordUserId);
    
    try
    {
        // Call OCR
        _logger.LogDebug("Calling OCR service for image: {ImageUrl}", request.ImageUrl);
        var ocrResult = await _ocrClient.ScanImageAsync(request.ImageUrl);
        
        _logger.LogInformation(
            "OCR completed. Confidence={Confidence}, Text={Text}",
            ocrResult.Confidence,
            ocrResult.Text);
        
        // Resolve gym
        _logger.LogDebug("Resolving gym name: {GymName}", parsedGymName);
        var gymId = await _gymResolutionService.ResolveGymAsync(parsedGymName);
        
        if (gymId == null)
        {
            _logger.LogWarning(
                "Gym resolution failed. GymName={GymName}, MessageId={MessageId}",
                parsedGymName,
                request.DiscordMessageId);
            return BadRequest(new { error = "Could not resolve gym name" });
        }
        
        _logger.LogInformation("Gym resolved. GymId={GymId}", gymId);
        
        // Create raid
        _logger.LogDebug("Creating raid in backend");
        var raid = await _raidClient.CreateRaidAsync(createRaidDto);
        
        _logger.LogInformation(
            "Raid created successfully. RaidId={RaidId}, GymId={GymId}, Pokemon={Pokemon}",
            raid.Id,
            raid.GymId,
            raid.PokemonSpecies);
        
        return CreatedAtAction(nameof(GetRaid), new { id = raid.Id }, raid);
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Raid scan failed. MessageId={MessageId}, Error={Error}",
            request.DiscordMessageId,
            ex.Message);
        
        return StatusCode(500, new { error = "Raid scan failed", details = ex.Message });
    }
}
```

**Key logging points**:
- Request start with key parameters
- OCR results and confidence scores
- Gym resolution attempts and results
- Service call successes/failures
- Final outcomes with IDs
- Errors with context

### 3. Add Prometheus Metrics

**File**: `apps/backend/bffs/Bot.BFF/Program.cs`

**Setup**:

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ... other services ...

var app = builder.Build();

// Prometheus metrics endpoint
app.UseMetricServer(); // Exposes /metrics endpoint
app.UseHttpMetrics();  // Tracks HTTP request metrics

app.Run();
```

**Custom metrics in controllers**:

```csharp
using Prometheus;

public class ScanController : ControllerBase
{
    private static readonly Counter ScanAttempts = Metrics.CreateCounter(
        "raid_scan_attempts_total",
        "Total number of raid scan attempts");
    
    private static readonly Counter ScanSuccesses = Metrics.CreateCounter(
        "raid_scan_successes_total",
        "Total number of successful raid scans");
    
    private static readonly Counter ScanFailures = Metrics.CreateCounter(
        "raid_scan_failures_total",
        "Total number of failed raid scans",
        new CounterConfiguration { LabelNames = new[] { "reason" } });
    
    private static readonly Histogram ScanDuration = Metrics.CreateHistogram(
        "raid_scan_duration_seconds",
        "Duration of raid scan operations");
    
    private static readonly Gauge ActiveRaids = Metrics.CreateGauge(
        "active_raids_total",
        "Current number of active raids");
    
    [HttpPost("scan")]
    public async Task<IActionResult> ScanRaidAsync([FromBody] ScanRaidRequest request)
    {
        ScanAttempts.Inc();
        
        using (ScanDuration.NewTimer())
        {
            try
            {
                var result = await PerformScanAsync(request);
                ScanSuccesses.Inc();
                ActiveRaids.Inc();
                return Ok(result);
            }
            catch (OcrException)
            {
                ScanFailures.WithLabels("ocr_failed").Inc();
                throw;
            }
            catch (GymResolutionException)
            {
                ScanFailures.WithLabels("gym_resolution_failed").Inc();
                throw;
            }
            catch (Exception)
            {
                ScanFailures.WithLabels("unknown").Inc();
                throw;
            }
        }
    }
}
```

**Metrics to track**:
- Scan attempts, successes, failures (by reason)
- Scan duration (histogram)
- Active raids count
- Player registration rate
- Raid participation events
- API latencies per endpoint
- Error rates by type

### 4. Implement Error Handling

**Create error response DTOs**:

**File**: `apps/backend/bffs/Bot.BFF/DTOs/ErrorResponse.cs`

```csharp
public class ErrorResponse
{
    public string Error { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
    public Dictionary<string, string[]> ValidationErrors { get; set; }
}
```

**Implement global exception handler**:

**File**: `apps/backend/bffs/Bot.BFF/Middleware/GlobalExceptionHandler.cs`

```csharp
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GymNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound);
        }
        catch (RaidFullException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status409Conflict);
        }
        catch (PlayerNotRegisteredException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse
        {
            Error = ex.GetType().Name,
            Message = ex.Message,
            Details = _environment.IsDevelopment() ? ex.StackTrace : null
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
}
```

**Specific error scenarios**:
- OCR confidence too low → Suggest manual confirmation
- Multiple gym matches → Return list for user selection
- Raid full → Clear message with current capacity
- Player not registered → Prompt registration
- Raid already completed → Cannot join

### 5. Implement Retry Logic

**File**: `apps/backend/bffs/Bot.BFF/Services/ResilientHttpClient.cs`

**Use Polly for retries**:

```csharp
using Polly;
using Polly.Extensions.Http;

public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// In Program.cs
builder.Services.AddHttpClient<IRaidClient, RaidClient>()
    .AddPolicyHandler(ResilientHttpClient.GetRetryPolicy());
```

### 6. Create Health Checks

**File**: `apps/backend/bffs/Bot.BFF/Program.cs`

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<OcrServiceHealthCheck>("ocr_service")
    .AddCheck<RaidServiceHealthCheck>("raid_service")
    .AddCheck<PlayerServiceHealthCheck>("player_service");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

## Verification

- [ ] Bruno tests for scan flow pass
- [ ] Bruno tests for player registration with emojis pass
- [ ] Bruno tests for raid participation lifecycle pass
- [ ] Structured logging captures all key events
- [ ] Prometheus metrics are exposed at /metrics
- [ ] Metrics track success rates and latencies
- [ ] Error responses are meaningful and actionable
- [ ] Retry logic handles transient failures
- [ ] Health checks report service status
- [ ] Grafana dashboard shows key metrics (optional)

## Dependencies

- Requires: All previous todos (1-9)
- This is the final validation step

## Files to Create/Modify

**New Test Files**:
- `bruno/bffs/bot-bff/Scan Raid.bru`
- `bruno/bffs/bot-bff/Register Player with Emoji.bru`
- `bruno/bffs/bot-bff/Raid Participation Lifecycle.bru`

**New Middleware/Services**:
- `apps/backend/bffs/Bot.BFF/Middleware/GlobalExceptionHandler.cs`
- `apps/backend/bffs/Bot.BFF/Services/ResilientHttpClient.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/ErrorResponse.cs`

**Modified Files**:
- All BFF controllers (add logging and metrics)
- `apps/backend/bffs/Bot.BFF/Program.cs` (add metrics, health checks)
- All microservice controllers (add logging)

**NuGet Packages to Add**:
- `prometheus-net.AspNetCore`
- `Microsoft.Extensions.Diagnostics.HealthChecks`
- `Polly`
- `Polly.Extensions.Http`

## Observability Best Practices

1. **Log levels**:
   - Debug: Detailed flow information
   - Information: Key business events
   - Warning: Recoverable issues
   - Error: Failures requiring attention

2. **Metrics naming**:
   - Use snake_case for Prometheus compatibility
   - Include units in name (e.g., `_seconds`, `_total`)
   - Use consistent prefixes (e.g., `raid_`, `player_`)

3. **Error messages**:
   - User-facing: Clear, actionable
   - Logs: Detailed with context
   - Never expose sensitive data

