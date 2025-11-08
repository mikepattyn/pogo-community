#!/bin/bash
# Deploy all apps

echo "🚀 Deploying apps..."

kubectl apply -f k8s/apps/bot-deployment.yaml

echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available deployment/pogo-bot -n pogo-system --timeout=300s

echo "✅ Apps deployed!"

echo "🔍 Pod status:"
kubectl get pods -n pogo-system -l tier=frontend
































