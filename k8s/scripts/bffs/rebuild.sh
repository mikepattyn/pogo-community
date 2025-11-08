#!/bin/bash
# Rebuild all BFF images

echo "🔧 Configuring Docker to use Minikube..."
eval $(minikube docker-env)

echo "📦 Rebuilding BFF images..."

echo "  → Building App BFF..."
docker build -t pogo/app-bff:latest \
  -f apps/backend/bffs/App.BFF/Dockerfile .

echo "  → Building Bot BFF..."
docker build -t pogo/bot-bff:latest \
  -f apps/backend/bffs/Bot.BFF/Dockerfile .

echo "✅ All BFF images rebuilt!"
































