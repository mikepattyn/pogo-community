#!/bin/bash
# Remove only the microservices deployments

echo "🗑️  Removing microservice deployments..."

kubectl delete deployment account-service -n pogo-system --ignore-not-found=true
kubectl delete deployment player-service -n pogo-system --ignore-not-found=true
kubectl delete deployment location-service -n pogo-system --ignore-not-found=true
kubectl delete deployment gym-service -n pogo-system --ignore-not-found=true
kubectl delete deployment raid-service -n pogo-system --ignore-not-found=true

echo "⏳ Waiting for pods to be removed..."
kubectl wait --for=delete pod -l app=account-service -n pogo-system --timeout=60s || true
kubectl wait --for=delete pod -l app=player-service -n pogo-system --timeout=60s || true
kubectl wait --for=delete pod -l app=location-service -n pogo-system --timeout=60s || true
kubectl wait --for=delete pod -l app=gym-service -n pogo-system --timeout=60s || true
kubectl wait --for=delete pod -l app=raid-service -n pogo-system --timeout=60s || true

echo "✅ Microservice pods removed!"
