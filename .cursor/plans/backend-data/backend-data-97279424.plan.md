<!-- 97279424-caf1-4910-89e0-9cdd2c5dd84f 8a59ae1e-dafa-4d50-be6b-3d98cc3a4967 -->
# Backend Data Remediation - Revised Plan

## Overview

Fix backend data persistence by standardizing API versioning across all services, implementing proper BFF orchestration endpoints for bot commands, completing raid participation functionality, and ensuring Unicode support for emoji-based interactions.

## Core Issues Identified

### 1. API Versioning Inconsistency

- **OCR Service**: Uses `[Route("api/v1/[controller]")]` → `/api/v1/scans`
- **All other services**: Use `[Route("api/[controller]")]` → `/api/player`, `/api/raid`, etc.
- **Bot client**: Expects versioned routes like `/api/player/v1/players`, `/api/raid/v1/raids`
- **BFF Ocelot config**: Routes `/api/player/{everything}` → downstream `/api/{everything}`
- **Result**: 404 errors when bot tries to call player/raid endpoints

### 2. Bot BFF Architecture Misalignment

- **Current**: Bot BFF is pure Ocelot gateway with no custom controllers
- **Problem**: Bot calls OCR, parses locally, stores in memory only
- **Missing**: Orchestration endpoints that handle complete bot workflows
- **Example**: `/scan` command should trigger OCR → gym lookup → raid creation in one BFF call

### 3. Raid Persistence Disabled

- `RaidService.CreateRaidAsync` has commented line: `// await _botBffClient.CreateRaidAsync(raid);`
- Raids only stored in-memory dictionary `_raids[messageId]`
- No backend persistence means data lost on bot restart
- Discord message IDs not tracked in backend

### 4. Incomplete Raid Participation

- `RaidDbContext` has no `RaidParticipant` entity or join table
- Command handlers exist but unimplemented: `JoinRaidCommand`, `LeaveRaidCommand`, `CompleteRaidCommand`, `CancelRaidCommand`
- Query handler `GetRaidParticipantsQuery` declared but no implementation
- No way to persist which players joined which raids

### 5. Contract Mismatches

- **Bot client** sends: `CreatePlayerDto` with `DiscordId` field
- **Player service** expects: `CreatePlayerDto` with `DiscordUserId` field
- **Bot client** sends: Generic `object raidData`
- **Raid service** expects: `CreateRaidDto` with specific fields like `GymId`, `PokemonSpecies`, `StartTime`, `EndTime`

### 6. Unicode/Emoji Support Unverified

- No confirmation that database columns support emoji characters
- No JSON encoder configuration for emoji serialization
- Player registration with emoji nicknames untested

## Implementation Plan

### Phase 1: Standardize API Versioning

**1.1 Update all microservice controllers to use v1 routes**

- Change `[Route("api/[controller]")]` to `[Route("api/v1/[controller]")]` in:
  - `Account.Service/API/Controllers/AccountController.cs`
  - `Player.Service/API/Controllers/PlayerController.cs`
  - `Raid.Service/API/Controllers/RaidController.cs`
  - `Gym.Service/API/Controllers/GymController.cs`
  - `Location.Service/API/Controllers/LocationController.cs`
- OCR service already correct at `api/v1/[controller]`

**1.2 Update BFF Ocelot routing**

- Modify `Bot.BFF/ocelot.json` and `App.BFF/ocelot.json`
- Change upstream patterns from `/api/{service}/{everything}` to `/api/v1/{service}/{everything}`
- Keep downstream as `/api/v1/{everything}` to match new controller routes

**1.3 Update bot client calls**

- Adjust `BotBffClient.cs` URLs from `/api/player/v1/players` to `/api/v1/player/players`
- Pattern: `/api/v1/{service}/{resource}` instead of `/api/{service}/v1/{resource}`

### Phase 2: Implement Bot BFF Orchestration Endpoints

**2.1 Create custom controllers in Bot.BFF**

- Add `Controllers/ScanController.cs` with `POST /api/v1/scan` endpoint
- Add `Controllers/PlayerRegistrationController.cs` with `POST /api/v1/player-registration` endpoint
- Add `Controllers/RaidParticipationController.cs` with join/leave endpoints

**2.2 Implement scan orchestration**

- `ScanController.ScanRaidAsync` should:

  1. Call OCR service to extract text from image
  2. Parse OCR results to identify gym name, Pokemon, tier, time
  3. Call Gym/Location services to resolve gym name → GymId
  4. Call Raid service to create raid with Discord message metadata
  5. Return complete raid DTO to bot including raid ID for tracking

- Handle errors gracefully with fallback/manual confirmation paths

