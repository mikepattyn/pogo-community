# Raid Command Handlers Implementation

## Overview

Implement the command handlers for raid participation lifecycle: joining, leaving, completing, and cancelling raids. These handlers will use the RaidParticipant entity to track participation.

## Problem Statement

Command handlers exist but are unimplemented: `JoinRaidCommand`, `LeaveRaidCommand`, `CompleteRaidCommand`, `CancelRaidCommand`. Without these implementations, there's no way to manage raid participation through the backend.

## Tasks

### 1. Implement JoinRaidCommandHandler

**File**: `apps/backend/microservices/Raid.Service/Application/Commands/JoinRaidCommandHandler.cs` (NEW)

**Command**: `JoinRaidCommand`
- `RaidId` (Guid)
- `PlayerId` (Guid)
- `ExtraPlayers` (int, default 0)

**Logic**:
1. Verify raid exists and is active
2. Check if raid is not already completed or cancelled
3. Check if player hasn't already joined
4. Check if raid has capacity (CurrentParticipants + ExtraPlayers + 1 <= MaxParticipants)
5. Create RaidParticipant record with Status = Joined
6. Increment Raid.CurrentParticipants
7. Save changes
8. Return success with updated raid data

**Validations**:
- Raid must exist
- Raid must be active (not completed/cancelled)
- Player cannot join twice
- Raid must have capacity

### 2. Implement LeaveRaidCommandHandler

**File**: `apps/backend/microservices/Raid.Service/Application/Commands/LeaveRaidCommandHandler.cs` (NEW)

**Command**: `LeaveRaidCommand`
- `RaidId` (Guid)
- `PlayerId` (Guid)

**Logic**:
1. Verify raid exists
2. Find RaidParticipant record
3. Update Status to Left or remove record (design decision)
4. Decrement Raid.CurrentParticipants (including ExtraPlayers)
5. Save changes
6. Return success with updated raid data

**Validations**:
- Raid must exist
- Player must be currently joined
- Cannot leave after raid is completed

### 3. Implement CompleteRaidCommandHandler

**File**: `apps/backend/microservices/Raid.Service/Application/Commands/CompleteRaidCommandHandler.cs` (NEW)

**Command**: `CompleteRaidCommand`
- `RaidId` (Guid)
- `CompletedBy` (Guid) - Discord user or player ID

**Logic**:
1. Verify raid exists and is active
2. Set Raid.IsActive = false
3. Set Raid.IsCompleted = true
4. Set Raid.CompletedAt = DateTime.UtcNow
5. Update all RaidParticipant records with Status = Joined to Status = Completed
6. Save changes
7. Return success

**Validations**:
- Raid must exist
- Raid must be active
- Raid cannot already be completed or cancelled

### 4. Implement CancelRaidCommandHandler

**File**: `apps/backend/microservices/Raid.Service/Application/Commands/CancelRaidCommandHandler.cs` (NEW)

**Command**: `CancelRaidCommand`
- `RaidId` (Guid)
- `CancelledBy` (Guid) - Discord user or player ID
- `Reason` (string, optional)

**Logic**:
1. Verify raid exists and is active
2. Set Raid.IsActive = false
3. Set Raid.IsCancelled = true
4. Set Raid.CancelledAt = DateTime.UtcNow
5. Optionally store cancellation reason
6. Save changes
7. Return success

**Validations**:
- Raid must exist
- Raid must be active
- Raid cannot already be completed or cancelled

## Verification

- [ ] JoinRaidCommandHandler prevents duplicate joins
- [ ] JoinRaidCommandHandler respects raid capacity
- [ ] LeaveRaidCommandHandler correctly updates participant count
- [ ] CompleteRaidCommandHandler marks raid and participants as completed
- [ ] CancelRaidCommandHandler prevents further participation
- [ ] All handlers have proper error handling
- [ ] All handlers return appropriate error messages
- [ ] Unit tests cover happy path and edge cases

## Dependencies

- Requires: Todo #3 (RaidParticipant entity and migration)
- Required by: Todo #2 (Bot BFF orchestration), Todo #8 (Bot integration)

## Files to Create

- `apps/backend/microservices/Raid.Service/Application/Commands/JoinRaidCommand.cs` (if not exists)
- `apps/backend/microservices/Raid.Service/Application/Commands/JoinRaidCommandHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/LeaveRaidCommand.cs` (if not exists)
- `apps/backend/microservices/Raid.Service/Application/Commands/LeaveRaidCommandHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/CompleteRaidCommand.cs` (if not exists)
- `apps/backend/microservices/Raid.Service/Application/Commands/CompleteRaidCommandHandler.cs`
- `apps/backend/microservices/Raid.Service/Application/Commands/CancelRaidCommand.cs` (if not exists)
- `apps/backend/microservices/Raid.Service/Application/Commands/CancelRaidCommandHandler.cs`

## Design Decisions

1. **Leave behavior**: Should LeaveRaidCommandHandler soft-delete (Status = Left) or hard-delete the RaidParticipant record?
   - Recommendation: Soft-delete for audit trail

2. **Capacity checking**: Should capacity include ExtraPlayers in the calculation?
   - Recommendation: Yes, total = participants + sum(ExtraPlayers)

3. **Completion status**: Should completing a raid update participant status?
   - Recommendation: Yes, update to Status = Completed for analytics

