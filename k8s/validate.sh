#!/bin/bash

# POGO Community - Validate Kubernetes Deployment
# This script validates that all components are working correctly

set -e

echo "🔍 Validating POGO Community Kubernetes deployment..."
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check if minikube is running
if ! minikube status > /dev/null 2>&1; then
    echo -e "${RED}❌ Minikube is not running${NC}"
    exit 1
fi

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo -e "${RED}❌ kubectl is not installed${NC}"
    exit 1
fi

echo -e "${CYAN}📋 Checking namespace...${NC}"
if kubectl get namespace pogo-system > /dev/null 2>&1; then
    echo -e "${GREEN}✅ Namespace pogo-system exists${NC}"
else
    echo -e "${RED}❌ Namespace pogo-system not found${NC}"
    exit 1
fi

echo ""
echo -e "${CYAN}📋 Checking pods status...${NC}"
PODS_READY=$(kubectl get pods -n pogo-system --field-selector=status.phase=Running --no-headers | wc -l)
TOTAL_PODS=$(kubectl get pods -n pogo-system --no-headers | wc -l)

if [ "$PODS_READY" -eq "$TOTAL_PODS" ] && [ "$TOTAL_PODS" -gt 0 ]; then
    echo -e "${GREEN}✅ All $TOTAL_PODS pods are running${NC}"
else
    echo -e "${YELLOW}⚠️  $PODS_READY/$TOTAL_PODS pods are running${NC}"
    echo "Pods status:"
    kubectl get pods -n pogo-system
fi

echo ""
echo -e "${CYAN}📋 Checking services...${NC}"
SERVICES=$(kubectl get services -n pogo-system --no-headers | wc -l)
echo -e "${GREEN}✅ $SERVICES services found${NC}"

echo ""
echo -e "${CYAN}📋 Checking CockroachDB cluster...${NC}"
COCKROACH_PODS=$(kubectl get pods -l app=cockroachdb -n pogo-system --no-headers | wc -l)
if [ "$COCKROACH_PODS" -eq 3 ]; then
    echo -e "${GREEN}✅ CockroachDB cluster has 3 replicas${NC}"
else
    echo -e "${YELLOW}⚠️  CockroachDB has $COCKROACH_PODS replicas (expected 3)${NC}"
fi

echo ""
echo -e "${CYAN}📋 Checking microservices...${NC}"
MICROSERVICES=("account-service" "player-service" "location-service" "gym-service" "raid-service")
for service in "${MICROSERVICES[@]}"; do
    if kubectl get deployment $service -n pogo-system > /dev/null 2>&1; then
        READY=$(kubectl get deployment $service -n pogo-system -o jsonpath='{.status.readyReplicas}')
        DESIRED=$(kubectl get deployment $service -n pogo-system -o jsonpath='{.spec.replicas}')
        if [ "$READY" -eq "$DESIRED" ]; then
            echo -e "${GREEN}✅ $service: $READY/$DESIRED replicas ready${NC}"
        else
            echo -e "${YELLOW}⚠️  $service: $READY/$DESIRED replicas ready${NC}"
        fi
    else
        echo -e "${RED}❌ $service deployment not found${NC}"
    fi
done

echo ""
echo -e "${CYAN}📋 Checking BFF services...${NC}"
BFFS=("bot-bff" "app-bff")
for bff in "${BFFS[@]}"; do
    if kubectl get deployment $bff -n pogo-system > /dev/null 2>&1; then
        READY=$(kubectl get deployment $bff -n pogo-system -o jsonpath='{.status.readyReplicas}')
        DESIRED=$(kubectl get deployment $bff -n pogo-system -o jsonpath='{.spec.replicas}')
        if [ "$READY" -eq "$DESIRED" ]; then
            echo -e "${GREEN}✅ $bff: $READY/$DESIRED replicas ready${NC}"
        else
            echo -e "${YELLOW}⚠️  $bff: $READY/$DESIRED replicas ready${NC}"
        fi
    else
        echo -e "${RED}❌ $bff deployment not found${NC}"
    fi
done

