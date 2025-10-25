# 🛠️ POGO Community - Development Guide

## Overview

This guide covers setting up a local development environment for the POGO Community microservices architecture. You'll learn how to run individual services, debug issues, and contribute to the project.

## 🚀 Quick Start

### Prerequisites

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **Visual Studio Code** - [Download](https://code.visualstudio.com/)
- **Git** - [Download](https://git-scm.com/)

### Recommended VS Code Extensions

```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "ms-vscode.vscode-docker",
    "humao.rest-client",
    "ms-vscode.vscode-json",
    "bradlc.vscode-tailwindcss",
    "esbenp.prettier-vscode",
    "ms-vscode.vscode-eslint"
  ]
}
```

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
│   └── dotnet-shared/               # Shared .NET libraries
├── docs/                            # Documentation
├── docker-compose.yml               # Main Docker Compose
├── Makefile                         # Build commands
└── PogoMicroservices.sln           # .NET Solution file
```

## 🔧 Development Setup

### 1. Clone and Install

```bash
# Clone the repository
git clone <repository-url>
cd pogo

# Install dependencies
make install
# or
pnpm install
```

### 2. Environment Configuration

Create a `.env` file in the root directory:

```bash
# Database Configuration
MSSQL_SA_PASSWORD=YourStrong@Passw0rd123
MSSQL_ENCRYPT=false
MSSQL_TRUST_CERT=true

# JWT Configuration
JWT_KEY=your-super-secret-jwt-key-here-min-32-chars
JWT_ISSUER=POGO-Community
JWT_AUDIENCE=POGO-Community-Users
JWT_EXPIRE_MINUTES=60

# Discord Bot Configuration
DISCORD_BOT_TOKEN=your-discord-bot-token

# Development URLs
BOT_BFF_URL=http://localhost:6001
APP_BFF_URL=http://localhost:6002
```

### 3. Start Development Environment

#### Option A: Full Stack with Docker
```bash
# Start all services with Docker Compose
make microservices-start

# Check status
make microservices-status

# View logs
make microservices-logs
```

#### Option B: Hybrid Development
```bash
# Start only databases
docker-compose up -d account-db player-db location-db gym-db raid-db

# Run microservices locally
dotnet run --project apps/backend/microservices/Account.Service
dotnet run --project apps/backend/microservices/Player.Service
# ... etc
```

## 🏗️ Building and Running Services

### .NET Microservices

#### Build All Services
```bash
# Build entire solution
dotnet build PogoMicroservices.sln

# Build specific service
dotnet build apps/backend/microservices/Account.Service/Account.Service.csproj
```

#### Run Individual Services
```bash
# Run Account Service
cd apps/backend/microservices/Account.Service
dotnet run

# Run with specific configuration
dotnet run --environment Development --urls "http://localhost:5001"

# Run with debugging
dotnet run --launch-profile "https"
```

#### Database Migrations
```bash
# Add migration
dotnet ef migrations add InitialCreate --project apps/backend/microservices/Account.Service

# Update database
dotnet ef database update --project apps/backend/microservices/Account.Service

# Remove migration
dotnet ef migrations remove --project apps/backend/microservices/Account.Service
```

### Client Applications

#### Discord Bot
```bash
# Install dependencies
cd apps/frontend/bot
pnpm install

# Build
pnpm run build

# Run
pnpm run start

# Development mode
pnpm run dev
```

#### Mobile App
```bash
# Install dependencies
cd apps/frontend/mobile
pnpm install

# Run web version
pnpm run web

# Run Android
pnpm run android

# Run iOS
pnpm run ios
```

## 🐛 Debugging

### Visual Studio Code

#### 1. Configure Launch Settings

Create `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Account Service",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build-account-service",
      "program": "${workspaceFolder}/apps/backend/microservices/Account.Service/bin/Debug/net10.0/Account.Service.dll",
      "args": [],
      "cwd": "${workspaceFolder}/apps/backend/microservices/Account.Service",
      "console": "internalConsole",
      "stopAtEntry": false
    },
    {
      "name": "Player Service",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build-player-service",
      "program": "${workspaceFolder}/apps/backend/microservices/Player.Service/bin/Debug/net10.0/Player.Service.dll",
      "args": [],
      "cwd": "${workspaceFolder}/apps/backend/microservices/Player.Service",
      "console": "internalConsole",
      "stopAtEntry": false
    }
  ]
}
```

#### 2. Configure Tasks

Create `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build-account-service",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/apps/backend/microservices/Account.Service/Account.Service.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "build-player-service",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/apps/backend/microservices/Player.Service/Player.Service.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

### Docker Debugging

#### 1. Debug in Container

```dockerfile
# Add to Dockerfile for debugging
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS debug
WORKDIR /app
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5001"]
```

#### 2. Attach to Running Container

```bash
# Get container ID
docker ps

# Attach to container
docker exec -it <container-id> /bin/bash

# Check logs
docker logs -f <container-id>
```

