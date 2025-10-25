# 🚀 POGO Community API Backend

A Node.js/Express API backend for the POGO Community ecosystem, providing REST endpoints for player management, raid coordination, gym tracking, and location services.

## 📋 Prerequisites

- **Node.js** >= 18.0.0
- **MySQL** or **Microsoft SQL Server** database
- **Google Cloud Platform** account (for Vision API and Logging)
- **pnpm** >= 8.0.0 (recommended) or npm

## 🛠️ Installation

1. **Navigate to the API directory:**

   ```bash
   cd apps/backend/api
   ```

2. **Install dependencies:**

   ```bash
   pnpm install
   # or
   npm install
   ```

3. **Set up environment variables:**
   Create a `.env` file in the API directory with the following variables:

   ```env
   PORT=8080
   JWT_KEY=your-jwt-secret-key-here
   CLOUD_SQL_CONNECTION_NAME=your-google-cloud-project-id

   # Database Configuration (choose one)
   # For MySQL:
   DB_HOST=localhost
   DB_PORT=3306
   DB_NAME=pogo_community
   DB_USER=your-username
   DB_PASSWORD=your-password

   # For MSSQL:
   DB_SERVER=localhost
   DB_DATABASE=pogo_community
   DB_USER=your-username
   DB_PASSWORD=your-password
   ```

4. **Set up Google Cloud Services:**
   - Enable Google Cloud Vision API
   - Enable Google Cloud Logging API
   - Create a service account with appropriate permissions
   - Set up authentication (see [Google Cloud Authentication](https://cloud.google.com/docs/authentication))

## 🔧 Environment Variables

| Variable                    | Description                         | Required | Default |
| --------------------------- | ----------------------------------- | -------- | ------- |
| `PORT`                      | Server port                         | No       | `8080`  |
| `JWT_KEY`                   | Secret key for JWT token signing    | Yes      | -       |
| `CLOUD_SQL_CONNECTION_NAME` | Google Cloud project ID for logging | Yes      | -       |
| `DB_HOST`                   | Database host (MySQL)               | Yes\*    | -       |
| `DB_PORT`                   | Database port (MySQL)               | No       | `3306`  |
| `DB_NAME`                   | Database name                       | Yes      | -       |
| `DB_USER`                   | Database username                   | Yes      | -       |
| `DB_PASSWORD`               | Database password                   | Yes      | -       |
| `DB_SERVER`                 | Database server (MSSQL)             | Yes\*    | -       |
| `DB_DATABASE`               | Database name (MSSQL)               | Yes\*    | -       |

\*Required for either MySQL or MSSQL configuration

## 🚀 Running the Application

### Development Mode

```bash
# Build TypeScript
pnpm run build

# Start the server
pnpm start

# Or run in watch mode
pnpm run dev
```

### Production Mode

```bash
# Build the application
pnpm run build

# Start the production server
pnpm start
```

The API will be available at `http://localhost:8080`

## 📡 API Endpoints

### Authentication

- `POST /api/v1/accounts/login` - User login
- `POST /api/v1/accounts/register` - User registration

### Players

- `GET /api/v1/players` - Get all players
- `POST /api/v1/players` - Create new player
- `GET /api/v1/players/:id` - Get player by ID
- `PUT /api/v1/players/:id` - Update player
- `DELETE /api/v1/players/:id` - Delete player

### Raids

- `GET /api/v1/raids` - Get all raids
- `POST /api/v1/raids` - Create new raid
- `GET /api/v1/raids/:id` - Get raid by ID
- `PUT /api/v1/raids/:id` - Update raid
- `DELETE /api/v1/raids/:id` - Delete raid

### Gyms

- `GET /api/v1/gyms` - Get all gyms
- `POST /api/v1/gyms` - Create new gym
- `GET /api/v1/gyms/:id` - Get gym by ID
- `PUT /api/v1/gyms/:id` - Update gym
- `DELETE /api/v1/gyms/:id` - Delete gym

### Locations

- `GET /api/v1/locations` - Get all locations
- `POST /api/v1/locations` - Create new location
- `GET /api/v1/locations/:id` - Get location by ID
- `PUT /api/v1/locations/:id` - Update location
- `DELETE /api/v1/locations/:id` - Delete location

### Image Scanning

- `POST /api/v1/scan` - Scan raid image using Google Vision API

### Status

- `GET /api/status` - Health check endpoint

## ☁️ Google Cloud Services

### Vision API

The API uses Google Cloud Vision API for image text detection, specifically for scanning raid images. Ensure you have:

- Vision API enabled in your Google Cloud project
- Proper authentication set up
- Sufficient API quotas

### Cloud Logging

Application logs are sent to Google Cloud Logging with the log name `Pokebot.Api.Debug`. Configure:

- Logging API enabled
- Appropriate IAM permissions for the service account

## 🏗️ Architecture

### Dependency Injection

The application uses **Inversify** for dependency injection, providing:

- Clean separation of concerns
- Easy testing and mocking
- Loose coupling between components

### Store Pattern

Data access is handled through store classes:

- `PlayerStore` - Player data operations
- `RaidStore` - Raid data operations
- `GymStore` - Gym data operations
- `LocationStore` - Location data operations
- `AuthStore` - Authentication data operations

### Controllers

REST endpoints are organized in controllers:

- `AccountController` - Authentication endpoints
- `PlayerController` - Player management
- `RaidController` - Raid management
- `GymController` - Gym management
- `LocationController` - Location management
- `ScanController` - Image scanning

## 🐳 Docker

> **TODO**: Docker configuration will be added in a future update.
>
> This section will include:
>
> - Dockerfile for the API
> - Docker Compose configuration
> - Environment variable handling
> - Database connection setup
> - Multi-stage build optimization

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

1. **Database Connection Errors**

   - Verify database credentials in `.env`
   - Ensure database server is running
   - Check network connectivity

2. **Google Cloud Authentication Issues**

   - Verify service account key is properly configured
   - Check API permissions and quotas
   - Ensure project ID is correct

3. **JWT Token Errors**

   - Verify `JWT_KEY` is set in environment
   - Ensure JWT key is consistent across services

4. **Port Already in Use**
   - Change `PORT` in `.env` file
   - Kill existing process using the port

### Logs

Application logs are available in:

- Console output (development)
- Google Cloud Logging (production)
- Log name: `Pokebot.Api.Debug`

## 📚 Additional Resources

- [Express.js Documentation](https://expressjs.com/)
- [Inversify Documentation](https://inversify.io/)
- [Google Cloud Vision API](https://cloud.google.com/vision)
- [Google Cloud Logging](https://cloud.google.com/logging)
- [JWT.io](https://jwt.io/) - JWT token debugging
