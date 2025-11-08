# Unicode and Emoji Support

## Overview

Ensure complete Unicode and emoji support across the entire stack: database storage, JSON serialization, and API transport, enabling players to use emoji-rich nicknames without data corruption.

## Problem Statement

Current concerns:
- No confirmation that database columns support emoji characters
- No JSON encoder configuration for emoji serialization
- Player registration with emoji nicknames untested
- Potential for data corruption or escaping issues

## Tasks

### 1. Database Configuration

**SQL Server** (if using):

**File**: Check migration files in `apps/backend/microservices/Player.Service/Migrations/`

**Requirements**:
- Verify Player entity `Username` and `Nickname` fields use `nvarchar` type
- Check column definitions in migrations:
  ```csharp
  Username = table.Column<string>(type: "nvarchar(450)", nullable: false)
  ```
- If using `varchar`, update to `nvarchar` to support Unicode

**PostgreSQL** (if using):

**Requirements**:
- Verify database encoding is UTF-8
- Check connection string includes: `Encoding=utf8;`
- Text columns automatically support Unicode

**CockroachDB** (if using):

**Requirements**:
- CockroachDB uses UTF-8 by default
- Verify string columns are `STRING` or `TEXT` type
- No special configuration needed

### 2. Entity Framework Configuration

**File**: `apps/backend/microservices/Player.Service/Infrastructure/Data/PlayerDbContext.cs`

