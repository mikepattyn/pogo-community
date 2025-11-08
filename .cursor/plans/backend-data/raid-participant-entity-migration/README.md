# Raid Participant Entity and Migration

## Overview

Create the RaidParticipant entity to track which players have joined which raids, update the database context, and generate the necessary EF Core migration.

## Problem Statement

Currently `RaidDbContext` has no `RaidParticipant` entity or join table. There's no way to persist which players joined which raids. The participation tracking is missing from the database schema entirely.

## Tasks

### 1. Create RaidParticipant Entity

**File**: `apps/backend/microservices/Raid.Service/Domain/Entities/RaidParticipant.cs` (NEW)

**Properties**:
- `Id` (Guid) - Primary key
- `RaidId` (Guid) - Foreign key to Raid
- `PlayerId` (Guid) - Foreign key to Player (external service)
- `JoinedAt` (DateTime) - When player joined
- `ExtraPlayers` (int) - Number of additional players brought (default 0)
- `Status` (enum) - Joined, Left, Completed, NoShow
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)

**Relationships**:
- Many-to-one with Raid entity
- Store PlayerId as Guid (Player entity lives in Player.Service)

### 2. Update RaidDbContext

**File**: `apps/backend/microservices/Raid.Service/Infrastructure/Data/RaidDbContext.cs`

**Changes**:
- Add `DbSet<RaidParticipant> RaidParticipants { get; set; }`
- Configure entity relationships in `OnModelCreating`:
  - RaidParticipant → Raid (many-to-one)
  - Index on `RaidId` for query performance
  - Index on `PlayerId` for player history queries
  - Unique constraint on `(RaidId, PlayerId)` to prevent duplicate joins
- Remove any `Database.EnsureCreated()` calls if present

### 3. Create EF Core Migration

**Commands**:
```bash
cd apps/backend/microservices/Raid.Service
dotnet ef migrations add AddRaidParticipantEntity
```

**Migration should include**:
- Create `RaidParticipants` table
- Add foreign key constraint to `Raids` table
- Add indexes for performance
- Add unique constraint

### 4. Update Raid Entity (if needed)

**File**: `apps/backend/microservices/Raid.Service/Domain/Entities/Raid.cs`

**Potential additions**:
- Navigation property: `ICollection<RaidParticipant> Participants { get; set; }`
- Consider adding `CurrentParticipants` calculated property

## Verification

- [ ] RaidParticipant entity created with all required properties
- [ ] RaidDbContext includes RaidParticipants DbSet
- [ ] Entity relationships configured correctly
- [ ] Migration generated successfully
- [ ] Migration creates table with proper schema
- [ ] Indexes and constraints are in place
- [ ] No `Database.EnsureCreated()` calls remain
- [ ] Migration can be applied and rolled back successfully

## Dependencies

- Requires: Todo #1 (API versioning) - not strictly required but good to have
- Required by: Todo #4 (Command handlers), Todo #5 (Query handlers)

## Files to Create/Modify

**New Files**:
- `apps/backend/microservices/Raid.Service/Domain/Entities/RaidParticipant.cs`
- Migration file (auto-generated)

**Modified Files**:
- `apps/backend/microservices/Raid.Service/Infrastructure/Data/RaidDbContext.cs`
- `apps/backend/microservices/Raid.Service/Domain/Entities/Raid.cs` (optional navigation property)

## Notes

- PlayerId is stored as Guid since Player entity lives in a different microservice
- Status enum allows tracking participation lifecycle
- ExtraPlayers field supports "+1" or "+2" scenarios common in raid coordination
- Unique constraint prevents accidental duplicate joins

