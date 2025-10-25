# 🐳 Docker Setup Guide

This guide explains how to run the POGO Community applications using Docker and Docker Compose.

## 📋 Prerequisites

- Docker Engine 20.10 or later
- Docker Compose 2.0 or later
- At least 4GB of available RAM
- At least 10GB of available disk space

## 🚀 Quick Start

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

```bash
# Build all images and start services
docker-compose up -d

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f api
docker-compose logs -f bot
docker-compose logs -f app
```

### 3. Access the Applications

- **API Backend**: http://localhost:1000
- **Web App**: http://localhost:3000
- **Discord Bot**: Running in background (connects to Discord)

### 4. Check Service Health

```bash
# Check status of all services
docker-compose ps

# Check health of specific service
docker-compose ps api
```

## 🏗️ Architecture

### Services

| Service | Description | Port Mapping | Database |
|---------|-------------|--------------|----------|
| `api` | REST API Backend | 1000 → 8080 | MySQL |
| `bot` | Discord Bot | 2000 → 2000 | MSSQL |
| `app` | Web Application | 3000 → 3000 | - |
| `mysql` | MySQL Database | Internal only | - |
| `mssql` | MSSQL Database | Internal only | - |

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
│   │       ├── api.Dockerfile
│   │       └── .dockerignore
│   └── frontend/
│       ├── bot/
│       │   ├── bot.Dockerfile
│       │   └── .dockerignore
│       └── mobile/
│           ├── app.Dockerfile
│           └── .dockerignore
├── databases/
│   ├── mysql/
│   │   ├── mysql.Dockerfile
│   │   └── init/
│   │       ├── 01-create-tables.sql
│   │       └── 02-seed-data.sql
│   └── mssql/
│       ├── mssql.Dockerfile
│       └── init/
│           ├── 01-create-database.sql
│           ├── 02-create-tables.sql
│           └── 03-seed-data.sql
├── docker-compose.yml
├── .env.example
└── DOCKER.md (this file)
```

## 🔧 Common Commands

### Starting and Stopping

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

### Building

```bash
# Rebuild all images
docker-compose build

# Rebuild specific service
docker-compose build api

# Rebuild without cache
docker-compose build --no-cache
```

### Logs and Debugging

```bash
# View logs for all services
docker-compose logs -f

# View logs for specific service
docker-compose logs -f bot

# View last 100 lines
docker-compose logs --tail=100 api

# Execute command in running container
docker-compose exec api sh
docker-compose exec mysql mysql -u root -p
```

### Database Access

```bash
# Access MySQL database
docker-compose exec mysql mysql -u root -p
# Password: value of MYSQL_ROOT_PASSWORD from .env

# Access MSSQL database
docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YOUR_SA_PASSWORD'
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

