#!/bin/bash
# Remove only the BFF deployments

echo "🗑️  Removing BFF deployments..."

kubectl delete deployment app-bff -n pogo-system --ignore-not-found=true
kubectl delete deployment bot-bff -n pogo-system --ignore-not-found=true

echo "⏳ Waiting for pods to be removed..."
kubectl wait --for=delete pod -l app=app-bff -n pogo-system --timeout=60s || true
kubectl wait --for=delete pod -l app=bot-bff -n pogo-system --timeout=60s || true

echo "✅ BFF pods removed!"
































