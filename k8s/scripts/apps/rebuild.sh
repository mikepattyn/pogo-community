#!/bin/bash
# Rebuild all app images

echo "🔧 Configuring Docker to use Minikube..."
eval $(minikube docker-env)

echo "📦 Rebuilding app images..."

echo "  → Building Bot App..."
docker build -t pogo/bot:latest \
  -f bot.Dockerfile .

echo "✅ All app images rebuilt!"
































