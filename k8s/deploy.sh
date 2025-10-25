#!/bin/bash

# POGO Community - Deploy to Kubernetes
# This script deploys the entire POGO platform to Minikube

set -e

echo "🚀 Deploying POGO Community to Kubernetes..."
echo ""

# Check if minikube is running
if ! minikube status > /dev/null 2>&1; then
    echo "❌ Minikube is not running. Please start it with: minikube start"
    exit 1
fi

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo "❌ kubectl is not installed or not in PATH"
    exit 1
fi

echo "🔧 Enabling Minikube addons..."
minikube addons enable ingress

echo ""
echo "📦 Applying Kubernetes manifests..."

# Apply base resources
echo "  → Creating namespace and base resources..."
kubectl apply -f k8s/base/

# Apply database
echo "  → Deploying CockroachDB..."
kubectl apply -f k8s/databases/

# Wait for CockroachDB to be ready
echo "  → Waiting for CockroachDB to be ready..."
kubectl wait --for=condition=ready pod -l app=cockroachdb -n pogo-system --timeout=300s

# Apply microservices
echo "  → Deploying microservices..."
kubectl apply -f k8s/microservices/

# Apply BFFs
echo "  → Deploying BFF gateways..."
kubectl apply -f k8s/bffs/

# Apply frontend apps
echo "  → Deploying frontend applications..."
kubectl apply -f k8s/apps/

# Apply monitoring
echo "  → Deploying monitoring stack..."
kubectl apply -f k8s/monitoring/

echo ""
echo "⏳ Waiting for all deployments to be ready..."
kubectl wait --for=condition=available deployment --all -n pogo-system --timeout=300s

echo ""
echo "✅ Deployment complete!"
echo ""
echo "📋 Access your applications:"
echo "  Mobile App:    http://$(minikube ip):30000"
echo "  Grafana:       http://$(minikube ip):30030"
echo "  Prometheus:    http://$(minikube ip):30090"
echo ""
echo "🔍 Check pod status:"
echo "  kubectl get pods -n pogo-system"
echo ""
echo "📊 Check services:"
echo "  kubectl get services -n pogo-system"