echo ""
echo -e "${CYAN}📋 Checking frontend applications...${NC}"
APPS=("pogo-bot" "pogo-app")
for app in "${APPS[@]}"; do
    if kubectl get deployment $app -n pogo-system > /dev/null 2>&1; then
        READY=$(kubectl get deployment $app -n pogo-system -o jsonpath='{.status.readyReplicas}')
        DESIRED=$(kubectl get deployment $app -n pogo-system -o jsonpath='{.spec.replicas}')
        if [ "$READY" -eq "$DESIRED" ]; then
            echo -e "${GREEN}✅ $app: $READY/$DESIRED replicas ready${NC}"
        else
            echo -e "${YELLOW}⚠️  $app: $READY/$DESIRED replicas ready${NC}"
        fi
    else
        echo -e "${RED}❌ $app deployment not found${NC}"
    fi
done

echo ""
echo -e "${CYAN}📋 Checking monitoring stack...${NC}"
MONITORING=("prometheus" "grafana")
for monitor in "${MONITORING[@]}"; do
    if kubectl get deployment $monitor -n pogo-system > /dev/null 2>&1; then
        READY=$(kubectl get deployment $monitor -n pogo-system -o jsonpath='{.status.readyReplicas}')
        DESIRED=$(kubectl get deployment $monitor -n pogo-system -o jsonpath='{.spec.replicas}')
        if [ "$READY" -eq "$DESIRED" ]; then
            echo -e "${GREEN}✅ $monitor: $READY/$DESIRED replicas ready${NC}"
        else
            echo -e "${YELLOW}⚠️  $monitor: $READY/$DESIRED replicas ready${NC}"
        fi
    else
        echo -e "${RED}❌ $monitor deployment not found${NC}"
    fi
done

echo ""
echo -e "${CYAN}📋 Checking persistent volumes...${NC}"
PVS=$(kubectl get pvc -n pogo-system --no-headers | wc -l)
if [ "$PVS" -gt 0 ]; then
    echo -e "${GREEN}✅ $PVS persistent volume claims found${NC}"
else
    echo -e "${YELLOW}⚠️  No persistent volume claims found${NC}"
fi

echo ""
echo -e "${CYAN}📋 Checking ingress...${NC}"
if kubectl get ingress pogo-ingress -n pogo-system > /dev/null 2>&1; then
    echo -e "${GREEN}✅ Ingress pogo-ingress found${NC}"
else
    echo -e "${YELLOW}⚠️  Ingress not found${NC}"
fi

echo ""
echo -e "${CYAN}📋 Testing service connectivity...${NC}"
# Test internal service connectivity
if kubectl run test-pod --image=busybox --rm -i --restart=Never -- nslookup account-service.pogo-system.svc.cluster.local > /dev/null 2>&1; then
    echo -e "${GREEN}✅ Internal DNS resolution working${NC}"
else
    echo -e "${YELLOW}⚠️  Internal DNS resolution test failed${NC}"
fi

echo ""
echo -e "${CYAN}📋 Getting Minikube IP and access URLs...${NC}"
MINIKUBE_IP=$(minikube ip)
echo -e "${GREEN}✅ Minikube IP: $MINIKUBE_IP${NC}"
echo ""
echo -e "${YELLOW}🌐 Access URLs:${NC}"
echo -e "  Mobile App:    http://$MINIKUBE_IP:30000"
echo -e "  Grafana:       http://$MINIKUBE_IP:30030"
echo -e "  Prometheus:    http://$MINIKUBE_IP:30090"
echo ""

echo -e "${CYAN}📋 Summary${NC}"
echo "=================="
echo -e "Total Pods: $TOTAL_PODS"
echo -e "Running Pods: $PODS_READY"
echo -e "Services: $SERVICES"
echo -e "CockroachDB Replicas: $COCKROACH_PODS"
echo -e "Persistent Volumes: $PVS"
echo ""

if [ "$PODS_READY" -eq "$TOTAL_PODS" ] && [ "$TOTAL_PODS" -gt 0 ]; then
    echo -e "${GREEN}🎉 All components are running successfully!${NC}"
    echo -e "${GREEN}✅ POGO Community platform is ready!${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️  Some components may need attention${NC}"
    echo -e "${YELLOW}Check the status above and run 'kubectl get pods -n pogo-system' for details${NC}"
    exit 1
fi