**2.3 Implement player registration orchestration**

- `PlayerRegistrationController.RegisterPlayerAsync` should:

  1. Accept Discord user data (ID, username, emoji nickname)
  2. Validate and transform to Player service DTOs
  3. Call Player service to create/update player
  4. Return player ID for future raid participation

- Ensure emoji-rich nicknames preserved

**2.4 Implement raid participation orchestration**

- `RaidParticipationController` endpoints:
  - `POST /api/v1/raids/{raidId}/join` - join raid
  - `POST /api/v1/raids/{raidId}/leave` - leave raid
  - `POST /api/v1/raids/{raidId}/complete` - mark complete
  - `POST /api/v1/raids/{raidId}/cancel` - cancel raid
- Each should call Raid service and handle player lookups

### Phase 3: Complete Raid Microservice Functionality

**3.1 Create RaidParticipant entity and migration**

- Add `Domain/Entities/RaidParticipant.cs` with:
  - `Id`, `RaidId`, `PlayerId`, `JoinedAt`, `ExtraPlayers`, `Status`
- Update `RaidDbContext.cs` to include `DbSet<RaidParticipant>`
- Configure entity relationships (Raid → many RaidParticipants)
- Create EF Core migration for new table
- Remove any `Database.EnsureCreated()` calls

**3.2 Implement command handlers**

- `JoinRaidCommandHandler`: Add player to raid, increment `CurrentParticipants`, check max capacity
- `LeaveRaidCommandHandler`: Remove player from raid, decrement `CurrentParticipants`
- `CompleteRaidCommandHandler`: Mark raid as completed, set `IsActive = false`, `IsCompleted = true`
- `CancelRaidCommandHandler`: Mark raid as cancelled, set `IsActive = false`, `IsCancelled = true`

**3.3 Implement query handlers**

- `GetRaidParticipantsQueryHandler`: Fetch all participants for a raid with player details
- Consider adding `GetPlayerRaidsQuery` for player history

**3.4 Add Discord metadata to Raid entity**

- Add fields: `DiscordMessageId`, `DiscordChannelId`, `DiscordGuildId`, `DiscordUserId` (creator)
- Update `CreateRaidDto` and `CreateRaidCommand` to accept these fields
- Update migration and `RaidDbContext` configuration

### Phase 4: Fix Bot Integration

**4.1 Update bot DTOs**

- Align `CreatePlayerDto` field names with Player service expectations
- Create proper `CreateRaidDto` matching Raid service contract
- Remove generic `object` parameters in `BotBffClient`

**4.2 Enable raid persistence**

- Uncomment and fix `await _botBffClient.CreateRaidAsync(raid)` in `RaidService.cs`
- Update to call new BFF scan endpoint instead
- Store returned raid ID from backend
- Keep in-memory cache for quick lookups but sync with backend

**4.3 Update ScanCommand to use BFF orchestration**

- Change `ScanCommand.cs` to call new `/api/v1/scan` BFF endpoint
- Pass tier, image URL, Discord message metadata
- Receive complete raid data including ID
- Update embed with backend raid ID

**4.4 Implement emoji registration flow**

- Update reaction handlers to call player registration endpoint
- Ensure Discord nicknames with emojis sent to backend
- Call raid participation endpoints when users react

### Phase 5: Unicode and Emoji Support

**5.1 Database configuration**

- Verify Player entity `Username` field uses `nvarchar` (SQL Server) or `text` with UTF-8 (PostgreSQL)
- Check EF Core string properties have no ASCII restrictions
- Add integration test creating player with emoji name

**5.2 JSON serialization**

- Configure `System.Text.Json` in BFF and services:
  ```csharp
  services.AddControllers().AddJsonOptions(options => {
      options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  });
  ```

- Test emoji round-trip through API

**5.3 Validation**

- Add test cases with emoji characters in player names
- Verify Discord.NET emoji handling in bot
- Test database storage and retrieval

### Phase 6: Testing and Observability

**6.1 Integration tests**

- Create Bruno collection tests for new BFF endpoints
- Test full scan flow: image → OCR → raid creation
- Test player registration with emoji names
- Test raid participation lifecycle

**6.2 Logging and metrics**

- Add structured logging to BFF orchestration endpoints
- Add Prometheus metrics: scan success rate, raid creation latency, participation errors
- Log OCR confidence scores and gym resolution results

**6.3 Error handling**

- Implement retry logic for transient failures
- Return meaningful error messages to bot
- Handle gym name ambiguity (multiple matches)
- Handle unregistered players attempting to join raids

## Detailed Changes Required

### Files to Modify

**Microservice Controllers (add v1 to route):**

