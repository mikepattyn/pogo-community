# 🐳 Docker Setup Guide

This guide explains how to run the POGO Community applications using Docker and Docker Compose.

## 📋 Prerequisites

- Docker Engine 20.10 or later
- Docker Compose 2.0 or later
- At least 4GB of available RAM
- At least 10GB of available disk space

## 🚀 Quick Start

> **💡 Tip**: This project includes a Makefile with convenient Docker commands. Run `make help` to see all available commands.

### 1. Configure Environment Variables

Copy the example environment file and fill in your credentials:

```bash
cp .env.example .env
```

Edit `.env` and provide values for:
- Database passwords (MySQL and MSSQL)
- JWT secret key
- Discord bot token
- Google Cloud Project ID

### 2. Build and Start Services

**Using Make (Recommended):**
```bash
# Build and start all services
make docker-up-build

# View logs
make docker-logs

# View logs for specific service
make docker-logs-api
make docker-logs-bot
make docker-logs-app
```

**Using Docker Compose directly:**
```bash
# Build all images and start services
docker-compose up -d --build

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f api
```

### 3. Access the Applications

- **API Backend**: http://localhost:1000
- **Web App**: http://localhost:3000
- **Discord Bot**: Running in background (connects to Discord)
- **MySQL Database**: localhost:4000
- **MSSQL Database**: localhost:5000

### 4. Check Service Health

**Using Make:**
```bash
# Check status of all services
make docker-status

# Show running containers
make docker-ps
```

**Using Docker Compose:**
```bash
# Check status of all services
docker-compose ps
```

## 🏗️ Architecture

### Services

| Service | Description | Port Mapping | Database |
|---------|-------------|--------------|----------|
| `api` | REST API Backend | 1000 → 8080 | MySQL |
| `bot` | Discord Bot | 2000 → 2000 | MSSQL |
| `app` | Web Application | 3000 → 3000 | - |
| `mysql` | MySQL Database | 4000 → 3306 | - |
| `mssql` | MSSQL Database | 5000 → 1433 | - |

### Volumes

- `mysql-data`: Persistent storage for MySQL database
- `mssql-data`: Persistent storage for MSSQL database

### Network

All services communicate via the `pogo-network` bridge network.

## 📁 Project Structure

```
pogo/
├── apps/
│   ├── backend/
│   │   └── api/
│   │       └── .dockerignore
│   └── frontend/
│       ├── bot/
│       │   └── .dockerignore
│       └── mobile/
│           └── .dockerignore
├── databases/
│   ├── mysql/
│   │   └── init/
│   │       ├── 01-create-tables.sql
│   │       └── 02-seed-data.sql
│   └── mssql/
│       └── init/
│           ├── 01-create-database.sql
│           ├── 02-create-tables.sql
│           └── 03-seed-data.sql
├── api.Dockerfile
├── bot.Dockerfile
├── app.Dockerfile
├── mysql.Dockerfile
├── mssql.Dockerfile
├── docker-compose.yml
├── .env.example
└── DOCKER.md (this file)
```

## 🔧 Common Commands

### Using Make Commands

The Makefile provides convenient shortcuts for all Docker operations. Run `make help` to see all available commands.

#### Starting and Stopping

**Using Make:**
```bash
# Start all services
make docker-up

# Build and start all services
make docker-up-build

# Stop all services
make docker-down

# Stop and remove volumes (⚠️ deletes all data)
make docker-down-volumes

# Restart all services
make docker-restart

# Restart a specific service
make docker-restart-api
make docker-restart-bot
make docker-restart-app
```

**Using Docker Compose:**
```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# Stop and remove volumes (⚠️ deletes all data)
docker-compose down -v

# Restart a specific service
docker-compose restart api
```

#### Building

**Using Make:**
```bash
# Build all images
make docker-build

# Build specific service
make docker-build-api
make docker-build-bot
make docker-build-app
make docker-build-mysql
make docker-build-mssql
```

**Using Docker Compose:**
```bash
# Rebuild all images
docker-compose build

# Rebuild specific service
docker-compose build api

# Rebuild without cache
docker-compose build --no-cache
```

