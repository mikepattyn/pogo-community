# 🚀 POGO Community - Deployment Guide

## Overview

This guide covers deploying the POGO Community microservices architecture to production environments. The system is designed to be containerized and can be deployed using Docker Compose or Kubernetes.

## 🏗️ Architecture Overview

The production deployment includes:
- **5 Microservices** (.NET 10)
- **2 BFF Services** (API Gateways)
- **5 SQL Server Databases** (one per microservice)
- **2 Client Applications** (Discord Bot, Mobile App)

## 📋 Prerequisites

### System Requirements
- **CPU:** 8+ cores (2 cores per service minimum)
- **RAM:** 16GB+ (2GB per service minimum)
- **Storage:** 100GB+ SSD (20GB per database minimum)
- **Network:** 1Gbps+ bandwidth

### Software Requirements
- **Docker** 20.10+
- **Docker Compose** 2.0+
- **.NET 10 Runtime** (for local development)
- **Node.js 18+** (for client applications)

### External Dependencies
- **SQL Server 2022** (or Azure SQL Database)
- **SSL Certificate** (for HTTPS)
- **Domain Name** (for production URLs)

## 🔧 Environment Configuration

### Environment Variables

Create a `.env` file with the following variables:

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
BOT_BFF_URL=http://bot-bff:6001

# Google Cloud Configuration (Optional)
GOOGLE_CLOUD_PROJECT_ID=your-project-id
GOOGLE_APPLICATION_CREDENTIALS=/path/to/service-account.json

# Production URLs
BOT_BFF_URL=https://api.pogo-community.com/bot
APP_BFF_URL=https://api.pogo-community.com/app
```

### Database Connection Strings

Each microservice uses its own database:

```bash
# Account Service
ConnectionStrings__DefaultConnection=Server=account-db,1433;Database=AccountDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;

# Player Service
ConnectionStrings__DefaultConnection=Server=player-db,1433;Database=PlayerDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;

# Location Service
ConnectionStrings__DefaultConnection=Server=location-db,1433;Database=LocationDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;

# Gym Service
ConnectionStrings__DefaultConnection=Server=gym-db,1433;Database=GymDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;

# Raid Service
ConnectionStrings__DefaultConnection=Server=raid-db,1433;Database=RaidDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;
```

## 🐳 Docker Deployment

### 1. Production Docker Compose

Create a `docker-compose.prod.yml` file:

```yaml
version: '3.8'

