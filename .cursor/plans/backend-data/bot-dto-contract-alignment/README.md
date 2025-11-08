# Bot DTO Contract Alignment

## Overview

Align bot DTOs with microservice contracts, fixing field name mismatches and removing generic object parameters to ensure proper data serialization and API communication.

## Problem Statement

Current contract mismatches:
- **Bot client** sends: `CreatePlayerDto` with `DiscordId` field
- **Player service** expects: `CreatePlayerDto` with `DiscordUserId` field
- **Bot client** sends: Generic `object raidData`
- **Raid service** expects: `CreateRaidDto` with specific fields like `GymId`, `PokemonSpecies`, `StartTime`, `EndTime`

These mismatches cause serialization errors and data not being persisted correctly.

## Tasks

### 1. Update Bot Player DTOs

**File**: `apps/frontend/bot/Application/DTOs/CreatePlayerDto.cs` (or wherever defined)

**Changes**:
- Rename `DiscordId` to `DiscordUserId` to match Player.Service contract
- Verify all other fields match Player.Service expectations:
  - `Username` (string)
  - `TeamColor` (enum or string)
  - `Level` (int, optional)
  - Any emoji-related fields

**Alternative approach**:
- Keep bot's internal DTO as-is
- Create mapping in BotBffClient or BFF controller to transform field names

### 2. Create Proper Bot Raid DTOs

**File**: `apps/frontend/bot/Application/DTOs/CreateRaidDto.cs` (NEW or update existing)

**Required Fields** (matching Raid.Service):
- `GymId` (Guid)
- `PokemonSpecies` (string)
- `Tier` (int)
- `StartTime` (DateTime)
- `EndTime` (DateTime)
- `MaxParticipants` (int, optional)
- `DiscordMessageId` (string, optional)
- `DiscordChannelId` (string, optional)
- `DiscordGuildId` (string, optional)
- `DiscordUserId` (string, optional)

**Remove**:
- Generic `object raidData` parameters

### 3. Update BotBffClient

**File**: `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`

**Changes**:
- Replace `object` parameters with strongly-typed DTOs
- Update method signatures:
  ```csharp
  // Before
  Task<RaidDto> CreateRaidAsync(object raidData);
  
  // After
  Task<RaidDto> CreateRaidAsync(CreateRaidDto createRaidDto);
  ```

- Update player creation:
  ```csharp
  // Before
  Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto); // with DiscordId
  
  // After
  Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto); // with DiscordUserId
  ```

### 4. Update RaidService

**File**: `apps/frontend/bot/Application/Services/RaidService.cs`

**Changes**:
- Build proper `CreateRaidDto` objects instead of anonymous objects
- Map parsed OCR data to DTO fields:
  ```csharp
  var createRaidDto = new CreateRaidDto
  {
      GymId = resolvedGymId,
      PokemonSpecies = parsedPokemon,
      Tier = parsedTier,
      StartTime = parsedStartTime,
      EndTime = parsedEndTime,
      MaxParticipants = 20, // or from config
      DiscordMessageId = messageId.ToString(),
      DiscordChannelId = channelId.ToString(),
      DiscordGuildId = guildId.ToString(),
      DiscordUserId = userId.ToString()
  };
  ```

### 5. Verify Player Service Contract

**Files to check**:
- `apps/backend/microservices/Player.Service/Application/DTOs/CreatePlayerDto.cs`
- `apps/backend/microservices/Player.Service/Application/Commands/CreatePlayerCommand.cs`

**Verify fields**:
- Confirm field names and types
- Document any optional fields
- Check validation requirements

### 6. Verify Raid Service Contract

**Files to check**:
- `apps/backend/microservices/Raid.Service/Application/DTOs/CreateRaidDto.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/CreateRaidCommand.cs`

**Verify fields**:
- Confirm all required fields
- Document optional fields
- Check validation rules (e.g., EndTime > StartTime)

## Verification

- [ ] Bot CreatePlayerDto matches Player.Service contract exactly
- [ ] Bot CreateRaidDto matches Raid.Service contract exactly
- [ ] No generic `object` parameters remain in BotBffClient
- [ ] RaidService builds proper DTOs with all required fields
- [ ] Field name mismatches resolved (DiscordId → DiscordUserId)
- [ ] Serialization/deserialization works without errors
- [ ] Integration tests pass with real API calls

## Dependencies

- Requires: Todo #6 (Discord metadata in Raid entity) - for Discord fields in CreateRaidDto
- Required by: Todo #8 (Bot integration updates)

## Files to Modify/Create

**Bot Files**:
- `apps/frontend/bot/Application/DTOs/CreatePlayerDto.cs`
- `apps/frontend/bot/Application/DTOs/CreateRaidDto.cs` (NEW or update)
- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`
- `apps/frontend/bot/Application/Services/RaidService.cs`

**Verification Files** (read-only):
- `apps/backend/microservices/Player.Service/Application/DTOs/CreatePlayerDto.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/CreateRaidDto.cs`

## Design Decisions

1. **Mapping location**: Where should DTO transformation happen?
   - Option A: In bot code (bot DTOs match service DTOs exactly)
   - Option B: In BFF controllers (bot has its own DTOs, BFF maps)
   - Recommendation: Option A for simplicity, Option B for better separation

2. **Validation**: Should bot validate DTO fields before sending?
   - Recommendation: Yes, validate required fields and formats before API calls

3. **Default values**: Should bot provide defaults for optional fields?
   - Recommendation: Yes, use sensible defaults (e.g., MaxParticipants = 20)

