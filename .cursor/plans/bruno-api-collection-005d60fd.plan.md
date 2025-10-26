<!-- 005d60fd-ff34-46e8-a00d-033364a50f80 48944e0a-283d-40b3-935c-345810be4912 -->

# Bruno API Collection Implementation Plan

## Phase 1: Implement Missing Controllers

### 1.1 Create GymController

Create `apps/backend/microservices/Gym.Service/API/Controllers/GymController.cs` with the following endpoints:

- POST `/api/gym` - Create gym (CreateGymCommand)
- PUT `/api/gym/{id}` - Update gym (UpdateGymCommand)
- PUT `/api/gym/{id}/status` - Update gym status (UpdateGymStatusCommand)
- DELETE `/api/gym/{id}` - Deactivate gym (DeactivateGymCommand)
- GET `/api/gym/{id}` - Get gym by ID (GetGymByIdQuery)
- GET `/api/gym` - Get all gyms with pagination (GetAllGymsQuery)
- GET `/api/gym/by-location/{locationId}` - Get gyms by location (GetGymsByLocationQuery)
- GET `/api/gym/by-team/{team}` - Get gyms by team (GetGymsByTeamQuery)
- GET `/api/gym/search/nearby` - Search gyms nearby (SearchGymsNearbyQuery)

Pattern: Follow the same structure as `AccountController.cs`, `PlayerController.cs`, and `LocationController.cs`

### 1.2 Create RaidController

Create `apps/backend/microservices/Raid.Service/API/Controllers/RaidController.cs` with the following endpoints:

- POST `/api/raid` - Create raid (CreateRaidCommand)
- PUT `/api/raid/{id}` - Update raid (UpdateRaidCommand)
- POST `/api/raid/{id}/join` - Join raid (JoinRaidCommand)
- POST `/api/raid/{id}/leave` - Leave raid (LeaveRaidCommand)
- POST `/api/raid/{id}/complete` - Complete raid (CompleteRaidCommand)
- POST `/api/raid/{id}/cancel` - Cancel raid (CancelRaidCommand)
- GET `/api/raid/{id}` - Get raid by ID (GetRaidByIdQuery)
- GET `/api/raid/active` - Get active raids (GetActiveRaidsQuery)
- GET `/api/raid/by-gym/{gymId}` - Get raids by gym (GetRaidsByGymQuery)
- GET `/api/raid/{id}/participants` - Get raid participants (GetRaidParticipantsQuery)
- GET `/api/raid/search/nearby` - Search raids nearby (SearchRaidsNearbyQuery)

Pattern: Follow the same structure as existing controllers with MediatR integration

## Phase 2: Create Bruno Collection Structure

### 2.1 Collection Root

Create `bruno/` directory at repository root with:

- `bruno.json` - Collection configuration
- `environments/` - Environment configurations

### 2.2 Environment Configuration

Create `bruno/environments/Local.bru` with variables:

- `account_service_url`: http://localhost:5001
- `player_service_url`: http://localhost:5002
- `location_service_url`: http://localhost:5003
- `gym_service_url`: http://localhost:5004
- `raid_service_url`: http://localhost:5005
- `ocr_service_url`: http://localhost:5006
- `app_bff_url`: http://localhost:6002
- `bot_bff_url`: http://localhost:6001

### 2.3 Microservices Folders

Create organized folder structure:

- `bruno/microservices/account-service/` - Account endpoints
- `bruno/microservices/player-service/` - Player endpoints
- `bruno/microservices/location-service/` - Location endpoints
- `bruno/microservices/gym-service/` - Gym endpoints
- `bruno/microservices/raid-service/` - Raid endpoints
- `bruno/microservices/ocr-service/` - OCR endpoints

### 2.4 BFF Folders

- `bruno/bffs/app-bff/` - App BFF proxied endpoints
- `bruno/bffs/bot-bff/` - Bot BFF proxied endpoints

## Phase 3: Create Bruno Request Files

### 3.1 Account Service Requests

- Create Account.bru
- Login.bru
- Get Account by Email.bru
- Get Account by Player ID.bru

### 3.2 Player Service Requests

- Create Player.bru
- Update Player.bru
- Deactivate Player.bru
- Get Player by ID.bru
- Get Player by Username.bru
- Get Player by Discord ID.bru
- Get All Players.bru

### 3.3 Location Service Requests

- Create Location.bru
- Update Location.bru
- Deactivate Location.bru
- Get Location by ID.bru
- Get Locations by Name.bru
- Get Locations by Type.bru
- Search Locations Nearby.bru
- Get All Locations.bru

### 3.4 Gym Service Requests

- Create Gym.bru
- Update Gym.bru
- Update Gym Status.bru
- Deactivate Gym.bru
- Get Gym by ID.bru
- Get All Gyms.bru
- Get Gyms by Location.bru
- Get Gyms by Team.bru
- Search Gyms Nearby.bru

### 3.5 Raid Service Requests

- Create Raid.bru
- Update Raid.bru
- Join Raid.bru
- Leave Raid.bru
- Complete Raid.bru
- Cancel Raid.bru
- Get Raid by ID.bru
- Get Active Raids.bru
- Get Raids by Gym.bru
- Get Raid Participants.bru
- Search Raids Nearby.bru

### 3.6 OCR Service Requests

- Scan Image.bru

### 3.7 BFF Requests

Create proxied versions of microservice endpoints through both BFFs using their respective base URLs and routing patterns from `ocelot.json`

## Key Implementation Details

- All .bru files use Bruno's native format with proper HTTP method, URL with environment variables, headers, and JSON body examples
- Include sample request bodies with realistic test data
- Add descriptive names and documentation for each endpoint
- Use consistent naming conventions across all requests
- Group related requests in appropriate folders

### To-dos

- [ ] Create GymController.cs with all CRUD and query endpoints using MediatR pattern
- [ ] Create RaidController.cs with all CRUD, participation, and query endpoints using MediatR pattern
- [ ] Create bruno/ directory structure with environments and organized folders for microservices and BFFs
- [ ] Create Bruno request files for Account Service endpoints
- [ ] Create Bruno request files for Player Service endpoints
- [ ] Create Bruno request files for Location Service endpoints
- [ ] Create Bruno request files for Gym Service endpoints
- [ ] Create Bruno request files for Raid Service endpoints
- [ ] Create Bruno request files for OCR Service endpoints
- [ ] Create Bruno request files for App BFF and Bot BFF proxied endpoints
