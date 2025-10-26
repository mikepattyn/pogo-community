# POGO Community - Kubernetes Deployment Guide

This guide provides comprehensive documentation for deploying and managing the POGO Community platform on Kubernetes using Minikube.

## 📋 Table of Contents

- [Prerequisites](#prerequisites)
- [Architecture Overview](#architecture-overview)
- [Quick Start](#quick-start)
- [Detailed Deployment](#detailed-deployment)
- [Service Management](#service-management)
- [Monitoring & Observability](#monitoring--observability)
- [Troubleshooting](#troubleshooting)
- [Production Considerations](#production-considerations)

## Prerequisites

### Required Software

- **Minikube** v1.37.0+ - Local Kubernetes cluster
- **kubectl** v1.28+ - Kubernetes command-line tool
- **Docker** v24+ - Container runtime
- **Make** - Build automation tool

### System Requirements

- **Memory**: Minimum 8GB RAM (16GB recommended)
- **CPU**: Minimum 4 cores (8 cores recommended)
- **Storage**: Minimum 20GB free disk space
- **OS**: macOS, Linux, or Windows with WSL2

### Verify Installation

```bash
# Check Minikube version
minikube version

# Check kubectl version
kubectl version --client

# Check Docker version
docker --version

# Check Make version
make --version
```

## Architecture Overview

### System Architecture

```mermaid
graph TB
    subgraph "External Access"
        Discord[Discord API]
        Mobile[Mobile App Users]
    end
    
    subgraph "Kubernetes Cluster (pogo-system namespace)"
        subgraph "Frontend Layer"
            Bot[Discord Bot<br/>Port 2000]
            App[Mobile App<br/>Port 3000<br/>NodePort 30000]
        end
        
        subgraph "API Gateway Layer"
            BotBFF[Bot BFF<br/>Port 6001<br/>Ocelot Gateway]
            AppBFF[App BFF<br/>Port 6002<br/>Ocelot Gateway]
        end
        
        subgraph "Microservices Layer"
            Account[Account Service<br/>Port 5001<br/>Authentication]
            Player[Player Service<br/>Port 5002<br/>User Management]
            Location[Location Service<br/>Port 5003<br/>POI Management]
            Gym[Gym Service<br/>Port 5004<br/>Gym Management]
            Raid[Raid Service<br/>Port 5005<br/>Raid Management]
        end
        
        subgraph "Data Layer"
            CockroachDB[(CockroachDB<br/>Port 26257<br/>PostgreSQL Compatible)]
        end
        
        subgraph "Monitoring Layer"
            Prometheus[Prometheus<br/>Port 9090<br/>Metrics Collection]
            Grafana[Grafana<br/>Port 3000<br/>Dashboards]
        end
    end
    
    %% External connections
    Discord --> Bot
    Mobile --> App
    
    %% Frontend to BFF connections
    Bot --> BotBFF
    App --> AppBFF
    
    %% BFF to Microservices connections
    BotBFF --> Account
    BotBFF --> Player
    BotBFF --> Location
    BotBFF --> Gym
    BotBFF --> Raid
    
    AppBFF --> Account
    AppBFF --> Player
    AppBFF --> Location
    AppBFF --> Gym
    AppBFF --> Raid
    
    %% Microservices to Database connections
    Account --> CockroachDB
    Player --> CockroachDB
    Location --> CockroachDB
    Gym --> CockroachDB
    Raid --> CockroachDB
    
    %% Monitoring connections
    Prometheus --> Account
    Prometheus --> Player
    Prometheus --> Location
    Prometheus --> Gym
    Prometheus --> Raid
    Prometheus --> BotBFF
    Prometheus --> AppBFF
    Grafana --> Prometheus
    
    %% Styling
    classDef frontend fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef gateway fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef microservice fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    classDef database fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef monitoring fill:#fce4ec,stroke:#880e4f,stroke-width:2px
    
    class Bot,App frontend
    class BotBFF,AppBFF gateway
    class Account,Player,Location,Gym,Raid microservice
    class CockroachDB database
    class Prometheus,Grafana monitoring
```

### Component Overview

| Component | Type | Purpose | Port | External Access |
|-----------|------|---------|------|------------------|
| **Frontend Applications** |
| Discord Bot | Deployment | Discord integration | 2000 | Port Forward |
| Mobile App | Deployment | React Native web app | 3000 | NodePort 30000 |
| **API Gateways** |
| Bot BFF | Deployment | Bot API gateway | 6001 | Port Forward |
| App BFF | Deployment | App API gateway | 6002 | Port Forward |
| **Microservices** |
| Account Service | Deployment | Authentication & users | 5001 | Internal |
| Player Service | Deployment | Player management | 5002 | Internal |
| Location Service | Deployment | POI management | 5003 | Internal |
| Gym Service | Deployment | Gym management | 5004 | Internal |
| Raid Service | Deployment | Raid management | 5005 | Internal |
| **Database** |
| CockroachDB | StatefulSet | PostgreSQL database | 26257 | Internal |
| **Monitoring** |
| Prometheus | Deployment | Metrics collection | 9090 | Port Forward |
| Grafana | Deployment | Monitoring dashboards | 3000 | Port Forward |

## Quick Start

### 1. Start Minikube

```bash
# Start Minikube with sufficient resources
minikube start --memory=8192 --cpus=4

# Enable required addons
minikube addons enable ingress
minikube addons enable metrics-server

# Verify Minikube is running
minikube status
```

### 2. Build and Deploy

```bash
# Build all Docker images and load into Minikube
make k8s-build

# Deploy all services to Kubernetes
make k8s-deploy

# Validate deployment
make k8s-validate
```

### 3. Access Applications

```bash
# Get Minikube IP
MINIKUBE_IP=$(minikube ip)
echo "Minikube IP: $MINIKUBE_IP"

# Access applications
echo "Mobile App: http://$MINIKUBE_IP:30000"
echo "Grafana: http://$MINIKUBE_IP:30030"
echo "Prometheus: http://$MINIKUBE_IP:30090"
```

## Detailed Deployment

### Step-by-Step Deployment

#### 1. Environment Setup

```bash
# Clone repository
git clone <repository-url>
cd pogo

# Verify prerequisites
make k8s  # Shows available commands and prerequisites
```

#### 2. Build Images

```bash
# Build all Docker images
make k8s-build

# Verify images are loaded
eval $(minikube docker-env)
docker images | grep pogo
```

#### 3. Deploy Services

```bash
# Deploy all services
make k8s-deploy

# Monitor deployment progress
kubectl get pods -n pogo-system -w
```

#### 4. Verify Deployment

```bash
# Check all components
make k8s-validate

# Check specific services
kubectl get pods -n pogo-system
kubectl get services -n pogo-system
kubectl get ingress -n pogo-system
```

### Manual Deployment Steps

If you prefer manual deployment:

```bash
# 1. Create namespace
kubectl apply -f k8s/base/namespace.yaml

# 2. Deploy database
kubectl apply -f k8s/databases/

# 3. Wait for database to be ready
kubectl wait --for=condition=ready pod -l app=cockroachdb -n pogo-system --timeout=300s

# 4. Deploy microservices
kubectl apply -f k8s/microservices/

# 5. Deploy BFFs
kubectl apply -f k8s/bffs/

# 6. Deploy frontend apps
kubectl apply -f k8s/apps/

# 7. Deploy monitoring
kubectl apply -f k8s/monitoring/

# 8. Deploy ingress
kubectl apply -f k8s/base/ingress.yaml
```

## Service Management

### Available Commands

```bash
# Build and Deploy
make k8s-build          # Build all Docker images
make k8s-deploy         # Deploy to Kubernetes
make k8s-teardown       # Remove all resources

# Monitoring
make k8s-status         # Show pod status
make k8s-logs           # View logs
make k8s-validate       # Validate deployment

# Debugging
make k8s-shell POD=<pod-name>  # Open shell in pod
```

### Service Status

```bash
# Check all pods
kubectl get pods -n pogo-system

# Check specific service
kubectl get pods -l app=account-service -n pogo-system

# Check service endpoints
kubectl get endpoints -n pogo-system

# Check persistent volumes
kubectl get pvc -n pogo-system
```

### Scaling Services

```bash
# Scale microservice
kubectl scale deployment account-service --replicas=3 -n pogo-system

# Scale BFF
kubectl scale deployment bot-bff --replicas=2 -n pogo-system

# Check scaling status
kubectl get pods -l app=account-service -n pogo-system
```

### Rolling Updates

```bash
# Update deployment
kubectl set image deployment/account-service account-service=pogo/account-service:v2.0 -n pogo-system

# Check rollout status
kubectl rollout status deployment/account-service -n pogo-system

# Rollback if needed
kubectl rollout undo deployment/account-service -n pogo-system
```

## Monitoring & Observability

### Health Checks

All services include comprehensive health checks:

- **Readiness Probe**: `/health/ready` - Service is ready to accept traffic
- **Liveness Probe**: `/health/live` - Service is running and healthy
- **Custom Health Checks**: Database connectivity and external service checks

### Prometheus Metrics

```bash
# Access Prometheus
kubectl port-forward service/prometheus 9090:9090 -n pogo-system

# Open in browser
open http://localhost:9090

# Query metrics
curl http://localhost:9090/api/v1/query?query=up
```

### Grafana Dashboards

```bash
# Access Grafana
kubectl port-forward service/grafana 3000:3000 -n pogo-system

# Open in browser
open http://localhost:3000

# Login credentials
# Username: admin
# Password: admin
```

### Log Management

```bash
# View all logs
make k8s-logs

# View specific service logs
kubectl logs -l app=account-service -n pogo-system --tail=100

# Follow logs in real-time
kubectl logs -l app=account-service -n pogo-system -f
```

## Troubleshooting

### Common Issues

#### 1. Pods Not Starting

```bash
# Check pod status
kubectl get pods -n pogo-system

# Check pod events
kubectl describe pod <pod-name> -n pogo-system

# Check pod logs
kubectl logs <pod-name> -n pogo-system
```

#### 2. Database Connection Issues

```bash
# Check CockroachDB status
kubectl get pods -l app=cockroachdb -n pogo-system

# Check database logs
kubectl logs -l app=cockroachdb -n pogo-system

# Test database connectivity
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach sql --insecure
```

#### 3. Service Discovery Issues

```bash
# Check service endpoints
kubectl get endpoints -n pogo-system

# Test DNS resolution
kubectl run test-pod --image=busybox --rm -it --restart=Never -- nslookup account-service.pogo-system.svc.cluster.local

# Check service connectivity
kubectl run test-pod --image=curlimages/curl --rm -it --restart=Never -- curl http://account-service.pogo-system.svc.cluster.local:5001/health/ready
```

#### 4. Resource Constraints

```bash
# Check resource usage
kubectl top pods -n pogo-system
kubectl top nodes

# Check resource limits
kubectl describe pod <pod-name> -n pogo-system | grep -A 5 "Limits:"
```

### Debugging Commands

```bash
# Get detailed pod information
kubectl describe pod <pod-name> -n pogo-system

# Check service configuration
kubectl describe service <service-name> -n pogo-system

# Check ingress configuration
kubectl describe ingress pogo-ingress -n pogo-system

# Check persistent volume claims
kubectl describe pvc <pvc-name> -n pogo-system
```

### Log Analysis

```bash
# Search for errors
kubectl logs -l app=account-service -n pogo-system | grep -i error

# Search for specific patterns
kubectl logs -l app=account-service -n pogo-system | grep "connection"

# Get logs from all containers
kubectl logs -l app=account-service -n pogo-system --all-containers=true
```

## Production Considerations

### Security

- **Secrets Management**: Use Kubernetes secrets for sensitive data
- **Network Policies**: Implement network policies for service isolation
- **RBAC**: Configure role-based access control
- **Image Security**: Use trusted base images and scan for vulnerabilities

### Performance

- **Resource Limits**: Set appropriate CPU and memory limits
- **Horizontal Pod Autoscaling**: Implement HPA for automatic scaling
- **Database Optimization**: Configure CockroachDB for production workloads
- **Caching**: Implement Redis for caching frequently accessed data

### High Availability

- **Multi-Node Cluster**: Deploy on multiple nodes
- **Database Replication**: Configure CockroachDB with multiple replicas
- **Load Balancing**: Use proper load balancing strategies
- **Backup Strategy**: Implement regular database backups

### Monitoring

- **Alerting**: Configure Prometheus alerts for critical metrics
- **Log Aggregation**: Use centralized logging solution
- **Distributed Tracing**: Implement tracing for microservices
- **Performance Monitoring**: Monitor application performance metrics

### Backup and Recovery

```bash
# Backup CockroachDB
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach dump --insecure --database=pogo_account > account_backup.sql

# Restore from backup
kubectl exec -i cockroachdb-0 -n pogo-system -- cockroach sql --insecure < account_backup.sql
```

## Additional Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Minikube Documentation](https://minikube.sigs.k8s.io/docs/)
- [CockroachDB Kubernetes Guide](https://www.cockroachlabs.com/docs/stable/orchestrate-cockroachdb-with-kubernetes.html)
- [Prometheus Kubernetes Integration](https://prometheus.io/docs/prometheus/latest/configuration/configuration/)
- [Grafana Kubernetes Dashboards](https://grafana.com/docs/grafana/latest/datasources/prometheus/)

---

For more information, see the main [README.md](../README.md) or contact the development team.