#!/bin/bash
# Deploy all gateways

echo "🚀 Deploying gateways..."

kubectl apply -f k8s/gateways/swagger-gateway-deployment.yaml

echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available deployment/swagger-gateway -n pogo-system --timeout=300s

echo "✅ Gateways deployed!"

echo "🔍 Pod status:"
kubectl get pods -n pogo-system -l tier=gateway
































