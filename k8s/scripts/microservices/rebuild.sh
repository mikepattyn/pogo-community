#!/bin/bash
# Rebuild alleen de 5 microservices

echo "🔧 Configureren Docker naar Minikube..."
eval $(minikube docker-env)

echo "📦 Rebuilding microservice images..."

echo "  → Building Account Service..."
docker build -t pogo/account-service:latest \
  -f apps/backend/microservices/Account.Service/Dockerfile .

echo "  → Building Player Service..."
docker build -t pogo/player-service:latest \
  -f apps/backend/microservices/Player.Service/Dockerfile .

echo "  → Building Location Service..."
docker build -t pogo/location-service:latest \
  -f apps/backend/microservices/Location.Service/Dockerfile .

echo "  → Building Gym Service..."
docker build -t pogo/gym-service:latest \
  -f apps/backend/microservices/Gym.Service/Dockerfile .

echo "  → Building Raid Service..."
docker build -t pogo/raid-service:latest \
  -f apps/backend/microservices/Raid.Service/Dockerfile .

echo "✅ Alle microservice images herbouwd!"
