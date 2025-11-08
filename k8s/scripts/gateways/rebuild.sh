#!/bin/bash
# Rebuild all gateway images

echo "🔧 Configuring Docker to use Minikube..."
eval $(minikube docker-env)

echo "📦 Rebuilding gateway images..."

echo "  → Building Swagger Gateway..."
docker build -t pogo/swagger-gateway:latest \
  -f apps/backend/gateways/Swagger.Gateway/Dockerfile .

echo "✅ All gateway images rebuilt!"
