**Changes**:
- Verify string properties have no ASCII restrictions
- Check for any `.IsUnicode(false)` configurations and remove them
- Ensure proper column types:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Player>(builder =>
    {
        builder.Property(p => p.Username)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(true); // Explicitly enable Unicode
            
        builder.Property(p => p.Nickname)
            .HasMaxLength(100)
            .IsUnicode(true); // Explicitly enable Unicode
    });
}
```

### 3. JSON Serialization Configuration

**Files**: All `Program.cs` files in services and BFFs

**Bot BFF**:
`apps/backend/bffs/Bot.BFF/Program.cs`

**All Microservices**:
- `apps/backend/microservices/Player.Service/Program.cs`
- `apps/backend/microservices/Raid.Service/Program.cs`
- `apps/backend/microservices/Gym.Service/Program.cs`
- `apps/backend/microservices/Location.Service/Program.cs`
- `apps/backend/microservices/Account.Service/Program.cs`
- `apps/backend/microservices/OCR.Service/Program.cs`

**Changes**:
Add JSON encoder configuration to prevent emoji escaping:

```csharp
using System.Text.Encodings.Web;
using System.Text.Json;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allow emojis and Unicode characters without escaping
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        
        // Optional: other useful settings
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });
```

### 4. Bot Discord.NET Configuration

**File**: `apps/frontend/bot/Program.cs`

**Verify**:
- Discord.NET handles emojis correctly by default
- No special configuration needed
- Test extracting user nicknames with emojis

### 5. HTTP Client Configuration

**File**: `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`

**Verify**:
- HttpClient uses UTF-8 encoding for requests
- JSON serialization options match backend:

```csharp
private readonly JsonSerializerOptions _jsonOptions = new()
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
```

### 6. Create Integration Test

**File**: `apps/backend/microservices/Player.Service/Tests/Integration/PlayerEmojiTests.cs` (NEW)

**Test cases**:

```csharp
[Fact]
public async Task CreatePlayer_WithEmojiNickname_StoresAndRetrievesCorrectly()
{
    // Arrange
    var createDto = new CreatePlayerDto
    {
        DiscordUserId = "123456789",
        Username = "TestUser",
        Nickname = "🔥FireMaster🔥"
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/player", createDto);
    var player = await response.Content.ReadFromJsonAsync<PlayerDto>();
    
    var getResponse = await _client.GetAsync($"/api/v1/player/{player.Id}");
    var retrievedPlayer = await getResponse.Content.ReadFromJsonAsync<PlayerDto>();
    
    // Assert
    Assert.Equal("🔥FireMaster🔥", retrievedPlayer.Nickname);
}

[Theory]
[InlineData("🎮Gamer123")]
[InlineData("⚡️Thunder⚡️")]
[InlineData("🌟Star🌟Player")]
[InlineData("日本語ユーザー")] // Japanese characters
[InlineData("用户名")] // Chinese characters
public async Task CreatePlayer_WithVariousUnicodeCharacters_StoresCorrectly(string nickname)
{
    // Test various Unicode scenarios
}
```

### 7. Create End-to-End Test

**File**: Bruno collection test

**Test**: `bruno/microservices/player-service/Create Player with Emoji.bru`

```
meta {
  name: Create Player with Emoji
  type: http
  seq: 5
}

post {
  url: {{baseUrl}}/api/v1/player
  body: json
  auth: none
}

body:json {
  {
    "discordUserId": "987654321",
    "username": "EmojiTester",
    "nickname": "🎯Raid🎯Master",
    "teamColor": "Mystic",
    "level": 40
  }
}

assert {
  res.status: eq 201
  res.body.nickname: eq 🎯Raid🎯Master
}
```

## Verification

- [ ] Database columns use Unicode-compatible types (nvarchar/text)
- [ ] EF Core entities explicitly enable Unicode
- [ ] JSON encoder configured in all services
- [ ] Integration test with emoji nickname passes
- [ ] Bruno test with emoji nickname passes
- [ ] Emoji round-trip through API works correctly
- [ ] No character escaping (e.g., `\uD83D\uDD25` instead of 🔥)
- [ ] Database stores emojis correctly (verify with SQL query)
- [ ] Discord bot can send and receive emoji nicknames

## Dependencies

- Requires: Todo #7 (DTO contract alignment) - for proper field mapping
- Required by: Todo #8 (Bot integration) - for emoji registration flow

## Files to Modify/Create

**Modified Files**:
- `apps/backend/bffs/Bot.BFF/Program.cs`
- `apps/backend/microservices/Player.Service/Program.cs`
- `apps/backend/microservices/Raid.Service/Program.cs`
- `apps/backend/microservices/Gym.Service/Program.cs`
- `apps/backend/microservices/Location.Service/Program.cs`
- `apps/backend/microservices/Account.Service/Program.cs`
- `apps/backend/microservices/OCR.Service/Program.cs`
- `apps/backend/microservices/Player.Service/Infrastructure/Data/PlayerDbContext.cs`
- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`

**New Files**:
- `apps/backend/microservices/Player.Service/Tests/Integration/PlayerEmojiTests.cs`
- `bruno/microservices/player-service/Create Player with Emoji.bru`

**Potential Migration**:
- If database columns need updating from varchar to nvarchar

## Testing Checklist

- [ ] Test with single emoji: "🔥"
- [ ] Test with multiple emojis: "🔥⚡️🌟"
- [ ] Test with emoji + text: "🔥FireMaster🔥"
- [ ] Test with skin tone modifiers: "👍🏽"
- [ ] Test with flag emojis: "🇺🇸"
- [ ] Test with Japanese characters: "日本語"
- [ ] Test with Chinese characters: "中文"
- [ ] Test with Arabic characters: "العربية"
- [ ] Test with special symbols: "♠️♣️♥️♦️"
- [ ] Test with zero-width joiners: "👨‍👩‍👧‍👦"

## Design Decisions

1. **Encoder choice**: Use `UnsafeRelaxedJsonEscaping` vs custom encoder?
   - Recommendation: `UnsafeRelaxedJsonEscaping` is sufficient and standard
   - Note: "Unsafe" refers to potential XSS in HTML contexts, not an issue for APIs

2. **Database migration**: Update existing columns or only new ones?
   - Recommendation: Update existing if any varchar columns exist

3. **Validation**: Should we validate/sanitize emoji input?
   - Recommendation: No sanitization, allow all valid Unicode
   - Consider max length validation only

