# Bot BFF Orchestration Controllers

## Overview

Implement custom orchestration controllers in Bot.BFF to handle complete bot workflows, replacing the current pure Ocelot gateway approach with intelligent endpoints that coordinate multiple microservice calls.

## Problem Statement

Currently Bot BFF is a pure Ocelot gateway with no custom controllers. The bot calls OCR, parses locally, and stores data in memory only. There are no orchestration endpoints that handle complete bot workflows end-to-end.

## Tasks

### 1. Create Scan Controller

**File**: `apps/backend/bffs/Bot.BFF/Controllers/ScanController.cs`

**Endpoint**: `POST /api/v1/scan`

**Workflow**:
1. Call OCR service to extract text from image
2. Parse OCR results to identify gym name, Pokemon, tier, time
3. Call Gym/Location services to resolve gym name → GymId
4. Call Raid service to create raid with Discord message metadata
5. Return complete raid DTO to bot including raid ID for tracking

**Supporting Files**:
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidRequest.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidResponse.cs` (NEW)
- `apps/backend/bffs/Bot.BFF/Services/GymResolutionService.cs` (NEW)

**Error Handling**:
- Handle gym name ambiguity (multiple matches)
- Handle OCR confidence thresholds
- Graceful fallback for manual confirmation paths

### 2. Create Player Registration Controller

**File**: `apps/backend/bffs/Bot.BFF/Controllers/PlayerRegistrationController.cs`

**Endpoint**: `POST /api/v1/player-registration`

**Workflow**:
1. Accept Discord user data (ID, username, emoji nickname)
2. Validate and transform to Player service DTOs
3. Call Player service to create/update player
4. Return player ID for future raid participation

**Requirements**:
- Ensure emoji-rich nicknames are preserved
- Handle duplicate registrations gracefully
- Validate Discord user data

### 3. Create Raid Participation Controller

**File**: `apps/backend/bffs/Bot.BFF/Controllers/RaidParticipationController.cs`

**Endpoints**:
- `POST /api/v1/raids/{raidId}/join` - Join raid
- `POST /api/v1/raids/{raidId}/leave` - Leave raid
- `POST /api/v1/raids/{raidId}/complete` - Mark raid as complete
- `POST /api/v1/raids/{raidId}/cancel` - Cancel raid

**Workflow** (for each endpoint):
- Validate raid exists and is active
- Handle player lookups
- Call appropriate Raid service command
- Return updated raid status

**Error Handling**:
- Handle unregistered players
- Handle full raids (max capacity)
- Handle invalid raid states (already completed/cancelled)

## Verification

- [ ] ScanController successfully orchestrates full scan → raid creation flow
- [ ] PlayerRegistrationController preserves emoji nicknames
- [ ] RaidParticipationController handles all lifecycle operations
- [ ] Error responses are meaningful and actionable
- [ ] All endpoints return proper HTTP status codes
- [ ] Logging captures orchestration steps and errors

## Dependencies

- Requires: Todo #1 (API versioning standardization)
- Required by: Todo #8 (Bot integration updates)

## Files to Create

- `apps/backend/bffs/Bot.BFF/Controllers/ScanController.cs`
- `apps/backend/bffs/Bot.BFF/Controllers/PlayerRegistrationController.cs`
- `apps/backend/bffs/Bot.BFF/Controllers/RaidParticipationController.cs`
- `apps/backend/bffs/Bot.BFF/Services/GymResolutionService.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidRequest.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/ScanRaidResponse.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/PlayerRegistrationRequest.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/PlayerRegistrationResponse.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/RaidParticipationRequest.cs`
- `apps/backend/bffs/Bot.BFF/DTOs/RaidParticipationResponse.cs`