## 🧪 Testing

### Unit Tests

```bash
# Run all tests
dotnet test PogoMicroservices.sln

# Run specific test project
dotnet test apps/backend/microservices/Account.Service.Tests/Account.Service.Tests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Integration Tests

```bash
# Run integration tests
dotnet test --filter "Category=Integration"

# Run with specific environment
dotnet test --environment Testing
```

### API Testing

#### Using REST Client

Create `test-requests.http`:

```http
### Create Account
POST http://localhost:5001/api/account
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "TestPassword123!",
  "playerId": 1
}

### Login
POST http://localhost:5001/api/account/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "TestPassword123!"
}

### Get Account by Email
GET http://localhost:5001/api/account/email/test@example.com
Authorization: Bearer <token>
```

#### Using cURL

```bash
# Test Account Service
curl -X POST http://localhost:5001/api/account \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"TestPassword123!","playerId":1}'

# Test Player Service
curl -X POST http://localhost:5002/api/player \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"username":"TestPlayer","level":40,"team":"Valor"}'
```

## 🔍 Troubleshooting

### Common Issues

#### 1. Database Connection Issues

**Problem:** Services can't connect to databases

**Solution:**
```bash
# Check if databases are running
docker ps | grep db

# Check database logs
docker logs account-db

# Test connection
docker exec -it account-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT 1"
```

#### 2. Port Conflicts

**Problem:** Port already in use

**Solution:**
```bash
# Find process using port
lsof -i :5001

# Kill process
kill -9 <PID>

# Or change port in appsettings.json
```

#### 3. Build Errors

**Problem:** .NET build failures

**Solution:**
```bash
# Clean and restore
dotnet clean
dotnet restore

# Clear NuGet cache
dotnet nuget locals all --clear

# Rebuild
dotnet build --no-restore
```

#### 4. Docker Issues

**Problem:** Docker build or run failures

**Solution:**
```bash
# Clean Docker
docker system prune -a

# Rebuild images
docker-compose build --no-cache

# Check Docker logs
docker-compose logs <service-name>
```

### Performance Issues

#### 1. Slow Database Queries

**Solution:**
- Add database indexes
- Optimize LINQ queries
- Use async/await properly
- Enable query logging

#### 2. Memory Issues

**Solution:**
- Check for memory leaks
- Optimize object disposal
- Use connection pooling
- Monitor garbage collection

## 📊 Monitoring and Logging

### Application Logs

#### 1. Console Logging

```csharp
// In Program.cs
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

#### 2. Structured Logging

```csharp
// In controllers
_logger.LogInformation("Creating player with username: {Username}", username);
_logger.LogError("Failed to create player: {Error}", ex.Message);
```

#### 3. Health Checks

```csharp
// Check service health
curl http://localhost:5001/health

// Check database health
curl http://localhost:5001/health/ready
```

### Database Monitoring

#### 1. Query Performance

```sql
-- Check slow queries
SELECT TOP 10
    query_stats.query_hash,
    query_stats.total_elapsed_time / query_stats.execution_count AS avg_elapsed_time,
    query_stats.execution_count,
    sql_text.text
FROM sys.dm_exec_query_stats AS query_stats
CROSS APPLY sys.dm_exec_sql_text(query_stats.sql_handle) AS sql_text
ORDER BY avg_elapsed_time DESC;
```

#### 2. Connection Monitoring

```sql
-- Check active connections
SELECT 
    session_id,
    login_name,
    host_name,
    program_name,
    status
FROM sys.dm_exec_sessions
WHERE is_user_process = 1;
```

## 🔄 Development Workflow

### 1. Feature Development

```bash
# Create feature branch
git checkout -b feature/new-feature

# Make changes
# ... code changes ...

# Test changes
make test

# Commit changes
git add .
git commit -m "Add new feature"

# Push branch
git push origin feature/new-feature
```

### 2. Code Review

```bash
# Create pull request
# ... via GitHub/GitLab ...

# Review checklist
- [ ] Code follows style guidelines
- [ ] Tests are included
- [ ] Documentation is updated
- [ ] No breaking changes
- [ ] Performance impact considered
```

### 3. Deployment

```bash
# Merge to main
git checkout main
git merge feature/new-feature

# Tag release
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0

# Deploy
make microservices-start
```

## 📚 Additional Resources

### Documentation
- [.NET 10 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [Ocelot API Gateway](https://ocelot.readthedocs.io/)

### Tools
- [Postman](https://www.postman.com/) - API testing
- [DBeaver](https://dbeaver.io/) - Database management
- [RedisInsight](https://redis.com/redis-enterprise/redis-insight/) - Redis management

### Learning Resources
- [Microservices Patterns](https://microservices.io/)
- [Domain-Driven Design](https://domainlanguage.com/ddd/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

This development guide provides everything you need to start contributing to the POGO Community microservices platform! 🚀
