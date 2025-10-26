# 🎮 POGO Community Apps Overview

A comprehensive guide to the POGO Community ecosystem, including how all three applications work together, shared technologies, and complete setup instructions.

## 🌟 Ecosystem Overview

The POGO Community ecosystem consists of three interconnected applications that work together to provide a complete Pokemon GO community management solution:

### 🏗️ Architecture Diagram

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   📱 Mobile App │    │   🤖 Discord Bot│    │   🚀 API Backend│
│                 │    │                 │    │                 │
│ • React Native  │    │ • Discord.js    │    │ • Node.js/Express│
│ • Cross-platform│    │ • Command System│    │ • REST API      │
│ • Maps Integration│   │ • Reaction Handler│   │ • Database Layer│
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────┴─────────────┐
                    │     ☁️ Google Cloud       │
                    │                           │
                    │ • Vision API (Image Scan) │
                    │ • Datastore (NoSQL)       │
                    │ • Cloud Logging           │
                    └───────────────────────────┘
```

### 🔄 Data Flow

1. **Discord Bot** receives commands and reactions from users
2. **API Backend** processes requests and manages data
3. **Mobile App** provides cross-platform access to features
4. **Google Cloud** provides AI services and logging
5. **Database** stores persistent data (MySQL/MSSQL + Datastore)

## 🛠️ Common Technologies

### Shared Dependencies

All three applications share common technologies and patterns:

| Technology               | API Backend | Discord Bot | Mobile App | Purpose               |
| ------------------------ | ----------- | ----------- | ---------- | --------------------- |
| **TypeScript**           | ✅          | ✅          | ✅         | Type-safe JavaScript  |
| **Inversify**            | ✅          | ✅          | ✅         | Dependency Injection  |
| **Axios**                | ✅          | ✅          | ✅         | HTTP Client           |
| **Google Cloud Logging** | ✅          | ✅          | ✅         | Centralized Logging   |
| **Dotenv**               | ✅          | ✅          | ✅         | Environment Variables |
| **Jest**                 | ✅          | ✅          | ✅         | Testing Framework     |
| **ESLint/Prettier**      | ✅          | ✅          | ✅         | Code Quality          |

### Architecture Patterns

- **Dependency Injection** - Clean separation of concerns
- **Store Pattern** - Data access abstraction
- **Controller Pattern** - Request handling
- **Service Layer** - Business logic separation
- **Error Handling** - Consistent error management

## 🚀 Complete Environment Setup

### Prerequisites

- **Node.js** >= 18.0.0
- **pnpm** >= 8.0.0 (recommended)
- **Git** for version control
- **Google Cloud Platform** account
- **Discord Application** (for bot)
- **Database** (MySQL or MSSQL)

### 1. Clone and Install

```bash
# Clone the repository
git clone <repository-url>
cd pogo

# Install all dependencies
make install
# or
pnpm install
```

### 2. Environment Configuration

#### Root Environment File

Create a `.env` file in the project root:

```env
# Shared Configuration
NODE_ENV=development
GOOGLE_CLOUD_PROJECT_ID=your-google-cloud-project-id

# API Backend
API_PORT=8080
API_JWT_KEY=your-jwt-secret-key
API_DB_HOST=localhost
API_DB_PORT=3306
API_DB_NAME=pogo_community
API_DB_USER=your-db-username
API_DB_PASSWORD=your-db-password

# Discord Bot
DISCORD_BOT_TOKEN=your-discord-bot-token
BOT_API_URL=http://localhost:8080/api/v1

# Mobile App
MOBILE_API_URL=http://localhost:8080/api/v1
GOOGLE_MAPS_API_KEY=your-google-maps-api-key
```

#### Individual App Environment Files

Each app can have its own `.env` file for specific configuration:

**API Backend** (`apps/backend/api/.env`):

```env
PORT=8080
JWT_KEY=your-jwt-secret-key
CLOUD_SQL_CONNECTION_NAME=your-google-cloud-project-id
DB_HOST=localhost
DB_PORT=3306
DB_NAME=pogo_community
DB_USER=your-db-username
DB_PASSWORD=your-db-password
```

**Discord Bot** (`apps/frontend/bot/.env`):

```env
DISCORD_BOT_TOKEN=your-discord-bot-token
API_BASE_URL=http://localhost:8080/api/v1
GOOGLE_CLOUD_PROJECT_ID=your-google-cloud-project-id
```

**Mobile App** (`apps/frontend/mobile/.env`):

```env
API_BASE_URL=http://localhost:8080/api/v1
GOOGLE_MAPS_API_KEY=your-google-maps-api-key
GOOGLE_CLOUD_PROJECT_ID=your-google-cloud-project-id
```

### 3. Google Cloud Setup

#### Required APIs

Enable the following APIs in Google Cloud Console:

- **Vision API** - Image text detection
- **Datastore API** - NoSQL database
- **Logging API** - Centralized logging

#### Service Account

1. Create a service account
2. Download the JSON key file
3. Set environment variable: `GOOGLE_APPLICATION_CREDENTIALS=path/to/key.json`

### 4. Database Setup

#### MySQL Setup

```sql
CREATE DATABASE pogo_community;
CREATE USER 'pogo_user'@'localhost' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON pogo_community.* TO 'pogo_user'@'localhost';
FLUSH PRIVILEGES;
```

#### MSSQL Setup

```sql
CREATE DATABASE pogo_community;
CREATE LOGIN pogo_user WITH PASSWORD = 'your_password';
USE pogo_community;
CREATE USER pogo_user FOR LOGIN pogo_user;
ALTER ROLE db_owner ADD MEMBER pogo_user;
```

### 5. Discord Bot Setup

1. **Create Discord Application:**

   - Go to [Discord Developer Portal](https://discord.com/developers/applications)
   - Create new application
   - Create bot user
   - Copy bot token

2. **Configure Bot Permissions:**

   - Send Messages
   - Read Message History
   - Add Reactions
   - Manage Messages
   - Use Slash Commands
   - Embed Links
   - Attach Files

3. **Set Up Channels:**
   - Create required channels in your Discord server
   - Update channel IDs in `apps/frontend/bot/src/models/channelIds.enum.ts`

## 🔄 Development Workflow

### Starting All Applications

#### Option 1: Using Make Commands

```bash
# Start all apps in development mode
make dev

