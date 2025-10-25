#!/bin/bash

# POGO Community - Teardown Kubernetes Deployment
# This script removes all POGO platform resources from Minikube

set -e

echo "🧹 Tearing down POGO Community from Kubernetes..."
echo ""

# Check if minikube is running
if ! minikube status > /dev/null 2>&1; then
    echo "❌ Minikube is not running. Nothing to tear down."
    exit 1
fi

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo "❌ kubectl is not installed or not in PATH"
    exit 1
fi

echo "🗑️  Removing all POGO resources..."

# Remove monitoring stack
echo "  → Removing monitoring stack..."
kubectl delete -f k8s/monitoring/ --ignore-not-found=true

# Remove frontend apps
echo "  → Removing frontend applications..."
kubectl delete -f k8s/apps/ --ignore-not-found=true

# Remove BFFs
echo "  → Removing BFF gateways..."
kubectl delete -f k8s/bffs/ --ignore-not-found=true

# Remove microservices
echo "  → Removing microservices..."
kubectl delete -f k8s/microservices/ --ignore-not-found=true

# Remove database
echo "  → Removing CockroachDB..."
kubectl delete -f k8s/databases/ --ignore-not-found=true

# Remove base resources
echo "  → Removing base resources..."
kubectl delete -f k8s/base/ --ignore-not-found=true

echo ""
echo "⏳ Waiting for all resources to be deleted..."
kubectl wait --for=delete pod --all -n pogo-system --timeout=60s || true

echo ""
echo "✅ Teardown complete!"
echo ""
echo "💡 To completely clean up, you can also:"
echo "  minikube stop"
echo "  minikube delete"
