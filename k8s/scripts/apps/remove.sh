#!/bin/bash
# Remove only the app deployments

echo "🗑️  Removing app deployments..."

kubectl delete deployment pogo-bot -n pogo-system --ignore-not-found=true

echo "⏳ Waiting for pods to be removed..."
kubectl wait --for=delete pod -l app=pogo-bot -n pogo-system --timeout=60s || true

echo "✅ App pods removed!"
































