# 📚 POGO Community - API Documentation

## Overview

The POGO Community platform provides a comprehensive REST API through microservices architecture. All client applications communicate through Backend for Frontend (BFF) services that aggregate data from individual microservices.

## 🌐 Base URLs

### Production
- **Bot BFF:** `https://api.pogo-community.com/bot`
- **App BFF:** `https://api.pogo-community.com/app`

### Development
- **Bot BFF:** `http://localhost:6001`
- **App BFF:** `http://localhost:6002`

### Direct Microservices (Internal)
- **Account Service:** `http://localhost:5001`
- **Player Service:** `http://localhost:5002`
- **Location Service:** `http://localhost:5003`
- **Gym Service:** `http://localhost:5004`
- **Raid Service:** `http://localhost:5005`

## 🔐 Authentication

All API endpoints require JWT authentication. Include the token in the Authorization header:

```http
Authorization: Bearer <your-jwt-token>
```

### Getting a Token
```http
POST /api/account/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "your-password"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "account": {
      "id": 1,
      "email": "user@example.com",
      "playerId": 123,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z",
      "isDeleted": false
    }
  }
}
```

## 📋 Common Response Format

All API responses follow this format:

```json
{
  "success": true,
  "data": { ... },
  "message": "Optional message",
  "errors": []
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

## 🔐 Account Service API

### Create Account
```http
POST /api/account
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "secure-password",
  "playerId": 123
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "playerId": 123,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z",
    "isDeleted": false
  }
}
```

### Login
```http
POST /api/account/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "secure-password"
}
```

### Get Account by Email
```http
GET /api/account/email/{email}
Authorization: Bearer <token>
```

### Get Account by Player ID
```http
GET /api/account/player/{playerId}
Authorization: Bearer <token>
```

## 👤 Player Service API

### Create Player
```http
POST /api/player
Content-Type: application/json

{
  "username": "TrainerName",
  "level": 40,
  "team": "Valor",
  "friendCode": "123456789012",
  "discordId": "123456789012345678"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "username": "TrainerName",
    "level": 40,
    "team": "Valor",
    "friendCode": "123456789012",
    "discordId": "123456789012345678",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z",
    "isDeleted": false
  }
}
```

### Get Player by ID
```http
GET /api/player/{id}
Authorization: Bearer <token>
```

### Get Player by Username
```http
GET /api/player/username/{username}
Authorization: Bearer <token>
```

### Get Player by Discord ID
```http
GET /api/player/discord/{discordId}
Authorization: Bearer <token>
```

### Get All Players
```http
GET /api/player?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Update Player
```http
PUT /api/player/{id}
Content-Type: application/json
Authorization: Bearer <token>

{
  "username": "NewTrainerName",
  "level": 41,
  "team": "Mystic",
  "friendCode": "987654321098"
}
```

### Deactivate Player
```http
DELETE /api/player/{id}
Authorization: Bearer <token>
```

## 📍 Location Service API

### Create Location
```http
POST /api/location
Content-Type: application/json

{
  "name": "Central Park",
  "type": "Gym",
  "latitude": 40.7829,
  "longitude": -73.9654,
  "description": "Main entrance of Central Park",
  "address": "Central Park, New York, NY",
  "city": "New York",
  "state": "NY",
  "country": "USA"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Central Park",
    "type": "Gym",
    "latitude": 40.7829,
    "longitude": -73.9654,
    "description": "Main entrance of Central Park",
    "address": "Central Park, New York, NY",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### Get Location by ID
```http
GET /api/location/{id}
Authorization: Bearer <token>
```

### Get Locations by Name
```http
GET /api/location/name/{name}?includeDeleted=false
Authorization: Bearer <token>
```

### Get Locations by Type
```http
GET /api/location/type/{type}?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Search Locations Nearby
```http
GET /api/location/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&type=Gym&maxResults=50
Authorization: Bearer <token>
```

### Get All Locations
```http
GET /api/location?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Update Location
```http
PUT /api/location/{id}
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "Updated Central Park",
  "description": "Updated description"
}
```

### Deactivate Location
```http
DELETE /api/location/{id}
Authorization: Bearer <token>
```

## 🏟️ Gym Service API

### Create Gym
```http
POST /api/gym
Content-Type: application/json

