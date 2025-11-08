# Bot Integration and Persistence

## Overview

Update bot services and commands to persist raids via BFF orchestration endpoints, replacing in-memory-only storage with proper backend persistence while maintaining quick lookups through caching.

## Problem Statement

Currently:

- `RaidService.CreateRaidAsync` has commented line: `// await _botBffClient.CreateRaidAsync(raid);`
- Raids only stored in-memory dictionary `_raids[messageId]`
- No backend persistence means data lost on bot restart
- ScanCommand calls OCR directly instead of using BFF orchestration

## Tasks

### 1. Enable Raid Persistence in RaidService

**File**: `apps/frontend/bot/Application/Services/RaidService.cs`

**Changes**:

- Uncomment and fix `await _botBffClient.CreateRaidAsync(raid)`
- Update to call new BFF scan endpoint instead of just create raid
- Store returned raid ID from backend
- Keep in-memory cache for quick lookups but sync with backend
- Add cache invalidation logic

**Implementation**:

```csharp
public async Task<Raid> CreateRaidAsync(/* params */)
{
    // Build proper CreateRaidDto
    var createRaidDto = new CreateRaidDto { /* ... */ };
    
    // Call BFF to persist
    var raidDto = await _botBffClient.CreateRaidAsync(createRaidDto);
    
    // Cache locally for quick access
    var raid = MapToRaid(raidDto);
    _raids[messageId] = raid;
    
    return raid;
}
```

### 2. Update ScanCommand to Use BFF Orchestration

**File**: `apps/frontend/bot/Application/Commands/ScanCommand.cs`

**Current flow**:

1. Upload image to OCR service
2. Parse OCR results locally
3. Store in memory only

**New flow**:

1. Call BFF `/api/v1/scan` endpoint with:

   - Image URL or base64 data
   - Tier
   - Discord message metadata (messageId, channelId, guildId, userId)

2. BFF orchestrates: OCR → gym resolution → raid creation
3. Receive complete raid data including backend raid ID
4. Update Discord embed with raid ID and details
5. Cache raid locally

**Changes**:

- Replace direct OCR calls with BFF scan endpoint call
- Pass tier and Discord metadata to BFF
- Handle BFF response with complete raid data
- Update embed creation to use backend raid ID
- Add error handling for BFF failures

### 3. Implement Emoji Registration Flow

**File**: `apps/frontend/bot/Application/Services/PlayerService.cs` (or create if not exists)

**Changes**:

- Create method to register players via BFF player registration endpoint
- Call when users react to raid embeds with emoji
- Ensure Discord nicknames with emojis are sent to backend
- Handle duplicate registration gracefully

**Implementation**:

```csharp
public async Task<Player> RegisterPlayerAsync(IUser discordUser)
{
    var createPlayerDto = new CreatePlayerDto
    {
        DiscordUserId = discordUser.Id.ToString(),
        Username = discordUser.Username,
        Nickname = discordUser.GlobalName, // May contain emojis
        // ... other fields
    };
    
    var playerDto = await _botBffClient.RegisterPlayerAsync(createPlayerDto);
    return MapToPlayer(playerDto);
}
```

### 4. Implement Raid Participation via BFF

**File**: `apps/frontend/bot/Application/Services/RaidService.cs`

**New methods**:

- `JoinRaidAsync(raidId, playerId, extraPlayers)`
- `LeaveRaidAsync(raidId, playerId)`
- `CompleteRaidAsync(raidId)`
- `CancelRaidAsync(raidId)`

**Changes**:

- Call BFF raid participation endpoints
- Update local cache with backend response
- Update Discord embeds to reflect changes
- Handle errors (raid full, player not registered, etc.)

**Implementation**:

```csharp
public async Task JoinRaidAsync(Guid raidId, Guid playerId, int extraPlayers = 0)
{
    var request = new JoinRaidRequest
    {
        PlayerId = playerId,
        ExtraPlayers = extraPlayers
    };
    
    var updatedRaid = await _botBffClient.JoinRaidAsync(raidId, request);
    
    // Update cache
    if (_raids.TryGetValue(messageId, out var raid))
    {
        raid.Participants = updatedRaid.Participants;
        raid.CurrentParticipants = updatedRaid.CurrentParticipants;
    }
    
    return updatedRaid;
}
```

### 5. Update Reaction Handlers

**Files**: Various command/event handlers for Discord reactions

**Changes**:

- When user reacts to raid embed:

  1. Check if player is registered (call BFF)
  2. If not registered, register player first
  3. Call JoinRaidAsync with backend raid ID
  4. Update Discord embed with new participant count

- When user removes reaction:

  1. Call LeaveRaidAsync
  2. Update Discord embed

### 6. Add Cache Synchronization

**File**: `apps/frontend/bot/Application/Services/RaidService.cs`

**Features**:

- On bot startup, load active raids from backend
- Periodically sync cache with backend (every 5-10 minutes)
- Handle cache misses by querying backend
- Implement cache eviction for old/completed raids

**Implementation**:

```csharp
public async Task SyncRaidsAsync()
{
    var activeRaids = await _botBffClient.GetActiveRaidsAsync();
    
    foreach (var raidDto in activeRaids)
    {
        var messageId = ulong.Parse(raidDto.DiscordMessageId);
        _raids[messageId] = MapToRaid(raidDto);
    }
}
```

## Verification

- [ ] ScanCommand persists raids to backend via BFF
- [ ] Raids are cached locally for quick access
- [ ] Bot can recover raid state after restart
- [ ] Player registration preserves emoji nicknames
- [ ] Raid participation calls backend endpoints
- [ ] Discord embeds update when participation changes
- [ ] Cache syncs with backend periodically
- [ ] Error handling provides meaningful feedback to users
- [ ] No data loss on bot restart

## Dependencies

- Requires: Todo #1 (API versioning)
- Requires: Todo #2 (Bot BFF orchestration controllers)
- Requires: Todo #4 (Raid command handlers)
- Requires: Todo #7 (DTO contract alignment)

## Files to Modify/Create

**Modified Files**:

- `apps/frontend/bot/Application/Services/RaidService.cs`
- `apps/frontend/bot/Application/Commands/ScanCommand.cs`
- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`
- Various reaction handler files

**New Files**:

- `apps/frontend/bot/Application/Services/PlayerService.cs` (if not exists)
- `apps/frontend/bot/Application/Services/CacheSyncService.cs` (optional)

## Design Decisions

1. **Cache strategy**: How long to keep raids in cache?

   - Recommendation: Keep until EndTime + 1 hour, then evict

2. **Startup behavior**: Should bot load all historical raids or just active ones?

   - Recommendation: Only active raids (IsActive = true, EndTime > now)

3. **Failure handling**: What if BFF is down when creating raid?

   - Recommendation: Fall back to memory-only mode, retry persistence later

4. **Sync frequency**: How often to sync cache with backend?

   - Recommendation: Every 10 minutes, or on-demand when cache miss occurs
