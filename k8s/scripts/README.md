# 🚀 Kubernetes Management Scripts

Interactive bash scripts for managing Kubernetes deployments of the POGO Community microservices architecture.

## 📋 Overview

This directory contains interactive management scripts organized by service type:

- **Microservices** - Core backend services (Account, Player, Location, Gym, Raid, OCR)
- **BFFs** - Backend for Frontend gateways (App BFF, Bot BFF)
- **Gateways** - API gateways (Swagger Gateway)
- **Apps** - Frontend applications (Bot)

Each category has its own interactive menu script that provides operations for rebuild, remove, deploy, and "run all" workflows.

## 📁 Directory Structure

```
k8s/scripts/
├── microservices.sh        # Interactive menu for microservices
├── microservices/
│   ├── rebuild.sh          # Rebuild all microservice images
│   ├── remove.sh           # Remove all microservice deployments
│   └── deploy.sh           # Deploy all microservices
├── bffs.sh                 # Interactive menu for BFFs
├── bffs/
│   ├── rebuild.sh          # Rebuild all BFF images
│   ├── remove.sh           # Remove all BFF deployments
│   └── deploy.sh           # Deploy all BFFs
├── gateways.sh             # Interactive menu for gateways
├── gateways/
│   ├── rebuild.sh          # Rebuild all gateway images
│   ├── remove.sh           # Remove all gateway deployments
│   └── deploy.sh           # Deploy all gateways
├── apps.sh                 # Interactive menu for apps
└── apps/
    ├── rebuild.sh          # Rebuild all app images
    ├── remove.sh           # Remove all app deployments
    └── deploy.sh           # Deploy all apps
```

## 🎯 Usage

### Interactive Menu Scripts

Each main script provides an interactive menu with the following options:

1. **🔨 Rebuild images** - Rebuild Docker images for selected service(s)
2. **🗑️ Remove deployments** - Remove Kubernetes deployments
3. **🚀 Deploy services** - Deploy services to Kubernetes
4. **🔄 Run all** - Execute rebuild → remove → deploy in sequence
5. **❌ Exit** - Exit the script

#### Example: Microservices

```bash
# Run the interactive menu
./k8s/scripts/microservices.sh

# Or from project root
cd k8s/scripts
./microservices.sh
```

#### Example: BFFs

```bash
./k8s/scripts/bffs.sh
```

#### Example: Gateways

```bash
./k8s/scripts/gateways.sh
```

#### Example: Apps

```bash
./k8s/scripts/apps.sh
```

### Direct Script Execution

You can also run individual scripts directly without the interactive menu:

```bash
# Rebuild all microservices
./k8s/scripts/microservices/rebuild.sh

# Remove all BFF deployments
./k8s/scripts/bffs/remove.sh

# Deploy all gateways
./k8s/scripts/gateways/deploy.sh
```

## 🔧 Prerequisites

Before using these scripts, ensure you have:

1. **Minikube** running:

   ```bash
   minikube status
   ```

2. **Docker** configured to use Minikube's Docker daemon:

   ```bash
   eval $(minikube docker-env)
   ```

   (The scripts automatically configure this)

3. **kubectl** configured to access your Kubernetes cluster

4. **Namespace** `pogo-system` created (scripts assume this namespace exists)

## 📦 Services Managed

### Microservices

- `account-service` - User authentication service
- `player-service` - Player management service
- `location-service` - Location management service
- `gym-service` - Gym management service
- `raid-service` - Raid management service
- `ocr-service` - OCR processing service

**Images**: `pogo/{service-name}:latest`  
**Deployments**: Located in `k8s/microservices/`  
**Source**: `apps/backend/microservices/{Service.Name}/`

### BFFs (Backend for Frontend)

- `app-bff` - API gateway for mobile app
- `bot-bff` - API gateway for Discord bot

**Images**: `pogo/{bff-name}:latest`  
**Deployments**: Located in `k8s/bffs/`  
**Source**: `apps/backend/bffs/{BFF.Name}/`

### Gateways

- `swagger-gateway` - API documentation gateway

**Images**: `pogo/swagger-gateway:latest`  
**Deployments**: Located in `k8s/gateways/`  
**Source**: `apps/backend/gateways/Swagger.Gateway/`

### Apps

- `pogo-bot` - Discord bot application

**Images**: `pogo/bot:latest`  
**Deployments**: Located in `k8s/apps/`  
**Dockerfile**: `bot.Dockerfile` (project root)

## 🔄 Workflow Examples

### Full Redeploy Workflow

To completely redeploy a service (rebuild image, remove old deployment, deploy new):

1. Run the interactive menu script for the service category
2. Select option **4) 🔄 Run all**
3. Choose the specific service or "all"
4. Confirm the action

Example:

```bash
./k8s/scripts/microservices.sh
# Select option 4
# Select service (e.g., account-service)
# Confirm
```

### Rebuild Only

To rebuild an image without redeploying:

1. Run the interactive menu script
2. Select option **1) 🔨 Rebuild images**
3. Choose service or "all"

### Individual Service Management

Each script allows you to:

- Select a specific service for operations
- Select "all" to operate on all services in the category
- Get confirmation prompts before destructive operations

## ⚙️ Script Features

All interactive scripts include:

- ✅ **Color-coded output** - Green for success, yellow for warnings, red for errors
- ✅ **Service selection** - Choose individual services or operate on all
- ✅ **Confirmation prompts** - Safety checks before destructive operations
- ✅ **Error handling** - Graceful error handling and reporting
- ✅ **Progress indicators** - Visual feedback during operations
- ✅ **Auto-configuration** - Automatically configures Docker to use Minikube

## 🛠️ Customization

### Adding a New Service

To add a new service to a category:

1. **Update the main script** (e.g., `microservices.sh`):

   - Add service name to the array (e.g., `MICROSERVICE_NAMES`)
   - Add service directory mapping in `get_service_dir()`
   - Update the selection menu in `show_microservice_selection()`

2. **Update the batch scripts** (e.g., `microservices/rebuild.sh`):
   - Add the build command for the new service
   - Update remove and deploy scripts similarly

### Adding a New Category

To create a new service category:

1. Create the main script (e.g., `newcategory.sh`) based on an existing one
2. Create the subdirectory (e.g., `newcategory/`)
3. Create the three batch scripts (`rebuild.sh`, `remove.sh`, `deploy.sh`)
4. Make scripts executable: `chmod +x newcategory.sh newcategory/*.sh`

## 📝 Notes

- All scripts assume you're running from the project root directory
- Scripts automatically configure Docker to use Minikube's Docker daemon
- Image pull policy is set to `Never` or `IfNotPresent` to use local images
- Deployments use namespace `pogo-system`
- Scripts wait for deployments to be ready with timeouts

## 🐛 Troubleshooting

### Scripts Not Executable

```bash
chmod +x k8s/scripts/*.sh k8s/scripts/*/*.sh
```

### Minikube Docker Not Available

```bash
minikube start
eval $(minikube docker-env)
```

### Deployment Not Found

Ensure the deployment YAML files exist in the corresponding `k8s/{category}/` directories.

### Namespace Not Found

```bash
kubectl create namespace pogo-system
```

## 📚 Related Documentation

- [Kubernetes Deployment Guide](../docs/kubernetes.md)
- [Troubleshooting Guide](../docs/troubleshooting.md)
- [Quick Reference](../docs/quick-reference.md)
