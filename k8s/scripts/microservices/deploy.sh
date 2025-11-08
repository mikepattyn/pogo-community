#!/bin/bash
# Deploy alleen de microservices

echo "🚀 Deploying microservices..."

kubectl apply -f k8s/microservices/account-service-deployment.yaml
kubectl apply -f k8s/microservices/player-service-deployment.yaml
kubectl apply -f k8s/microservices/location-service-deployment.yaml
kubectl apply -f k8s/microservices/gym-service-deployment.yaml
kubectl apply -f k8s/microservices/raid-service-deployment.yaml

echo "⏳ Wachten tot deployments klaar zijn..."
kubectl wait --for=condition=available deployment/account-service -n pogo-system --timeout=300s
kubectl wait --for=condition=available deployment/player-service -n pogo-system --timeout=300s
kubectl wait --for=condition=available deployment/location-service -n pogo-system --timeout=300s
kubectl wait --for=condition=available deployment/gym-service -n pogo-system --timeout=300s
kubectl wait --for=condition=available deployment/raid-service -n pogo-system --timeout=300s

echo "✅ Microservices gedeployed!"

echo "🔍 Pod status:"
kubectl get pods -n pogo-system -l tier=microservice