services:
  # Microservice Databases
  account-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: account-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${MSSQL_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - account_db_data:/var/opt/mssql
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  player-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: player-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${MSSQL_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - player_db_data:/var/opt/mssql
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  location-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: location-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${MSSQL_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - location_db_data:/var/opt/mssql
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  gym-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: gym-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${MSSQL_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - gym_db_data:/var/opt/mssql
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  raid-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: raid-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${MSSQL_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - raid_db_data:/var/opt/mssql
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  # Microservices
  account-service:
    build:
      context: .
      dockerfile: apps/backend/microservices/Account.Service/Dockerfile
    container_name: account-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5001
      - ConnectionStrings__DefaultConnection=Server=account-db,1433;Database=AccountDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=true;
    depends_on:
      account-db:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5001/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  player-service:
    build:
      context: .
      dockerfile: apps/backend/microservices/Player.Service/Dockerfile
    container_name: player-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5002
      - ConnectionStrings__DefaultConnection=Server=player-db,1433;Database=PlayerDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=true;
    depends_on:
      player-db:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5002/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  location-service:
    build:
      context: .
      dockerfile: apps/backend/microservices/Location.Service/Dockerfile
    container_name: location-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5003
      - ConnectionStrings__DefaultConnection=Server=location-db,1433;Database=LocationDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=true;
    depends_on:
      location-db:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5003/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  gym-service:
    build:
      context: .
      dockerfile: apps/backend/microservices/Gym.Service/Dockerfile
    container_name: gym-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5004
      - ConnectionStrings__DefaultConnection=Server=gym-db,1433;Database=GymDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=true;
      - LocationService__BaseUrl=http://location-service:5003
    depends_on:
      gym-db:
        condition: service_healthy
      location-service:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5004/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  raid-service:
    build:
      context: .
      dockerfile: apps/backend/microservices/Raid.Service/Dockerfile
    container_name: raid-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5005
      - ConnectionStrings__DefaultConnection=Server=raid-db,1433;Database=RaidDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=true;
      - GymService__BaseUrl=http://gym-service:5004
      - PlayerService__BaseUrl=http://player-service:5002
    depends_on:
      raid-db:
        condition: service_healthy
      gym-service:
        condition: service_healthy
      player-service:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5005/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  # BFF Services
  bot-bff:
    build:
      context: .
      dockerfile: apps/backend/bffs/Bot.BFF/Dockerfile
    container_name: bot-bff
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:6001
    depends_on:
      account-service:
        condition: service_healthy
      player-service:
        condition: service_healthy
      location-service:
        condition: service_healthy
      gym-service:
        condition: service_healthy
      raid-service:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:6001/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  app-bff:
    build:
      context: .
      dockerfile: apps/backend/bffs/App.BFF/Dockerfile
    container_name: app-bff
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:6002
    depends_on:
      account-service:
        condition: service_healthy
      player-service:
        condition: service_healthy
      location-service:
        condition: service_healthy
      gym-service:
        condition: service_healthy
      raid-service:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:6002/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  # Client Applications
  bot:
    build:
      context: .
      dockerfile: bot.Dockerfile
    container_name: pogo-bot
    environment:
      - BOT_TOKEN=${DISCORD_BOT_TOKEN}
      - BOT_BFF_URL=http://bot-bff:6001
      - NODE_ENV=production
    depends_on:
      bot-bff:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped

  app:
    build:
      context: .
      dockerfile: app.Dockerfile
    container_name: pogo-app
    environment:
      - REACT_APP_API_URL=http://app-bff:6002
      - NODE_ENV=production
    depends_on:
      app-bff:
        condition: service_healthy
    networks:
      - pogo-network
    restart: unless-stopped

volumes:
  account_db_data:
    name: pogo-account-db-data
  player_db_data:
    name: pogo-player-db-data
  location_db_data:
    name: pogo-location-db-data
  gym_db_data:
    name: pogo-gym-db-data
  raid_db_data:
    name: pogo-raid-db-data

networks:
  pogo-network:
    name: pogo-network
    driver: bridge
```

### 2. Deploy with Docker Compose

```bash
# Build and start all services
docker-compose -f docker-compose.prod.yml up -d --build

# Check status
docker-compose -f docker-compose.prod.yml ps

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Stop services
docker-compose -f docker-compose.prod.yml down
```

## ☸️ Kubernetes Deployment

### 1. Namespace

```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: pogo-community
```

### 2. ConfigMap

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: pogo-config
  namespace: pogo-community
data:
  MSSQL_SA_PASSWORD: "YourStrong@Passw0rd123"
  JWT_KEY: "your-super-secret-jwt-key-here-min-32-chars"
  ASPNETCORE_ENVIRONMENT: "Production"
```

### 3. Secret

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: pogo-secrets
  namespace: pogo-community
type: Opaque
data:
  discord-bot-token: <base64-encoded-token>
  google-credentials: <base64-encoded-json>
```

### 4. Database Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: account-db
  namespace: pogo-community
spec:
  replicas: 1
  selector:
    matchLabels:
      app: account-db
  template:
    metadata:
      labels:
        app: account-db
    spec:
      containers:
      - name: mssql
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
        - name: ACCEPT_EULA
          value: "Y"
        - name: SA_PASSWORD
          valueFrom:
            configMapKeyRef:
              name: pogo-config
              key: MSSQL_SA_PASSWORD
        - name: MSSQL_PID
          value: "Standard"
        ports:
        - containerPort: 1433
        volumeMounts:
        - name: mssql-data
          mountPath: /var/opt/mssql
        resources:
          requests:
            memory: "2Gi"
            cpu: "500m"
          limits:
            memory: "4Gi"
            cpu: "1000m"
      volumes:
      - name: mssql-data
        persistentVolumeClaim:
          claimName: account-db-pvc
---
apiVersion: v1
kind: Service
metadata:
  name: account-db
  namespace: pogo-community
spec:
  selector:
    app: account-db
  ports:
  - port: 1433
    targetPort: 1433
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: account-db-pvc
  namespace: pogo-community
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 20Gi
```

### 5. Microservice Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: account-service
  namespace: pogo-community
spec:
  replicas: 2
  selector:
    matchLabels:
      app: account-service
  template:
    metadata:
      labels:
        app: account-service
    spec:
      containers:
      - name: account-service
        image: pogo-account-service:latest
        ports:
        - containerPort: 5001
        env:
        - name: ASPNETCORE_ENVIRONMENT
          valueFrom:
            configMapKeyRef:
              name: pogo-config
              key: ASPNETCORE_ENVIRONMENT
        - name: ConnectionStrings__DefaultConnection
          value: "Server=account-db,1433;Database=AccountDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;"
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 5001
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 5001
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: account-service
  namespace: pogo-community
spec:
  selector:
    app: account-service
  ports:
  - port: 5001
    targetPort: 5001
```

## 🔒 SSL/TLS Configuration

### 1. Nginx Reverse Proxy

```nginx
upstream bot-bff {
    server bot-bff:6001;
}

upstream app-bff {
    server app-bff:6002;
}

server {
    listen 443 ssl http2;
    server_name api.pogo-community.com;

    ssl_certificate /etc/ssl/certs/pogo-community.crt;
    ssl_certificate_key /etc/ssl/private/pogo-community.key;

    location /bot/ {
        proxy_pass http://bot-bff/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /app/ {
        proxy_pass http://app-bff/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 2. Let's Encrypt SSL

```bash
# Install certbot
sudo apt-get install certbot

# Generate SSL certificate
sudo certbot certonly --standalone -d api.pogo-community.com

# Auto-renewal
sudo crontab -e
# Add: 0 12 * * * /usr/bin/certbot renew --quiet
```

## 📊 Monitoring and Logging

### 1. Health Checks

All services expose health check endpoints:

```bash
# Check service health
curl http://localhost:5001/health
curl http://localhost:6001/health

# Check readiness
curl http://localhost:5001/health/ready
curl http://localhost:6001/health/ready
```

### 2. Logging

Configure centralized logging with ELK Stack or similar:

```yaml
# Logstash configuration
input {
  beats {
    port => 5044
  }
}

filter {
  if [fields][service] == "account-service" {
    mutate {
      add_field => { "service_name" => "account-service" }
    }
  }
}

output {
  elasticsearch {
    hosts => ["elasticsearch:9200"]
    index => "pogo-community-%{+YYYY.MM.dd}"
  }
}
```

### 3. Metrics

Use Prometheus and Grafana for monitoring:

```yaml
# Prometheus configuration
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'pogo-community'
    static_configs:
      - targets: ['account-service:5001', 'player-service:5002', 'location-service:5003', 'gym-service:5004', 'raid-service:5005', 'bot-bff:6001', 'app-bff:6002']
```

## 🔄 Database Migrations

### 1. Run Migrations

```bash
# For each microservice
dotnet ef database update --project apps/backend/microservices/Account.Service
dotnet ef database update --project apps/backend/microservices/Player.Service
dotnet ef database update --project apps/backend/microservices/Location.Service
dotnet ef database update --project apps/backend/microservices/Gym.Service
dotnet ef database update --project apps/backend/microservices/Raid.Service
```

### 2. Migration Script

```bash
#!/bin/bash
# migrate.sh

echo "Running database migrations..."

services=("Account.Service" "Player.Service" "Location.Service" "Gym.Service" "Raid.Service")

for service in "${services[@]}"; do
    echo "Migrating $service..."
    dotnet ef database update --project "apps/backend/microservices/$service"
    if [ $? -eq 0 ]; then
        echo "✅ $service migrated successfully"
    else
        echo "❌ $service migration failed"
        exit 1
    fi
done

echo "🎉 All migrations completed successfully!"
```

## 🚀 Deployment Checklist

### Pre-deployment
- [ ] Environment variables configured
- [ ] SSL certificates obtained
- [ ] Database servers provisioned
- [ ] Docker images built and tested
- [ ] Health checks implemented
- [ ] Monitoring configured

### Deployment
- [ ] Deploy databases first
- [ ] Run database migrations
- [ ] Deploy microservices
- [ ] Deploy BFF services
- [ ] Deploy client applications
- [ ] Configure load balancer
- [ ] Test all endpoints

### Post-deployment
- [ ] Verify all services are healthy
- [ ] Test client applications
- [ ] Monitor logs and metrics
- [ ] Set up alerting
- [ ] Document deployment

## 🔧 Troubleshooting

### Common Issues

1. **Database Connection Issues**
   ```bash
   # Check database connectivity
   docker exec -it account-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT 1"
   ```

2. **Service Health Issues**
   ```bash
   # Check service logs
   docker logs account-service
   
   # Check health endpoint
   curl http://localhost:5001/health
   ```

3. **Network Issues**
   ```bash
   # Check network connectivity
   docker network ls
   docker network inspect pogo-network
   ```

### Performance Tuning

1. **Database Optimization**
   - Configure connection pooling
   - Set appropriate memory limits
   - Enable query optimization

2. **Service Optimization**
   - Configure appropriate resource limits
   - Enable response caching
   - Optimize database queries

3. **Load Balancing**
   - Configure multiple service instances
   - Use sticky sessions if needed
   - Monitor load distribution

---

This deployment guide provides a comprehensive approach to deploying the POGO Community microservices architecture in production environments! 🚀