{
  "name": "Central Park Gym",
  "locationId": 1,
  "level": 3,
  "team": "Valor"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Central Park Gym",
    "locationId": 1,
    "level": 3,
    "team": "Valor",
    "isActive": true,
    "isUnderAttack": false,
    "isInRaid": false,
    "motivationLevel": 100,
    "lastControlledByTeam": "Valor",
    "lastControlledAt": "2024-01-01T00:00:00Z",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### Get Gym by ID
```http
GET /api/gym/{id}
Authorization: Bearer <token>
```

### Get Gyms by Location
```http
GET /api/gym/location/{locationId}?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Get Gyms by Team
```http
GET /api/gym/team/{team}?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Search Gyms Nearby
```http
GET /api/gym/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&maxResults=50
Authorization: Bearer <token>
```

### Get All Gyms
```http
GET /api/gym?page=1&pageSize=10&includeDeleted=false
Authorization: Bearer <token>
```

### Update Gym
```http
PUT /api/gym/{id}
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "Updated Gym Name",
  "team": "Mystic"
}
```

### Update Gym Status
```http
PUT /api/gym/{id}/status
Content-Type: application/json
Authorization: Bearer <token>

{
  "team": "Mystic",
  "isUnderAttack": false,
  "isInRaid": true,
  "motivationLevel": 75
}
```

### Deactivate Gym
```http
DELETE /api/gym/{id}
Authorization: Bearer <token>
```

## ⚔️ Raid Service API

### Create Raid
```http
POST /api/raid
Content-Type: application/json

{
  "gymId": 1,
  "pokemonSpecies": "Mewtwo",
  "level": 5,
  "startTime": "2024-01-01T12:00:00Z",
  "endTime": "2024-01-01T13:00:00Z",
  "maxParticipants": 20,
  "difficulty": "Hard",
  "weatherBoost": "Windy",
  "notes": "Bring your best counters!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "gymId": 1,
    "pokemonSpecies": "Mewtwo",
    "level": 5,
    "startTime": "2024-01-01T12:00:00Z",
    "endTime": "2024-01-01T13:00:00Z",
    "isActive": true,
    "isCompleted": false,
    "isCancelled": false,
    "maxParticipants": 20,
    "currentParticipants": 0,
    "difficulty": "Hard",
    "weatherBoost": "Windy",
    "notes": "Bring your best counters!",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### Get Raid by ID
```http
GET /api/raid/{id}
Authorization: Bearer <token>
```

### Get Raids by Gym
```http
GET /api/raid/gym/{gymId}?activeOnly=true
Authorization: Bearer <token>
```

### Get Active Raids
```http
GET /api/raid/active?page=1&pageSize=10
Authorization: Bearer <token>
```

### Search Raids Nearby
```http
GET /api/raid/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&maxResults=50
Authorization: Bearer <token>
```

### Get Raid Participants
```http
GET /api/raid/{id}/participants
Authorization: Bearer <token>
```

### Join Raid
```http
POST /api/raid/join
Content-Type: application/json
Authorization: Bearer <token>

{
  "raidId": 1,
  "playerId": 123
}
```

### Leave Raid
```http
POST /api/raid/leave
Content-Type: application/json
Authorization: Bearer <token>

{
  "raidId": 1,
  "playerId": 123
}
```

### Complete Raid
```http
POST /api/raid/{id}/complete
Authorization: Bearer <token>
```

### Cancel Raid
```http
POST /api/raid/{id}/cancel
Authorization: Bearer <token>
```

## 🔍 Health Check Endpoints

### Service Health
```http
GET /health
```

**Response:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "sqlserver",
      "status": "Healthy",
      "description": "Database connection is healthy"
    }
  ]
}
```

### Ready Check
```http
GET /health/ready
```

### Live Check
```http
GET /health/live
```

## 📊 Pagination

Many endpoints support pagination:

```http
GET /api/player?page=1&pageSize=10&includeDeleted=false
```

**Parameters:**
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)
- `includeDeleted` - Include soft-deleted items (default: false)

## 🔍 Filtering and Search

### Location Search
```http
GET /api/location/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&type=Gym&maxResults=50
```

**Parameters:**
- `latitude` - Center latitude (required)
- `longitude` - Center longitude (required)
- `radiusKm` - Search radius in kilometers (default: 10)
- `type` - Location type filter (optional)
- `maxResults` - Maximum results to return (default: 50)

### Gym Search
```http
GET /api/gym/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&maxResults=50
```

### Raid Search
```http
GET /api/raid/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10&maxResults=50
```

## 🚨 Error Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 422 | Validation Error |
| 500 | Internal Server Error |

## 📝 Rate Limiting

- **Rate Limit:** 1000 requests per hour per IP
- **Burst Limit:** 100 requests per minute
- **Headers:**
  - `X-RateLimit-Limit` - Request limit per hour
  - `X-RateLimit-Remaining` - Remaining requests
  - `X-RateLimit-Reset` - Reset time (Unix timestamp)

## 🔧 SDKs and Examples

### JavaScript/TypeScript
```typescript
// Using the MicroservicesClient
const client = new MicroservicesClient();

// Create a player
const player = await client.createPlayer({
  username: "TrainerName",
  level: 40,
  team: "Valor"
});

// Search nearby gyms
const gyms = await client.searchGymsNearby(40.7829, -73.9654, 10);
```

### cURL Examples
```bash
# Create a player
curl -X POST http://localhost:6001/api/player \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"username":"TrainerName","level":40,"team":"Valor"}'

# Search nearby locations
curl -X GET "http://localhost:6001/api/location/search/nearby?latitude=40.7829&longitude=-73.9654&radiusKm=10" \
  -H "Authorization: Bearer <token>"
```

---

For more detailed information about specific endpoints, check the individual service documentation or use the Swagger UI at `/swagger` when running in development mode.
