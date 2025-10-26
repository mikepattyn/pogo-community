#!/bin/bash

# POGO Community - Build and Load Docker Images to Minikube
# This script builds all Docker images and loads them into Minikube's Docker daemon

set -e

echo "🚀 Building and loading POGO Community images into Minikube..."
echo ""

# Check if minikube is running
if ! minikube status > /dev/null 2>&1; then
    echo "❌ Minikube is not running. Please start it with: minikube start"
    exit 1
fi

# Configure Docker to use Minikube's Docker daemon
echo "🔧 Configuring Docker to use Minikube's daemon..."
eval $(minikube docker-env)

# Build images
echo ""
echo "📦 Building microservices images..."

echo "  → Building Account Service..."
docker build -t pogo/account-service:latest -f apps/backend/microservices/Account.Service/Dockerfile .

echo "  → Building Player Service..."
docker build -t pogo/player-service:latest -f apps/backend/microservices/Player.Service/Dockerfile .

echo "  → Building Location Service..."
docker build -t pogo/location-service:latest -f apps/backend/microservices/Location.Service/Dockerfile .

echo "  → Building Gym Service..."
docker build -t pogo/gym-service:latest -f apps/backend/microservices/Gym.Service/Dockerfile .

echo "  → Building Raid Service..."
docker build -t pogo/raid-service:latest -f apps/backend/microservices/Raid.Service/Dockerfile .

echo "  → Building OCR Service..."
docker build -t pogo/ocr-service:latest -f apps/backend/microservices/OCR.Service/Dockerfile .


echo ""
echo "📦 Building BFF (Backend for Frontend) images..."

echo "  → Building Bot BFF..."
docker build -t pogo/bot-bff:latest -f apps/backend/bffs/Bot.BFF/Dockerfile .

echo "  → Building App BFF..."
docker build -t pogo/app-bff:latest -f apps/backend/bffs/App.BFF/Dockerfile .

echo ""
echo "📦 Building gateway images..."

echo "  → Building Swagger Gateway..."
docker build -t pogo/swagger-gateway:latest -f apps/backend/gateways/Swagger.Gateway/Dockerfile .

echo ""
echo "📦 Building frontend application images..."

echo "  → Building Discord Bot..."
docker build -t pogo/bot:latest -f apps/frontend/bot/Dockerfile .

echo "  → Building Mobile App..."
docker build -t pogo/app:latest -f app.Dockerfile .

echo ""
echo "✅ All images built and loaded into Minikube successfully!"
echo ""
echo "📋 Available images:"
docker images | grep "pogo/"

echo ""
echo "💡 To verify images in Minikube, run:"
echo "   eval \$(minikube docker-env)"
echo "   docker images | grep pogo"

