# Raid Query Handlers Implementation

## Overview

Implement query handlers to retrieve raid participation data, including getting all participants for a raid and potentially getting all raids for a player.

## Problem Statement

Query handler `GetRaidParticipantsQuery` is declared but has no implementation. There's no way to retrieve the list of players who have joined a raid, which is essential for displaying raid rosters in the bot.

## Tasks

### 1. Implement GetRaidParticipantsQueryHandler

**File**: `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidParticipantsQueryHandler.cs` (NEW)

**Query**: `GetRaidParticipantsQuery`
- `RaidId` (Guid)

**Logic**:
1. Verify raid exists
2. Query RaidParticipants table filtered by RaidId
3. Include only active participants (Status = Joined)
4. Order by JoinedAt ascending
5. Map to DTOs including:
   - PlayerId
   - JoinedAt
   - ExtraPlayers
   - Status
6. Return list of participant DTOs

**Response DTO**: `RaidParticipantDto`
- `Id` (Guid)
- `PlayerId` (Guid)
- `JoinedAt` (DateTime)
- `ExtraPlayers` (int)
- `Status` (string)

**Note**: Player details (username, nickname) should be fetched by the BFF from Player.Service using the PlayerId

### 2. Create GetPlayerRaidsQuery (Optional but Recommended)

**File**: `apps/backend/microservices/Raid.Service/Application/Queries/GetPlayerRaidsQueryHandler.cs` (NEW)

**Query**: `GetPlayerRaidsQuery`
- `PlayerId` (Guid)
- `IncludeCompleted` (bool, default false)
- `IncludeCancelled` (bool, default false)

**Logic**:
1. Query RaidParticipants filtered by PlayerId
2. Join with Raids table
3. Filter by IsActive, IsCompleted, IsCancelled based on parameters
4. Order by Raid.StartTime descending
5. Map to DTOs with raid details
6. Return list of raid DTOs

**Response DTO**: `PlayerRaidDto`
- `RaidId` (Guid)
- `GymId` (Guid)
- `PokemonSpecies` (string)
- `Tier` (int)
- `StartTime` (DateTime)
- `EndTime` (DateTime)
- `JoinedAt` (DateTime)
- `Status` (string)
- `IsCompleted` (bool)
- `IsCancelled` (bool)

**Use case**: Player history, statistics, and "my raids" view

### 3. Create GetActiveRaidsQuery (Optional but Recommended)

**File**: `apps/backend/microservices/Raid.Service/Application/Queries/GetActiveRaidsQueryHandler.cs` (NEW)

**Query**: `GetActiveRaidsQuery`
- `GymId` (Guid, optional)
- `MinTier` (int, optional)
- `MaxTier` (int, optional)

**Logic**:
1. Query Raids table filtered by IsActive = true
2. Apply optional filters (GymId, tier range)
3. Filter out raids where EndTime < DateTime.UtcNow
4. Include participant count
5. Order by StartTime ascending
6. Map to DTOs
7. Return list of active raid DTOs

**Response DTO**: `ActiveRaidDto`
- `RaidId` (Guid)
- `GymId` (Guid)
- `PokemonSpecies` (string)
- `Tier` (int)
- `StartTime` (DateTime)
- `EndTime` (DateTime)
- `CurrentParticipants` (int)
- `MaxParticipants` (int)
- `DiscordMessageId` (string, optional)

**Use case**: Listing available raids for players to join

## Verification

- [ ] GetRaidParticipantsQueryHandler returns all active participants
- [ ] GetRaidParticipantsQueryHandler handles non-existent raids gracefully
- [ ] Participant list is ordered by join time
- [ ] GetPlayerRaidsQuery returns player's raid history
- [ ] GetActiveRaidsQuery returns only active, non-expired raids
- [ ] All queries have proper error handling
- [ ] Performance is acceptable with indexes on foreign keys
- [ ] Unit tests cover various scenarios

## Dependencies

- Requires: Todo #3 (RaidParticipant entity and migration)
- Required by: Todo #2 (Bot BFF orchestration), Todo #8 (Bot integration)

## Files to Create

**Required**:
- `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidParticipantsQuery.cs` (if not exists)
- `apps/backend/microservices/Raid.Service/Application/Queries/GetRaidParticipantsQueryHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/RaidParticipantDto.cs`

**Optional but Recommended**:
- `apps/backend/microservices/Raid.Service/Application/Queries/GetPlayerRaidsQuery.cs`
- `apps/backend/microservices/Raid.Service/Application/Queries/GetPlayerRaidsQueryHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/PlayerRaidDto.cs`
- `apps/backend/microservices/Raid.Service/Application/Queries/GetActiveRaidsQuery.cs`
- `apps/backend/microservices/Raid.Service/Application/Queries/GetActiveRaidsQueryHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/DTOs/ActiveRaidDto.cs`

## Design Decisions

1. **Player details**: Should query handler fetch player details from Player.Service?
   - Recommendation: No, return only PlayerId. Let BFF orchestrate player detail fetching

2. **Filtering left participants**: Should GetRaidParticipantsQuery include players who left?
   - Recommendation: No, only return Status = Joined by default. Add optional parameter if needed

3. **Pagination**: Should queries support pagination?
   - Recommendation: Not initially, but consider for GetPlayerRaidsQuery if history grows large

