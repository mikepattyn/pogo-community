# 🤖 POGO Community Discord Bot

A Discord bot for the POGO Community ecosystem that facilitates raid coordination, player registration, and community management through interactive commands and reactions.

## 📋 Prerequisites

- **Node.js** >= 18.0.0
- **Discord Application** with bot token
- **SQL Server** database (for data storage)
- **pnpm** >= 8.0.0 (recommended) or npm
- **API Backend** running (for data operations)
- **Loki** for centralized logging (optional, for production)

## 🛠️ Installation

1. **Navigate to the bot directory:**

   ```bash
   cd apps/frontend/bot
   ```

2. **Install dependencies:**

   ```bash
   pnpm install
   # or
   npm install
   ```

3. **Set up environment variables:**
   Create a `.env` file in the bot directory:

   ```env
   DISCORD_BOT_TOKEN=your-discord-bot-token-here

   # Bot BFF Configuration
   BOT_BFF_URL=http://localhost:6001

   # Database Configuration
   DB_SERVER=localhost
   DB_PORT=1433
   DB_NAME=pogo
   DB_USER=sa
   DB_PASSWORD=YourStrong@Passw0rd

   # Loki Configuration (optional for production)
   LOKI_URL=http://localhost:3100
   LOG_LEVEL=info
   ```

4. **Set up local infrastructure:**
   - Ensure SQL Server is running and accessible
   - Run the database schema migrations to create bot tables
   - Optionally set up Loki for centralized logging

## 🔧 Environment Variables

| Variable            | Description                      | Required | Default                 |
| ------------------- | -------------------------------- | -------- | ----------------------- |
| `DISCORD_BOT_TOKEN` | Discord bot authentication token | Yes      | -                       |
| `BOT_BFF_URL`       | Bot BFF gateway URL              | Yes      | `http://localhost:6001` |
| `DB_SERVER`         | SQL Server host                  | Yes      | `localhost`             |
| `DB_PORT`           | SQL Server port                  | No       | `1433`                  |
| `DB_NAME`           | Database name                    | No       | `pogo`                  |
| `DB_USER`           | Database user                    | No       | `sa`                    |
| `DB_PASSWORD`       | Database password                | Yes      | -                       |
| `LOKI_URL`          | Loki logging endpoint            | No       | -                       |
| `LOG_LEVEL`         | Logging level                    | No       | `info`                  |

## 🎮 Discord Setup

### Creating a Discord Bot

