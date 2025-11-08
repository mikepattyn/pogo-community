# Discord Metadata for Raid Entity

## Overview

Add Discord-specific metadata fields to the Raid entity to track which Discord message, channel, and guild a raid is associated with, enabling proper synchronization between Discord bot state and backend persistence.

## Problem Statement

Currently the Raid entity has no connection to Discord. There's no way to map a Discord message ID to a backend raid ID, making it impossible to update raid embeds when participation changes or to recover raid state after bot restarts.

## Tasks

### 1. Update Raid Entity

**File**: `apps/backend/microservices/Raid.Service/Domain/Entities/Raid.cs`

**New Properties**:
- `DiscordMessageId` (string, nullable) - The Discord message ID for the raid embed
- `DiscordChannelId` (string, nullable) - The Discord channel where raid was posted
- `DiscordGuildId` (string, nullable) - The Discord server/guild ID
- `DiscordUserId` (string, nullable) - The Discord user who created/scanned the raid

**Considerations**:
- Use string type for Discord IDs (they're ulong/snowflakes)
- Make nullable since raids might be created outside Discord context
- Add indexes on DiscordMessageId for quick lookups

### 2. Update CreateRaidDto

**File**: `apps/backend/microservices/Raid.Service/Application/DTOs/CreateRaidDto.cs`

**New Properties**:
- `DiscordMessageId` (string, optional)
- `DiscordChannelId` (string, optional)
- `DiscordGuildId` (string, optional)
- `DiscordUserId` (string, optional)

**Validation**:
- If any Discord field is provided, consider requiring DiscordGuildId
- Validate Discord ID format if needed (numeric string)

### 3. Update CreateRaidCommand

**File**: `apps/backend/microservices/Raid.Service/Application/Commands/CreateRaidCommand.cs`

**Changes**:
- Add Discord metadata properties from CreateRaidDto
- Update command handler to map these fields to entity

### 4. Update RaidDto (Response)

**File**: `apps/backend/microservices/Raid.Service/Application/DTOs/RaidDto.cs`

**New Properties**:
- Include Discord metadata in response DTO
- Allows bot to verify/sync message IDs

### 5. Create EF Core Migration

**Commands**:
```bash
cd apps/backend/microservices/Raid.Service
dotnet ef migrations add AddDiscordMetadataToRaid
```

**Migration should include**:
- Add DiscordMessageId column (nvarchar, nullable)
- Add DiscordChannelId column (nvarchar, nullable)
- Add DiscordGuildId column (nvarchar, nullable)
- Add DiscordUserId column (nvarchar, nullable)
- Add index on DiscordMessageId for lookups

### 6. Update RaidDbContext Configuration

**File**: `apps/backend/microservices/Raid.Service/Infrastructure/Data/RaidDbContext.cs`

**Changes**:
- Configure Discord field lengths (Discord IDs are ~18-20 chars)
- Add index on DiscordMessageId:
  ```csharp
  builder.HasIndex(r => r.DiscordMessageId);
  ```

### 7. Create GetRaidByDiscordMessageIdQuery (Optional but Recommended)

**File**: `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidByDiscordMessageIdQuery.cs` (NEW)

**Query**: `GetRaidByDiscordMessageIdQuery`
- `DiscordMessageId` (string)

**Logic**:
- Query Raids by DiscordMessageId
- Return raid details if found
- Return null if not found

**Use case**: Bot can look up raid by message ID to sync state

## Verification

- [ ] Raid entity has all Discord metadata fields
- [ ] CreateRaidDto accepts Discord metadata
- [ ] CreateRaidCommand maps Discord fields correctly
- [ ] Migration adds columns and index successfully
- [ ] RaidDto includes Discord metadata in responses
- [ ] GetRaidByDiscordMessageIdQuery works correctly
- [ ] Index improves query performance for Discord message lookups
- [ ] Existing raids without Discord metadata still work (nullable fields)

## Dependencies

- Requires: Todo #3 (RaidParticipant entity) - can be done in parallel
- Required by: Todo #2 (Bot BFF orchestration), Todo #8 (Bot integration)

## Files to Modify/Create

**Modified Files**:
- `apps/backend/microservices/Raid.Service/Domain/Entities/Raid.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/CreateRaidDto.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/RaidDto.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/CreateRaidCommand.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/CreateRaidCommandHandler.cs`
- `apps/backend/microservices/Raid.Service/Infrastructure/Data/RaidDbContext.cs`

**New Files**:
- Migration file (auto-generated)
- `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidByDiscordMessageIdQuery.cs` (optional)
- `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidByDiscordMessageIdQueryHandler.cs` (optional)

## Design Decisions

1. **Field types**: Use string for Discord IDs (not ulong) for database compatibility
2. **Nullability**: Make all Discord fields nullable to support non-Discord raid creation
3. **Uniqueness**: Should DiscordMessageId be unique?
   - Recommendation: Yes, add unique constraint in migration
4. **Cascade behavior**: What happens if Discord message is deleted?
   - Recommendation: Keep raid data, it's still valuable for history

