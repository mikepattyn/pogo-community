# 🚀 POGO Community - Microservices Architecture

A modern Pokemon GO community ecosystem built with .NET microservices, featuring a Discord bot, React Native mobile app, and comprehensive backend services.

## 🏗️ Architecture Overview

This project has been migrated from a monolithic Node.js API to a modern **microservices architecture** using .NET 10, featuring:

- **5 Microservices** - Each with its own database and domain
- **2 Backend for Frontend (BFF)** services - API gateways for client applications
- **Clean Architecture** - Domain-driven design with CQRS pattern
- **Containerized** - Full Docker support with health checks
- **Scalable** - Independent scaling and deployment

## 🎯 Services

### 🔐 **Account Service** (Port 5001)

- User authentication and account management
- JWT token generation and validation
- Password hashing with BCrypt
- Account creation and login

### 👤 **Player Service** (Port 5002)

- Player profiles and management
- Team assignments (Valor, Mystic, Instinct)
- Friend codes and Discord integration
- Player statistics and levels

### 📍 **Location Service** (Port 5003)

- Geographical locations and POIs
- Spatial search with radius queries
- Address management and geocoding
- Location types (Gym, Pokestop, etc.)

### 🏟️ **Gym Service** (Port 5004)

- Pokemon Gym management
- Team control and motivation tracking
- Gym status and attack monitoring
- Integration with Location service

### ⚔️ **Raid Service** (Port 5005)

- Pokemon Raid management
- Raid scheduling and participation
- Player join/leave functionality
- Integration with Gym and Player services

### 🤖 **Bot BFF** (Port 6001)

- API Gateway for Discord bot
- Ocelot routing and load balancing
- Request aggregation and transformation
- CORS and authentication handling

### 📱 **App BFF** (Port 6002)

- API Gateway for mobile app
- Ocelot routing and load balancing
- Request aggregation and transformation
- CORS and authentication handling

## 🚀 Quick Start

### Prerequisites

- **.NET 10 SDK** - For microservices development
- **Docker & Docker Compose** - For containerization
- **Node.js >= 18.0.0** - For client applications
- **pnpm >= 8.0.0** - Package manager

### Installation

```bash
# Clone the repository
git clone <repository-url>
cd pogo

# Install dependencies
make install
# or
pnpm install
```

### Development

#### Option 1: Full Microservices Stack

```bash
# Start all microservices with Docker Compose
make microservices-start

# Check status
make microservices-status

# View logs
make microservices-logs

# Health check
make microservices-health
```

#### Option 2: Individual Services

```bash
# Build all microservices
make microservices-build

# Run specific services locally (requires databases)
dotnet run --project apps/backend/microservices/Account.Service
dotnet run --project apps/backend/microservices/Player.Service
# ... etc
```

#### Option 3: Client Applications

```bash
# Run Discord bot
make run-bot

# Run mobile app (web)
make run-mobile-web

# Run mobile app (Android)
make run-mobile-android

# Run mobile app (iOS)
make run-mobile-ios
```

## 🛠️ Available Commands

### General Commands

| Command        | Description                      |
| -------------- | -------------------------------- |
| `make help`    | Show all available commands      |
| `make install` | Install all dependencies         |
| `make build`   | Build all applications           |
| `make dev`     | Run all apps in development mode |
| `make test`    | Run all tests                    |
| `make lint`    | Lint all applications            |

### Microservices Commands

| Command                      | Description                         |
| ---------------------------- | ----------------------------------- |
| `make microservices`         | Show microservices help             |
| `make microservices-build`   | Build all microservices and BFFs    |
| `make microservices-start`   | Start all microservices with Docker |
| `make microservices-stop`    | Stop all microservices              |
| `make microservices-restart` | Restart all microservices           |
| `make microservices-logs`    | View logs for all microservices     |
| `make microservices-status`  | Show status of all microservices    |
| `make microservices-health`  | Check health of all microservices   |
| `make microservices-clean`   | Clean up microservices resources    |

### Docker Commands

| Command              | Description                    |
| -------------------- | ------------------------------ |
| `make docker-up`     | Start all services with Docker |
| `make docker-down`   | Stop all services              |
| `make docker-logs`   | View logs for all services     |
| `make docker-status` | Show service status            |

## 📁 Project Structure