# Start specific apps
make run-api      # API backend only
make run-bot      # Discord bot only
make run-mobile-web  # Mobile app (web) only
```

#### Option 2: Using pnpm

```bash
# Start all apps
pnpm run dev

# Start specific apps
pnpm --filter @pogo/api run dev
pnpm --filter @pogo/bot run dev
pnpm --filter @pogo/mobile run dev
```

#### Option 3: Manual Start

```bash
# Terminal 1 - API Backend
cd apps/backend/api
pnpm run dev

# Terminal 2 - Discord Bot
cd apps/frontend/bot
pnpm run dev

# Terminal 3 - Mobile App
cd apps/frontend/mobile
pnpm run web
```

### Development Order

1. **Start API Backend** - Core data services
2. **Start Discord Bot** - Community interaction
3. **Start Mobile App** - Cross-platform access

### Testing the Integration

1. **API Health Check:** `curl http://localhost:8080/api/status`
2. **Discord Bot:** Send `!test` command in Discord
3. **Mobile App:** Open `http://localhost:19006` in browser

## 🧪 Testing Strategy

### Unit Testing

```bash
# Test all apps
make test

# Test specific apps
make test-api
make test-bot
make test-mobile
```

### Integration Testing

1. **API + Database** - Test data persistence
2. **Bot + API** - Test command processing
3. **Mobile + API** - Test data synchronization
4. **All Apps** - Test complete user workflows

### End-to-End Testing

1. **User Registration** - Discord → API → Mobile
2. **Raid Creation** - Mobile → API → Discord
3. **Raid Participation** - Discord → API → Mobile sync

## 🚀 Deployment Considerations

### Production Environment

- **API Backend:** Deploy to cloud platform (Google Cloud Run, AWS, etc.)
- **Discord Bot:** Deploy to cloud platform with persistent storage
- **Mobile App:** Build and distribute through app stores

### Environment Variables

- Use secure secret management
- Separate development/staging/production configs
- Rotate sensitive keys regularly

### Database

- Use managed database services
- Set up proper backups
- Configure connection pooling

### Monitoring

- Centralized logging with Google Cloud Logging
- Application performance monitoring
- Error tracking and alerting

## 🐳 Docker

> **TODO**: Docker configuration will be added in a future update.
>
> This section will include:
>
> - Multi-container Docker Compose setup
> - Individual Dockerfiles for each app
> - Environment variable management
> - Database containerization
> - Development vs production configurations
> - Container orchestration strategies

## 📊 Monitoring and Logging

### Centralized Logging

All applications send logs to Google Cloud Logging:

- **API Backend:** `Pokebot.Api.Debug`
- **Discord Bot:** `Pokebot.Bot.Debug`
- **Mobile App:** `Pokebot.Mobile.Debug`

### Health Checks

- **API:** `GET /api/status`
- **Bot:** `!test` command
- **Mobile:** App startup logs

### Performance Monitoring

- Response times
- Error rates
- Database query performance
- Memory usage

## 🔧 Troubleshooting

### Common Issues

1. **API Not Responding**

   - Check if API is running on correct port
   - Verify database connection
   - Check Google Cloud authentication

2. **Bot Not Responding**

   - Verify bot token is correct
   - Check bot permissions in Discord
   - Ensure API backend is accessible

3. **Mobile App Issues**

   - Check API connectivity
   - Verify Google Maps API key
   - Clear Metro cache: `npx expo start --clear`

4. **Database Connection Issues**
   - Verify database credentials
   - Check database server status
   - Test connection manually

### Debug Commands

```bash
# Check API status
curl http://localhost:8080/api/status

# Test Discord bot
!test

# Check mobile app
# Open browser console for web version
```

## 📚 Additional Resources

### Documentation

- [API Backend README](../apps/backend/api/README.md)
- [Discord Bot README](../apps/frontend/bot/README.md)
- [Mobile App README](../apps/frontend/mobile/README.md)

### External Resources

- [Node.js Documentation](https://nodejs.org/docs/)
- [Discord.js Documentation](https://discord.js.org/)
- [React Native Documentation](https://reactnative.dev/)
- [Google Cloud Documentation](https://cloud.google.com/docs)
- [Inversify Documentation](https://inversify.io/)

### Community

- [Discord.js Community](https://discord.gg/bRCvFy9)
- [React Native Community](https://reactnative.dev/community/overview)
- [Expo Community](https://forums.expo.dev/)

---

_This documentation is maintained alongside the codebase. Please update it when making changes to the applications or their configuration._
