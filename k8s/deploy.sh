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
echo "🔐 Checking for required secrets..."

# Check if secrets exist
SECRETS_MISSING=false
REQUIRED_SECRETS=("discord-secrets" "jwt-secrets" "db-secrets")

for secret in "${REQUIRED_SECRETS[@]}"; do
    if ! kubectl get secret "$secret" -n pogo-system &> /dev/null; then
        echo "  ❌ Missing secret: $secret"
        SECRETS_MISSING=true
    else
        echo "  ✅ Found secret: $secret"
    fi
done

if [ "$SECRETS_MISSING" = true ]; then
    echo ""
    echo "⚠️  Required secrets are missing!"
    echo ""
    echo "Please create secrets first by running:"
    echo "  ./k8s/create-secrets.sh"
    echo ""
    echo "Or for auto-generation:"
    echo "  ./k8s/create-secrets.sh --auto"
    echo ""
    exit 1
fi

echo ""
echo "📦 Applying Kubernetes manifests..."

# Apply base resources
echo "  → Creating namespace..."
kubectl apply -f k8s/base/namespace.yaml

# Wait for namespace to be ready
echo "  → Waiting for namespace to be ready..."
sleep 3

# Apply ingress after namespace is ready
echo "  → Creating ingress..."
kubectl apply -f k8s/base/ingress.yaml

# Apply database
echo "  → Deploying CockroachDB..."
kubectl apply -f k8s/databases/ || {
    echo "❌ Failed to apply database resources"
    exit 1
}

# Wait for CockroachDB to be ready
echo "  → Waiting for CockroachDB to be ready..."
kubectl wait --for=condition=ready pod -l app=cockroachdb -n pogo-system --timeout=300s

# Apply microservices
echo "  → Deploying microservices..."
kubectl apply -f k8s/microservices/

# Apply BFFs
echo "  → Deploying BFF gateways..."
kubectl apply -f k8s/bffs/

# Apply gateways
echo "  → Deploying gateway services..."
kubectl apply -f k8s/gateways/

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
echo "🚀 Starting automatic port forwarding..."
if ./k8s/port-forward.sh start; then
    echo ""
    echo "📋 Access your applications:"
    echo "  Mobile App:      http://$(minikube ip):30000"
    echo "  Swagger Gateway: http://localhost:10000"
    echo "  Grafana:         http://localhost:10001"
    echo "  Prometheus:      http://localhost:10002"
    echo ""
    echo "💡 Port forwarding is running in the background"
    echo "💡 To stop port forwarding: ./k8s/port-forward.sh stop"
    echo "💡 To check status: ./k8s/port-forward.sh status"
else
    echo ""
    echo "⚠️  Port forwarding failed to start automatically"
    echo "📋 Access your applications via NodePort:"
    echo "  Mobile App:      http://$(minikube ip):30000"
    echo "  Grafana:         http://$(minikube ip):30030"
    echo "  Prometheus:      http://$(minikube ip):30090"
    echo ""
    echo "💡 You can manually start port forwarding with: ./k8s/port-forward.sh start"
fi
echo ""
echo "🔍 Check pod status:"
echo "  kubectl get pods -n pogo-system"
echo ""
echo "📊 Check services:"
echo "  kubectl get services -n pogo-system"
