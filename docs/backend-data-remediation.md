# Backend Data Remediation Plan

## Context
- Discord bot commands already reach the Bot BFF and microservices; database containers start and apply migrations successfully.
- OCR-based raid scans currently only create Discord embeds; no records are persisted in `RaidDb`.
- Player registration through emojis fails because the Bot client contract does not match the Player microservice API and Unicode handling is unverified.

## Current Findings

### Raid ingestion stops at the Discord layer
- The scan command calls OCR, builds an embed, and exits without persisting anything to the backend.

```60:112:apps/frontend/bot/Application/Commands/ScanCommand.cs
// ... existing code ...
var scanResponse = await _botBffClient.ScanImageAsync(scanRequest);
// ... existing code ...
var raidInfo = ParseRaidInfo(scanResponse.TextResults, tier);
// ... existing code ...
await processingMessage.AddReactionAsync(new Emoji("👍"));
// ... existing code ...
```

- Raid state is cached in-memory; the Bot never calls the Raid microservice. The persistence hook is commented out.

```19:76:apps/frontend/bot/Application/Services/RaidService.cs
// ... existing code ...
_raids[messageId] = raid;

// Optionally save to backend via Bot.BFF
// await _botBffClient.CreateRaidAsync(raid);
// ... existing code ...
```

### Bot BFF contract diverges from microservices
- The Bot client calls `/api/player/v1/players` and `/api/raid/v1/raids`, but downstream controllers expose `/api/player` and `/api/raid` without versioned segments.

```20:131:apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs
// ... existing code ...
await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/player/v1/players", player, cancellationToken);
// ... existing code ...
await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/raid/v1/raids", raidData, cancellationToken);
// ... existing code ...
```

- The Player microservice expects different DTOs (e.g., `Username`, `FriendCode`) and routes.

```10:52:apps/backend/microservices/Player.Service/API/Controllers/PlayerController.cs
[ApiController]
[Route("api/[controller]")]
// ... existing code ...
public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto request, CancellationToken cancellationToken)
// ... existing code ...
```

### Raid microservice functionality is incomplete
- Only `CreateRaidCommandHandler` exists; join/leave/complete/cancel commands and query handlers are declared but unimplemented. Participants cannot be persisted or fetched, so even a fixed Bot client would fail during those calls.
- `RaidDbContext` models raids but has no join table for per-player participation.

### Emoji registration is unsupported end-to-end
- Upstream registration collects Discord names and emojis, but the payload never reaches the backend because of the contract mismatch.
- Database Unicode guarantees are not documented; we should confirm PostgreSQL collations and EF Core mappings support surrogate pairs used by emoji reactions.

## Required Work

### A. Align API contracts across Bot BFF and microservices
- Decide on a versioned URL pattern and update both `ocelot.json` and downstream controllers to match (or adjust the Bot client to current routes).
- Introduce Bot-facing DTOs that translate Discord-focused fields to backend requirements (e.g., derive `Username`, `FriendCode`, `GymId`).
- Harden Bot BFF endpoints so the bot never calls microservices directly; expose typed REST endpoints that validate payloads, log failures, and translate errors.

### B. Build the raid ingestion pipeline
- Replace the in-memory `RaidService` cache with calls to the Bot BFF; design a request model that captures OCR output, Discord message metadata, and guild context.
- Implement OCR parsing that maps gym names to IDs (via Gym/Location services) and infers raid times reliably; add fallback/manual confirmation paths when confidence is low.
- Complete the Raid microservice: add command/query handlers, participant persistence (new `RaidParticipant` entity), and integration with Player/Gym services.
- Ensure `RaidDbContext` uses EF Core migrations (remove `EnsureCreated`) and adds the new tables/constraints.

### C. Harden player registration and emoji support
- Update Bot registration flow to fetch/create Player records through the aligned Bot BFF API, including Discord IDs and emoji-rich nicknames.
- Confirm EF Core models store player-visible strings as Unicode (`nvarchar`/`text`) and add integration tests that create players with emoji names via the HTTP API.
- Review JSON serialization: configure ASP.NET `System.Text.Json` encoders to allow the emoji character set and round-trip Discord nicknames.

### D. Observability, resilience, and testing
- Add structured logging and Prometheus metrics around the new endpoints (e.g., OCR parse success rate, raid creation latency, emoji sign-up errors).
- Create contract/integration tests that exercise the full flow: OCR scan → Bot BFF → Raid service → database, and emoji registration → Player service.
- Document manual runbooks for reprocessing failed OCR scans and reconciling Discord reactions with persisted raid participation.

## Verification Plan
- Implement end-to-end tests using Bruno collections or integration test projects that call the Bot BFF routes.
- Validate database state directly (e.g., query `RaidDb` and `PlayerDb`) after bot interactions.
- Exercise Discord commands against a staging environment with logging enabled to ensure emojis persist.

## Open Questions
- How should gym names parsed from OCR be matched to canonical `GymId` values when multiple gyms share similar names?
- Do we require player consent before storing Discord nicknames containing emojis?
- Should raid participation be tied to Discord user IDs if the player has not yet registered through the Player service?

## Reference Files
- `apps/frontend/bot/Application/Commands/ScanCommand.cs`
- `apps/frontend/bot/Application/Services/RaidService.cs`
- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`
- `apps/backend/microservices/Raid.Service/*`
- `apps/backend/microservices/Player.Service/*`
