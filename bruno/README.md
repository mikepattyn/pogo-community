# Pogo API Collection for Bruno

This Bruno collection provides API requests for all microservices and BFFs in the Pogo system.

## Directory Structure

```
bruno/
├── bruno.json              # Collection configuration
├── README.md               # This file
├── environments/
│   └── Local.bru          # Local environment variables
├── microservices/
│   ├── account-service/   # Account Service endpoints
│   ├── player-service/    # Player Service endpoints
│   ├── location-service/  # Location Service endpoints
│   ├── gym-service/       # Gym Service endpoints
│   ├── raid-service/      # Raid Service endpoints
│   └── ocr-service/       # OCR Service endpoints
└── bffs/
    ├── app-bff/          # App BFF proxied endpoints
    └── bot-bff/          # Bot BFF proxied endpoints
```

## Environment Variables

The `Local.bru` environment defines the following service URLs:

- `account_service_url`: http://localhost:5001
- `player_service_url`: http://localhost:5002
- `location_service_url`: http://localhost:5003
- `gym_service_url`: http://localhost:5004
- `raid_service_url`: http://localhost:5005
- `ocr_service_url`: http://localhost:5006
- `app_bff_url`: http://localhost:6002
- `bot_bff_url`: http://localhost:6001

## Services Overview

### Account Service (`account-service`)

Authentication and account management.

**Endpoints:**

- `Create Account` - POST /api/account
- `Login` - POST /api/account/login
- `Get Account by Email` - GET /api/account/by-email/{email}
- `Get Account by Player ID` - GET /api/account/by-player/{playerId}

### Player Service (`player-service`)

Player profile management.

**Endpoints:**

- `Create Player` - POST /api/player
- `Update Player` - PUT /api/player/{id}
- `Deactivate Player` - DELETE /api/player/{id}
- `Get Player by ID` - GET /api/player/{id}
- `Get Player by Username` - GET /api/player/by-username/{username}
- `Get Player by Discord ID` - GET /api/player/by-discord/{discordUserId}
- `Get All Players` - GET /api/player

### Location Service (`location-service`)

Location and geography management.

**Endpoints:**

- `Create Location` - POST /api/location
- `Update Location` - PUT /api/location/{id}
- `Deactivate Location` - DELETE /api/location/{id}
- `Get Location by ID` - GET /api/location/{id}
- `Get Locations by Name` - GET /api/location/by-name/{name}
- `Get Locations by Type` - GET /api/location/by-type/{locationType}
- `Search Locations Nearby` - GET /api/location/search/nearby
- `Get All Locations` - GET /api/location

### Gym Service (`gym-service`)

Gym management and status tracking.

**Endpoints:**

- `Create Gym` - POST /api/gym
- `Update Gym` - PUT /api/gym/{id}
- `Update Gym Status` - PUT /api/gym/{id}/status
- `Deactivate Gym` - DELETE /api/gym/{id}
- `Get Gym by ID` - GET /api/gym/{id}
- `Get All Gyms` - GET /api/gym
- `Get Gyms by Location` - GET /api/gym/by-location/{locationId}
- `Get Gyms by Team` - GET /api/gym/by-team/{team}
- `Search Gyms Nearby` - GET /api/gym/search/nearby

### Raid Service (`raid-service`)

Raid scheduling and participation management.

**Endpoints:**

- `Create Raid` - POST /api/raid
- `Update Raid` - PUT /api/raid/{id}
- `Join Raid` - POST /api/raid/{id}/join
- `Leave Raid` - POST /api/raid/{id}/leave
- `Complete Raid` - POST /api/raid/{id}/complete
- `Cancel Raid` - POST /api/raid/{id}/cancel
- `Get Raid by ID` - GET /api/raid/{id}
- `Get Active Raids` - GET /api/raid/active
- `Get Raids by Gym` - GET /api/raid/by-gym/{gymId}
- `Get Raid Participants` - GET /api/raid/{id}/participants
- `Search Raids Nearby` - GET /api/raid/search/nearby

### OCR Service (`ocr-service`)

Optical Character Recognition for image text extraction.

**Endpoints:**

- `Scan Image` - POST /api/v1/scans

### BFFs (Backend for Frontend)

Both BFFs act as API gateways using Ocelot to proxy requests to microservices.

#### App BFF (`app-bff`)

Routes for the mobile app frontend.

**Proxied Services:**

- Account Service: `/api/account/{everything}`
- Player Service: `/api/player/{everything}`
- Location Service: `/api/location/{everything}`
- Gym Service: `/api/gym/{everything}`
- Raid Service: `/api/raid/{everything}`

**Base URL:** http://localhost:6002

#### Bot BFF (`bot-bff`)

Routes for the Discord bot frontend.

**Proxied Services:**

- Account Service: `/api/account/{everything}`
- Player Service: `/api/player/{everything}`
- Location Service: `/api/location/{everything}`
- Gym Service: `/api/gym/{everything}`
- Raid Service: `/api/raid/{everything}`
- OCR Service: `/api/ocr/{everything}`

**Base URL:** http://localhost:6001

## Usage

1. Import this collection into Bruno
2. Select the `Local` environment
3. Start the microservices using Docker Compose or individually
4. Make requests to the services

## Development

When adding new endpoints:

1. Add request files to the appropriate service directory
2. Use environment variables for URLs: `{{service_name_url}}`
3. Include documentation in the `docs` block
4. Use descriptive names for request files