```
pogo/
├── apps/
│   ├── backend/
│   │   ├── microservices/           # .NET Microservices
│   │   │   ├── Account.Service/     # User authentication
│   │   │   ├── Player.Service/      # Player management
│   │   │   ├── Location.Service/    # Location management
│   │   │   ├── Gym.Service/         # Gym management
│   │   │   └── Raid.Service/        # Raid management
│   │   └── bffs/                    # Backend for Frontend
│   │       ├── Bot.BFF/             # Discord bot gateway
│   │       └── App.BFF/             # Mobile app gateway
│   └── frontend/
│       ├── bot/                     # Discord bot (Node.js)
│       └── mobile/                  # React Native mobile app
├── packages/
│   ├── dotnet-shared/               # Shared .NET libraries
│   │   ├── Pogo.Shared.Kernel/      # Domain entities and base classes
│   │   ├── Pogo.Shared.Infrastructure/ # EF Core and repositories
│   │   ├── Pogo.Shared.Application/ # MediatR and CQRS
│   │   └── Pogo.Shared.API/         # API utilities and extensions
│   └── database/                    # Legacy database package
├── archive/
│   └── old-api/                     # Archived Node.js API
├── docker-compose.yml               # Main Docker Compose
├── docker-compose.microservices.yml # Microservices-only Docker Compose
├── Makefile                         # Main build commands
├── Makefile.microservices           # Microservices-specific commands
└── PogoMicroservices.sln           # .NET Solution file
```

## 🏗️ Technical Stack

### Backend (.NET 10)

- **ASP.NET Core** - Web framework with minimal APIs
- **Entity Framework Core** - ORM with separate databases per service
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **Ocelot** - API Gateway for BFF services
- **BCrypt.Net-Next** - Password hashing
- **JWT Authentication** - Token-based auth

### Frontend

- **Discord.js** - Discord bot framework
- **React Native** - Mobile app framework
- **Expo** - Development platform
- **TypeScript** - Type-safe JavaScript

### Infrastructure

- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **SQL Server** - Database (separate instance per service)
- **Health Checks** - Service monitoring

## 🔧 Development Tools

### VS Code Extensions

- **C# Dev Kit** - .NET development
- **Docker** - Container management
- **REST Client** - API testing
- **GitLens** - Git integration

### Code Quality

- **ESLint** - JavaScript/TypeScript linting
- **Prettier** - Code formatting
- **Jest** - Testing framework
- **FluentValidation** - .NET validation

## 🌐 API Endpoints

### Account Service

- `POST /api/account` - Create account
- `POST /api/account/login` - Login
- `GET /api/account/email/{email}` - Get account by email

### Player Service

- `POST /api/player` - Create player
- `GET /api/player/{id}` - Get player by ID
- `GET /api/player/username/{username}` - Get player by username
- `GET /api/player/discord/{discordId}` - Get player by Discord ID

### Location Service

- `POST /api/location` - Create location
- `GET /api/location/{id}` - Get location by ID
- `GET /api/location/search/nearby` - Search nearby locations

### Gym Service

- `POST /api/gym` - Create gym
- `GET /api/gym/{id}` - Get gym by ID
- `GET /api/gym/location/{locationId}` - Get gyms by location
- `GET /api/gym/search/nearby` - Search nearby gyms

### Raid Service

- `POST /api/raid` - Create raid
- `GET /api/raid/{id}` - Get raid by ID
- `GET /api/raid/active` - Get active raids
- `POST /api/raid/join` - Join raid
- `POST /api/raid/leave` - Leave raid

## 🐳 Docker Services

| Service          | Port | Database         | Description         |
| ---------------- | ---- | ---------------- | ------------------- |
| account-service  | 5001 | account-db:1433  | User authentication |
| player-service   | 5002 | player-db:1434   | Player management   |
| location-service | 5003 | location-db:1435 | Location management |
| gym-service      | 5004 | gym-db:1436      | Gym management      |
| raid-service     | 5005 | raid-db:1437     | Raid management     |
| bot-bff          | 6001 | -                | Discord bot gateway |
| app-bff          | 6002 | -                | Mobile app gateway  |
| bot              | 2000 | -                | Discord bot         |
| app              | 3000 | -                | Mobile app          |

## 🔄 Migration from Monolith

This project has been successfully migrated from a monolithic Node.js API to a modern microservices architecture:

### ✅ **What Was Migrated:**

- **5 Core Services** - Account, Player, Location, Gym, Raid
- **2 BFF Services** - Bot and App gateways
- **Database Per Service** - Complete data isolation
- **Clean Architecture** - Domain-driven design
- **Containerization** - Full Docker support

### ✅ **What Was Preserved:**

- **Client Applications** - Bot and mobile app functionality
- **Business Logic** - All core features maintained
- **Data Models** - Equivalent entities and relationships
- **API Contracts** - Compatible endpoints

### ✅ **What Was Improved:**

- **Scalability** - Independent service scaling
- **Maintainability** - Clear separation of concerns
- **Reliability** - Service isolation prevents cascading failures
- **Technology Stack** - Modern .NET 10 with best practices
- **Development Experience** - Better tooling and debugging

## 📚 Documentation

- [Architecture Overview](docs/architecture.md) - Detailed architecture documentation
- [API Documentation](docs/api.md) - Complete API reference
- [Deployment Guide](docs/deployment.md) - Production deployment instructions
- [Development Guide](docs/development.md) - Local development setup

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Built with ❤️ for the Pokemon GO community!** 🎮✨