#### Logs and Debugging

**Using Make:**
```bash
# View logs for all services
make docker-logs

# View logs for specific service
make docker-logs-api
make docker-logs-bot
make docker-logs-app
make docker-logs-mysql
make docker-logs-mssql
```

**Using Docker Compose:**
```bash
# View logs for all services
docker-compose logs -f

# View logs for specific service
docker-compose logs -f bot

# View last 100 lines
docker-compose logs --tail=100 api

# Execute command in running container
docker-compose exec api sh
```

#### Database Access

**Using Make:**
```bash
# Connect to MySQL database (interactive)
make docker-db-mysql

# Connect to MSSQL database (interactive)
make docker-db-mssql
```

**From Host Machine:**
```bash
# Access MySQL database
mysql -h localhost -P 4000 -u root -p
# Password: value of MYSQL_ROOT_PASSWORD from .env

# Access MSSQL database
sqlcmd -S localhost,5000 -U sa -P 'YOUR_SA_PASSWORD'
```

**Using Docker Compose:**
```bash
# Access MySQL database (from container)
docker-compose exec mysql mysql -u root -p

# Access MSSQL database (from container)
docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa
```

#### Cleanup

**Using Make:**
```bash
# Remove stopped containers and unused images
make docker-clean

# Remove all Docker resources (⚠️ destructive)
make docker-clean-all
```

**Using Docker Compose:**
```bash
# Remove stopped containers
docker-compose down --remove-orphans

# Clean up system
docker system prune -f
```

## 🔍 Troubleshooting

### Services Won't Start

1. Check if ports are already in use:
   ```bash
   lsof -i :1000  # API
   lsof -i :2000  # Bot
   lsof -i :3000  # App
   ```

2. Check service logs:
   ```bash
   docker-compose logs api
   docker-compose logs bot
   ```

3. Verify environment variables:
   ```bash
   docker-compose config
   ```

### Database Connection Issues

1. Ensure databases are healthy:
   ```bash
   docker-compose ps
   ```

2. Check database logs:
   ```bash
   docker-compose logs mysql
   docker-compose logs mssql
   ```

3. Verify database initialization:
   ```bash
   # MySQL
   docker-compose exec mysql mysql -u root -p -e "SHOW DATABASES;"
   
   # MSSQL
   docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YOUR_SA_PASSWORD' -Q "SELECT name FROM sys.databases"
   ```

### Out of Memory

If services crash due to memory issues:

1. Increase Docker memory limit (Docker Desktop → Settings → Resources)
2. Reduce number of running services
3. Add memory limits to docker-compose.yml:
   ```yaml
   services:
     api:
       mem_limit: 512m
   ```

### Permission Issues

If you encounter permission errors:

```bash
# Fix ownership of volumes
docker-compose down
sudo chown -R $USER:$USER ./databases
docker-compose up -d
```

## 🔒 Security Notes

- **Never commit `.env` file** - it contains sensitive credentials
- **Change default passwords** - especially for production deployments
- **Use strong passwords** - MSSQL requires complex passwords
- **Limit network exposure** - databases are not exposed by default
- **Keep images updated** - regularly rebuild with latest base images

## 🚀 Production Deployment

For production deployments, consider:

1. **Use secrets management** instead of `.env` files
2. **Enable SSL/TLS** for all services
3. **Set up proper logging** and monitoring
4. **Configure backups** for database volumes
5. **Use specific image tags** instead of `latest`
6. **Implement health checks** and auto-restart policies
7. **Set resource limits** for all services
8. **Use external databases** for better scalability

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [MySQL Docker Image](https://hub.docker.com/_/mysql)
- [MSSQL Docker Image](https://hub.docker.com/_/microsoft-mssql-server)

## 🆘 Getting Help

If you encounter issues:

1. Check service logs: `docker-compose logs -f [service]`
2. Verify configuration: `docker-compose config`
3. Review this documentation
4. Check application-specific README files in each app directory