1. **Go to Discord Developer Portal:**

   - Visit [https://discord.com/developers/applications](https://discord.com/developers/applications)
   - Click "New Application"
   - Give your bot a name

2. **Create Bot User:**

   - Go to "Bot" section
   - Click "Add Bot"
   - Copy the bot token (this is your `DISCORD_BOT_TOKEN`)

3. **Set Bot Permissions:**

   - Go to "OAuth2" > "URL Generator"
   - Select "bot" scope
   - Select required permissions:
     - Send Messages
     - Read Message History
     - Add Reactions
     - Manage Messages
     - Use Slash Commands
     - Embed Links
     - Attach Files

4. **Invite Bot to Server:**
   - Use the generated URL to invite the bot to your Discord server
   - Ensure the bot has appropriate roles and permissions

### Channel Configuration

The bot requires specific Discord channels to be configured. Update the channel IDs in `src/models/channelIds.enum.ts`:

```typescript
export enum ChannelIds {
  Welcome = 'YOUR_WELCOME_CHANNEL_ID',
  RaidRoeselare = 'YOUR_RAID_ROESELARE_CHANNEL_ID',
  RaidIzegem = 'YOUR_RAID_IZEGEM_CHANNEL_ID',
  RaidHooglede = 'YOUR_RAID_HOOGLEDE_CHANNEL_ID',
  RaidOekene = 'YOUR_RAID_OEKENE_CHANNEL_ID',
  RaidScanChannel = 'YOUR_RAID_SCAN_CHANNEL_ID',
}
```

### Getting Discord Channel IDs

1. **Enable Developer Mode:**

   - Go to User Settings > Advanced > Developer Mode (ON)

2. **Get Channel ID:**
   - Right-click on the channel
   - Select "Copy ID"
   - Paste the ID in the configuration

## 🚀 Running the Bot

### Development Mode

```bash
# Build TypeScript
pnpm run build

# Start the bot
pnpm start

# Or run in watch mode
pnpm run dev
```

### Production Mode

```bash
# Build the application
pnpm run build

# Start the production bot
pnpm start
```

## 🎯 Bot Commands

### Player Registration

```
!register (RANK) (FIRSTNAME) (INGAMENAME) (INGAMELVL)
```

- **RANK**: Player rank (e.g., Valor, Mystic, Instinct)
- **FIRSTNAME**: Player's first name
- **INGAMENAME**: In-game username
- **INGAMELVL**: Player level

**Example:**

```
!register Valor Mike TrainerMike 40
```

### Level Up Registration

```
!register levelup
```

Updates player level in the system.

### Raid Management

```
!raid start (TIER) (POKEMON_NAME) (TIME)
```

- **TIER**: Raid tier (1-5)
- **POKEMON_NAME**: Pokemon name
- **TIME**: Raid start time

**Example:**

```
!raid start 5 Mewtwo 19:30
```

### Raid Participation

#### Joining a Raid

- React with 👍 (thumbs up) to join a raid
- The bot will update the raid message with your nickname

#### Bringing Extra Players

- React with number emojis (1⃣-9⃣) to indicate extra players
- Shows you're bringing friends who aren't on Discord or extra accounts

#### Leaving a Raid

- Remove your reaction to leave the raid
- The bot will update the participant list

### Image Scanning

The bot can scan raid images using OCR (Optical Character Recognition) to extract raid information automatically. The OCR service uses Tesseract to extract text from raid images.

### Test Command

```
!test
```

Basic connectivity test to verify the bot is working.

## 🏗️ Architecture Components

### OCR Service

- **Purpose**: Image text detection for raid scanning
- **Technology**: Tesseract OCR
- **Usage**: Automatically extracts text from raid images via Bot BFF
- **Location**: Microservice at `/api/v1/scans` endpoint

### Database Storage

- **Purpose**: SQL Server database for bot data storage
- **Tables**: `bot_raids`, `bot_pokemon`, `bot_raid_participants`, `bot_logs`
- **Usage**: Stores raid data, Pokemon information, and bot state
- **Migration**: See `databases/mssql/init/04-create-bot-tables.sql`

### Centralized Logging (Loki)

- **Purpose**: Centralized logging for bot operations
- **Technology**: Grafana Loki with Winston transports
- **Usage**: Logs bot events, errors, and debugging information
- **Viewing**: Access logs via Grafana dashboards

## 🏗️ Architecture

### Dependency Injection

The bot uses **Inversify** for dependency injection, providing:

- Clean separation of concerns
- Easy testing and mocking
- Loose coupling between components

### Command System

Commands are organized using the `discord-message-handler` library:

- `RaidCommand` - Raid management
- `RegisterRankCommand` - Player registration
- `CounterCommand` - Counter information
- `JoinCommand` - Join functionality
- `ScanRaidImageCommand` - Image scanning
- `TestCommand` - Testing

### Message Reaction Handling

The bot handles Discord reactions for:

- Raid participation (👍)
- Extra player indication (1⃣-9⃣)
- Rank selection reactions

### API Integration

The bot communicates with the backend API through:

- `ApiClient` - HTTP client for API calls
- Automatic player registration on Discord join
- Real-time raid updates

## 🐳 Docker

> **TODO**: Docker configuration will be added in a future update.
>
> This section will include:
>
> - Dockerfile for the Discord bot
> - Docker Compose configuration
> - Environment variable handling
> - API connectivity setup
> - Multi-container orchestration

## 🧪 Testing

```bash
# Run all tests
pnpm test

# Run tests in watch mode
pnpm test:watch

# Run tests with coverage
pnpm test:coverage
```

## 🔍 Linting & Formatting

```bash
# Lint code
pnpm lint

# Fix linting issues
pnpm lint:fix

# Format code
pnpm format

# Check formatting
pnpm format:check
```

## 🚨 Troubleshooting

### Common Issues

1. **Bot Not Responding**

   - Check if `DISCORD_BOT_TOKEN` is correct
   - Verify bot has proper permissions
   - Check console for error messages

2. **API Connection Errors**

   - Ensure API backend is running
   - Verify `API_BASE_URL` is correct
   - Check network connectivity

3. **Database Connection Issues**

   - Verify SQL Server is running and accessible
   - Check database credentials are correct
   - Ensure bot tables are created (run schema migration)
   - Check network connectivity between bot and database

4. **OCR Service Issues**

   - Verify OCR service is running and accessible
   - Check Bot BFF can reach OCR service
   - Ensure Tesseract data files are available in OCR container

5. **Channel ID Errors**

   - Verify channel IDs in `channelIds.enum.ts`
   - Ensure bot has access to all configured channels
   - Check if channels still exist

6. **Reaction Not Working**
   - Verify bot has "Add Reactions" permission
   - Check if emoji reactions are in the correct channels
   - Ensure message hasn't been deleted

### Logs

Bot logs are available in:

- Console output (development)
- Loki (production, if configured)
- Winston logger for structured logging
- File-based logs (optional)

## 📚 Additional Resources

- [Discord.js Documentation](https://discord.js.org/)
- [Discord Developer Portal](https://discord.com/developers/applications)
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract)
- [Grafana Loki](https://grafana.com/docs/loki/latest/)
- [Winston Logger](https://github.com/winstonjs/winston)
- [Inversify Documentation](https://inversify.io/)
