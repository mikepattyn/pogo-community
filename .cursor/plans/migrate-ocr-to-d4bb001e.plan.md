<!-- d4bb001e-a78a-4473-bcdc-217163ec8cae cffcbccf-13f5-4244-893b-95391832233a -->
# Migrate OCR Service to Google Gemini Flash Vision API

## Overview

Replace the Tesseract-based OCR implementation with Google Gemini Flash Vision API to extract structured raid data from Pokemon Go screenshots. The service will return structured JSON data instead of raw text lines.

## Architecture Flow

```
Discord Bot (/scan command) 
  → Bot.BFF (port 6001, route: /api/ocr/*)
    → OCR.Service (port 5006)
      → Google Gemini Flash Vision API
```

## Implementation Steps

### 1. Update OCR.Service DTOs

**File**: `apps/backend/microservices/OCR.Service/Application/DTOs/ScanImageDto.cs`

Replace the current DTOs with structured raid data response:

- Keep `ScanImageRequest` with `Url` property
- Replace `ScanImageResponse` with structured fields:
  - `PokemonName` (string, lowercase)
  - `Tier` (int, 1-5)
  - `GymName` (string)
  - `CombatPower` (int)
  - `TimeRemaining` (string, HH:MM:SS or MM:SS format)
  - `GroupType` (string, "private" or "public")

### 2. Add Google Gemini NuGet Package

**File**: `apps/backend/microservices/OCR.Service/OCR.Service.csproj`

- Remove: `Tesseract` package (version 5.2.0)
- Add: `Google.Cloud.AIPlatform.V1` or equivalent Google Generative AI package for .NET
- Keep existing packages: MediatR, FluentValidation, prometheus-net, etc.

### 3. Create CQRS Commands/Queries Structure

**New files to create**:

- `Application/Commands/ExtractRaidDataCommand.cs` - Command with ImageUrl property
- `Application/Commands/ExtractRaidDataCommandHandler.cs` - Handler that calls Gemini service
- `Application/DTOs/RaidDataDto.cs` - Structured raid data DTO matching Python output

### 4. Implement Gemini Vision Service

**File**: `apps/backend/microservices/OCR.Service/Application/Services/OCRService.cs`

Replace Tesseract implementation with:

- Initialize Google Generative AI client with `gemini-2.0-flash-exp` model
- Read API key from environment variable `GOOGLE_API_KEY`
- Download image from URL using HttpClient
- Send image + structured prompt to Gemini API (same prompt as Python version)
- Parse JSON response (handle markdown code blocks: ```json)
- Validate required fields and return structured `RaidDataDto`
- Implement Result<T> pattern for error handling

Key prompt structure (from Python):

```
Extract the following data from this Pokemon Go raid screenshot and return as valid JSON only:
{
  "pokemon_name": "<string, lowercase, no spaces>",
  "tier": <integer 1-5>,
  "gym_name": "<string>",
  "combat_power": <integer>,
  "time_remaining": "<string in HH:MM:SS or MM:SS format>",
  "group_type": "<string: 'private' or 'public'>"
}
```

### 5. Update Interface

**File**: `apps/backend/microservices/OCR.Service/Application/Interfaces/IOCRService.cs`

Change method signature:

- From: `Task<string[]> ExtractTextFromImageAsync(string imageUrl, string[] languageHints)`
- To: `Task<Result<RaidDataDto>> ExtractRaidDataAsync(string imageUrl, CancellationToken cancellationToken)`

### 6. Update Controller to Use MediatR

**File**: `apps/backend/microservices/OCR.Service/API/Controllers/ScansController.cs`

- Inject `IMediator` instead of `IOCRService`
- Update POST endpoint to use `ExtractRaidDataCommand`
- Return `Result<RaidDataDto>` following the pattern from `GymController`
- Handle Result.IsFailure cases with BadRequest
- Return structured raid data on success

### 7. Register MediatR in Program.cs

**File**: `apps/backend/microservices/OCR.Service/Program.cs`

- Add `builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));`
- Keep existing service registrations (HttpClient, health checks, etc.)
- Ensure `IOCRService` is still registered for the command handler

### 8. Update Bot Integration

**File**: `apps/frontend/bot/Application/Commands/ScanCommand.cs`

Update `ParseRaidInfo` method:

- Change from parsing `string[] TextResults` to using structured `RaidDataDto`
- Remove manual parsing logic (regex for time, gym name, etc.)
- Directly use fields from the structured response
- Update `RaidScanResult` population with actual data from Gemini

**File**: `apps/frontend/bot/Application/DTOs/ScanResultDto.cs`

Update DTOs to match new structure:

- Update `ScanImageResponse` to include structured raid data fields
- Match the fields from `RaidDataDto` in OCR.Service

### 9. Configuration & Environment

**Files**:

- `apps/backend/microservices/OCR.Service/appsettings.json`
- `apps/backend/microservices/OCR.Service/appsettings.Development.json`

Add configuration section for Google API (optional, since we'll use environment variable):

```json
{
  "GoogleAI": {
    "Model": "gemini-2.0-flash-exp"
  }
}
```

**Environment Variables**:

- Ensure `GOOGLE_API_KEY` is set in deployment environments
- Update Kubernetes secrets/config if needed

### 10. Update Bot.BFF Ocelot Route

**File**: `apps/backend/bffs/Bot.BFF/ocelot.json`

Verify OCR service route is correctly configured (already exists at lines 63-74):

- Upstream: `/api/ocr/{everything}`
- Downstream: `ocr-service:5006`
- Methods: GET, POST

## Key Files to Modify

1. `OCR.Service/Application/DTOs/ScanImageDto.cs` - New response structure
2. `OCR.Service/Application/Services/OCRService.cs` - Gemini implementation
3. `OCR.Service/Application/Interfaces/IOCRService.cs` - Updated interface
4. `OCR.Service/API/Controllers/ScansController.cs` - MediatR integration
5. `OCR.Service/Program.cs` - Add MediatR registration
6. `OCR.Service/OCR.Service.csproj` - Package updates
7. `bot/Application/Commands/ScanCommand.cs` - Use structured data
8. `bot/Application/DTOs/ScanResultDto.cs` - Match new structure

## Testing

- Test via Bruno: `bruno/microservices/ocr-service/` endpoints
- Test via Discord bot: `/scan T5` command with raid screenshot attachment
- Verify structured data flows through: Bot → Bot.BFF → OCR.Service → Gemini API

### To-dos

- [ ] Update OCR.Service DTOs to return structured raid data instead of text array
- [ ] Replace Tesseract with Google Generative AI NuGet package in OCR.Service.csproj
- [ ] Create CQRS structure: ExtractRaidDataCommand, Handler, and RaidDataDto
- [ ] Implement Gemini Vision API integration in OCRService with structured prompt
- [ ] Update IOCRService interface to return Result<RaidDataDto>
- [ ] Update ScansController to use MediatR and Result pattern
- [ ] Register MediatR in OCR.Service Program.cs
- [ ] Update Discord bot ScanCommand to use structured data instead of parsing text
- [ ] Update bot ScanResultDto to match new structured response
- [ ] Test end-to-end: Discord /scan command → Bot.BFF → OCR.Service → Gemini API