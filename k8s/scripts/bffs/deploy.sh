#!/bin/bash
# Deploy all BFFs

echo "🚀 Deploying BFFs..."

kubectl apply -f k8s/bffs/app-bff-deployment.yaml
kubectl apply -f k8s/bffs/bot-bff-deployment.yaml

echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available deployment/app-bff -n pogo-system --timeout=300s
kubectl wait --for=condition=available deployment/bot-bff -n pogo-system --timeout=300s

echo "✅ BFFs deployed!"

echo "🔍 Pod status:"
kubectl get pods -n pogo-system -l tier=gateway
