- `apps/backend/microservices/Account.Service/API/Controllers/AccountController.cs`
- `apps/backend/microservices/Player.Service/API/Controllers/PlayerController.cs`
- `apps/backend/microservices/Raid.Service/API/Controllers/RaidController.cs`
- `apps/backend/microservices/Gym.Service/API/Controllers/GymController.cs`
- `apps/backend/microservices/Location.Service/API/Controllers/LocationController.cs`

**BFF Ocelot Configs:**

- `apps/backend/bffs/Bot.BFF/ocelot.json`
- `apps/backend/bffs/App.BFF/ocelot.json`

**Bot Client:**

- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`

**Bot Services:**

- `apps/frontend/bot/Application/Services/RaidService.cs`
- `apps/frontend/bot/Application/Commands/ScanCommand.cs`

**Raid Microservice:**

- `apps/backend/microservices/Raid.Service/Domain/Entities/RaidParticipant.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Domain/Entities/Raid.cs` (add Discord fields)
- `apps/backend/microservices/Raid.Service/Infrastructure/Data/RaidDbContext.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/JoinRaidCommandHandler.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Application/Commands/LeaveRaidCommandHandler.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Application/Commands/CompleteRaidCommandHandler.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Application/Commands/CancelRaidCommandHandler.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidParticipantsQueryHandler.cs` (NEW)
- `apps/backend/microservices/Raid.Service/Application/DTOs/CreateRaidDto.cs` (add Discord fields)
- `apps/backend/microservices/Raid.Service/Application/DTOs/RaidParticipationDto.cs` (NEW)

**Bot BFF New Files:**

- `apps/backend/bffs/Bot.BFF/Controllers/ScanController.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/Controllers/PlayerRegistrationController.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/Controllers/RaidParticipationController.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/Services/GymResolutionService.cs` (NEW - for OCR gym name → GymId)
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidRequest.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidResponse.cs` (NEW)

**JSON Configuration:**

- `apps/backend/bffs/Bot.BFF/Program.cs` (add JSON encoder config)
- All microservice `Program.cs` files (add JSON encoder config)

**Database Migrations:**

- Create new migration in Raid.Service for RaidParticipant table and Discord fields

## Verification Checklist

- [ ] All microservice routes include `/api/v1/` prefix
- [ ] BFF Ocelot configs route to versioned endpoints
- [ ] Bot client calls match new BFF route patterns
- [ ] Bot scan command persists raids to backend
- [ ] RaidParticipant table exists with proper relationships
- [ ] All raid command handlers implemented and tested
- [ ] Player registration preserves emoji nicknames
- [ ] Database stores and retrieves emojis correctly
- [ ] JSON serialization handles emojis without escaping
- [ ] Bruno tests cover scan → raid creation flow
- [ ] Bruno tests cover player registration with emojis
- [ ] Bruno tests cover raid participation lifecycle
- [ ] Logging captures OCR results and gym resolution
- [ ] Metrics track success rates and latencies
- [ ] Error messages returned to bot are actionable

## Open Questions to Address

1. **Gym name disambiguation**: When OCR returns "Park Gym" and multiple gyms match, how should BFF handle it?

   - Options: Return confidence scores, use location proximity, require manual selection

2. **Unregistered participants**: Should Discord users be able to join raids before registering as players?

   - Options: Auto-create player stub, require registration first, track separately

3. **Consent for emoji storage**: Do we need explicit consent before storing Discord nicknames with emojis?
4. **OCR confidence thresholds**: What confidence level should trigger manual confirmation vs auto-creation?
5. **Raid time parsing**: How to handle ambiguous times (is "2:30" AM or PM, or minutes remaining)?
6. **Migration strategy**: Should we backfill existing in-memory raids or start fresh?

### To-dos

- [ ] Update all microservice controllers to use api/v1 routes and update BFF Ocelot configs to match
- [ ] Implement Bot BFF custom controllers for scan, player registration, and raid participation orchestration
- [ ] Create RaidParticipant entity, update DbContext, and generate migration
- [ ] Implement join, leave, complete, and cancel raid command handlers
- [ ] Implement GetRaidParticipantsQueryHandler and related queries
- [ ] Add Discord message/channel/guild fields to Raid entity and DTOs
- [ ] Align bot DTOs with microservice contracts and remove generic object parameters
- [ ] Update bot RaidService and ScanCommand to persist raids via BFF orchestration endpoints
- [ ] Configure JSON encoders, verify database Unicode support, and test emoji round-trips
- [ ] Create Bruno tests for scan flow, player registration with emojis, and raid participation
- [ ] Add structured logging and Prometheus metrics to BFF orchestration endpoints