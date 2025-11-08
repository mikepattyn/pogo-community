# API Versioning Standardization

## Overview

Standardize API versioning across all microservices and BFFs to use consistent `/api/v1/` routes, fixing 404 errors caused by route mismatches between bot client, BFF gateways, and microservices.

## Problem Statement

Currently there's an inconsistency in API versioning:
- **OCR Service**: Uses `[Route("api/v1/[controller]")]` → `/api/v1/scans`
- **All other services**: Use `[Route("api/[controller]")]` → `/api/player`, `/api/raid`, etc.
- **Bot client**: Expects versioned routes like `/api/player/v1/players`, `/api/raid/v1/raids`
- **BFF Ocelot config**: Routes `/api/player/{everything}` → downstream `/api/{everything}`
- **Result**: 404 errors when bot tries to call player/raid endpoints

## Tasks

### 1. Update Microservice Controllers

Change `[Route("api/[controller]")]` to `[Route("api/v1/[controller]")]` in:

- `apps/backend/microservices/Account.Service/API/Controllers/AccountController.cs`
- `apps/backend/microservices/Player.Service/API/Controllers/PlayerController.cs`
- `apps/backend/microservices/Raid.Service/API/Controllers/RaidController.cs`
- `apps/backend/microservices/Gym.Service/API/Controllers/GymController.cs`
- `apps/backend/microservices/Location.Service/API/Controllers/LocationController.cs`

Note: OCR service already correct at `api/v1/[controller]`

### 2. Update BFF Ocelot Routing

Modify Ocelot configuration files:
- `apps/backend/bffs/Bot.BFF/ocelot.json`
- `apps/backend/bffs/App.BFF/ocelot.json`

Changes:
- Update upstream patterns from `/api/{service}/{everything}` to `/api/v1/{service}/{everything}`
- Keep downstream as `/api/v1/{everything}` to match new controller routes

### 3. Update Bot Client Calls

Adjust `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`:
- Change URLs from `/api/player/v1/players` to `/api/v1/player/players`
- Pattern: `/api/v1/{service}/{resource}` instead of `/api/{service}/v1/{resource}`

## Verification

- [ ] All microservice routes include `/api/v1/` prefix
- [ ] BFF Ocelot configs route to versioned endpoints correctly
- [ ] Bot client calls match new BFF route patterns
- [ ] Test with Bruno collections to verify all routes work
- [ ] No 404 errors when calling player/raid endpoints

## Dependencies

None - this is a foundational change that other todos depend on.

## Files to Modify

**Controllers:**
- `apps/backend/microservices/Account.Service/API/Controllers/AccountController.cs`
- `apps/backend/microservices/Player.Service/API/Controllers/PlayerController.cs`
- `apps/backend/microservices/Raid.Service/API/Controllers/RaidController.cs`
- `apps/backend/microservices/Gym.Service/API/Controllers/GymController.cs`
- `apps/backend/microservices/Location.Service/API/Controllers/LocationController.cs`

**BFF Configs:**
- `apps/backend/bffs/Bot.BFF/ocelot.json`
- `apps/backend/bffs/App.BFF/ocelot.json`

**Bot Client:**
- `apps/frontend/bot/Infrastructure/Clients/BotBffClient.cs`

