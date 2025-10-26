# Environment Variables Reference

This document provides a comprehensive guide to all environment variables used in the POGO Community project.

## Quick Start

1. For Kubernetes deployment, create secrets using `./k8s/create-secrets.sh`
2. For local development, use Kubernetes secrets or Docker Compose configuration
3. Never commit secrets to git

## Required Environment Variables

### Local Development (Docker Compose)

| Variable            | Description            | Example               | Required For                |
| ------------------- | ---------------------- | --------------------- | --------------------------- |
| `MSSQL_SA_PASSWORD` | SQL Server SA password | `YourStrong@Passw0rd` | All services with databases |
| `DISCORD_BOT_TOKEN` | Discord bot API token  | `MTA...`              | Bot service                 |

### Kubernetes Deployment

#### Secrets (use `kubectl create secret`)

- `discord-secrets`: Contains `DISCORD_BOT_TOKEN`
- `jwt-secrets`: Contains `JWT_SECRET_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_EXPIRY_MINUTES`
- `db-secrets`: Contains `DB_USERNAME`, `DB_PASSWORD`, `MSSQL_SA_PASSWORD`

#### ConfigMaps (defined in `k8s/config/`)

- `common-config`: Contains all shared configuration

## Configuration by Service

### Discord Bot

**Environment Variables:**

- `DISCORD_BOT_TOKEN` - Discord API bot token
- `BOT_BFF_URL` - URL of the Bot BFF service
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - Binding URLs

**Configuration Files:**

- `apps/frontend/bot/appsettings.json` - Default configuration
- `apps/frontend/bot/appsettings.Development.json` - Development overrides

**How to Get Token:**

1. Go to https://discord.com/developers/applications
2. Select your application
3. Navigate to Bot section
4. Click "Reset Token"
5. Copy the token

### Microservices

All microservices share common configuration patterns:

**Common Environment Variables:**

- `ASPNETCORE_ENVIRONMENT` - Environment setting
- `ASPNETCORE_URLS` - Service binding URL (e.g., `http://+:5001`)
- `ConnectionStrings__DefaultConnection` - Database connection string

**Account Service Specific:**

- `Jwt__SecretKey` - JWT signing key (min 32 chars)
- `Jwt__Issuer` - JWT issuer claim
- `Jwt__Audience` - JWT audience claim

**Gym Service Specific:**

- `LocationService__BaseUrl` - Location service URL

**Raid Service Specific:**

- `GymService__BaseUrl` - Gym service URL
- `PlayerService__BaseUrl` - Player service URL

### Kubernetes Configuration

#### ConfigMaps

**common-config**
Located at: `k8s/config/common-config.yaml`

Contains:

- `ASPNETCORE_ENVIRONMENT` - Application environment
- `NODE_ENV` - Node.js environment (for React frontend)
- `DB_HOST`, `DB_PORT`, `DB_SSL_MODE` - Database configuration
- `ACCOUNT_CONNECTION_STRING`, `PLAYER_CONNECTION_STRING`, etc. - Per-service connection strings
- `SERVICE_MESH_ENABLED` - Service mesh toggle (future use)

#### Secrets

Never commit real secret values to git! Use placeholders in YAML files and create actual secrets via kubectl.

**discord-secrets**

```
kubectl create secret generic discord-secrets \
  --from-literal=DISCORD_BOT_TOKEN='your-token-here' \
  --namespace=pogo-system
```

**jwt-secrets**

```
kubectl create secret generic jwt-secrets \
  --from-literal=JWT_SECRET_KEY='$(openssl rand -base64 32)' \
  --from-literal=JWT_ISSUER='pogo-community' \
  --from-literal=JWT_AUDIENCE='pogo-community-users' \
  --from-literal=JWT_EXPIRY_MINUTES='60' \
  --namespace=pogo-system
```

**db-secrets**

```
kubectl create secret generic db-secrets \
  --from-literal=DB_USERNAME='root' \
  --from-literal=DB_PASSWORD='' \
  --from-literal=MSSQL_SA_PASSWORD='your-password' \
  --namespace=pogo-system
```

## Connection Strings

### SQL Server (Local Development)

Format: `Server=HOST,PORT;Database=DBNAME;User Id=USER;Password=PASSWORD;TrustServerCertificate=true;`

### CockroachDB (Kubernetes)

Format: `Host=HOST;Port=PORT;Database=DBNAME;Username=USER;SslMode=MODE`

## Security Best Practices

### DO

- ✅ Use environment variables for all secrets
- ✅ Use `kubectl create secret` for K8s deployments
- ✅ Use Kubernetes ConfigMaps for non-sensitive configuration
- ✅ Rotate secrets regularly
- ✅ Use strong, randomly generated passwords
- ✅ Use key management services in production

### DON'T

- ❌ Commit secrets to git
- ❌ Hardcode passwords in source code
- ❌ Share secrets in plain text
- ❌ Use default passwords in production
- ❌ Store secrets in version control
- ❌ Use `.env` files (use Kubernetes secrets instead)

## Generating Secure Secrets

### Generate JWT Secret Key

```bash
openssl rand -base64 32
```

### Generate Strong Password

```bash
openssl rand -base64 24
```

### Setup for Kubernetes

```bash
# Create secrets for Kubernetes deployment
./k8s/create-secrets.sh --auto

# Verify secrets
kubectl get secrets -n pogo-system
```

## Troubleshooting

### Docker Compose Issues

**Problem:** Database connection fails
**Solution:** Ensure `MSSQL_SA_PASSWORD` is set in your `.env` file

**Problem:** Bot can't connect to Discord
**Solution:** Verify `DISCORD_BOT_TOKEN` is set and valid

### Kubernetes Issues

**Problem:** Pod fails to start with "secret not found"
**Solution:** Create the required secrets first:

```bash
kubectl apply -f k8s/config/discord-secrets.yaml
kubectl apply -f k8s/config/jwt-secrets.yaml
kubectl apply -f k8s/config/db-secrets.yaml
```

**Problem:** Wrong database connection
**Solution:** Check ConfigMap and Secret references in deployment files

## Validation

Run the validation script to check for hardcoded secrets:

```bash
./scripts/check-secrets.sh
```

## Related Documentation

- [Development Setup](../docs/development.md)
- [Kubernetes Deployment](../docs/kubernetes.md)
- [Architecture](../docs/architecture.md)
