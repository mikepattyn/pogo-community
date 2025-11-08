#!/bin/bash
# Remove only the gateway deployments

echo "🗑️  Removing gateway deployments..."

kubectl delete deployment swagger-gateway -n pogo-system --ignore-not-found=true

echo "⏳ Waiting for pods to be removed..."
kubectl wait --for=delete pod -l app=swagger-gateway -n pogo-system --timeout=60s || true

echo "✅ Gateway pods removed!"
































