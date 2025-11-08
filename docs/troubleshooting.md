# POGO Community - Troubleshooting Guide

This guide helps you diagnose and resolve common issues when deploying and running the POGO Community platform on Kubernetes.

## 📋 Table of Contents

- [Prerequisites Check](#prerequisites-check)
- [Common Deployment Issues](#common-deployment-issues)
- [Service-Specific Issues](#service-specific-issues)
- [Database Issues](#database-issues)
- [Monitoring Issues](#monitoring-issues)
- [Network Issues](#network-issues)
- [Resource Issues](#resource-issues)
- [Debugging Commands](#debugging-commands)
- [Emergency Procedures](#emergency-procedures)

## Prerequisites Check

### Verify Environment

```bash
# Check Minikube status
minikube status

# Check kubectl connectivity
kubectl cluster-info

# Check Docker daemon
docker ps

# Check available resources
minikube ssh "free -h && df -h"
```

### Required Resources

- **Memory**: Minimum 8GB RAM
- **CPU**: Minimum 4 cores
- **Storage**: Minimum 20GB free space
- **Network**: Internet connectivity for image pulls

## Common Deployment Issues

### 1. Minikube Not Starting

**Symptoms:**

- `minikube start` fails
- Error messages about insufficient resources

**Solutions:**

```bash
# Check system resources
minikube start --memory=8192 --cpus=4 --disk-size=20g

# If still failing, try with more resources
minikube start --memory=16384 --cpus=8 --disk-size=50g

# Check for conflicting processes
minikube delete
minikube start --memory=8192 --cpus=4
```

### 2. Images Not Building

**Symptoms:**

- `make k8s-build` fails
- Docker build errors

**Solutions:**

```bash
# Check Docker daemon
docker ps

# Configure Minikube Docker environment
eval $(minikube docker-env)

# Clean Docker cache
docker system prune -f

# Rebuild images
make k8s-build
```

### 3. Pods Stuck in Pending

**Symptoms:**

- Pods show `Pending` status
- No nodes available

**Solutions:**

```bash
# Check node status
kubectl get nodes

# Check pod events
kubectl describe pod <pod-name> -n pogo-system

# Check resource availability
kubectl top nodes
```

### 4. Deployment Timeout

**Symptoms:**

- `make k8s-deploy` times out
- Services not ready after deployment

**Solutions:**

```bash
# Check deployment status
kubectl get deployments -n pogo-system

# Check pod readiness
kubectl get pods -n pogo-system

# Wait for specific deployment
kubectl wait --for=condition=available deployment/account-service -n pogo-system --timeout=300s
```

## Service-Specific Issues

### Microservices Issues

#### Account Service Problems

**Symptoms:**

- Account service pods not ready
- Authentication failures

**Diagnosis:**

```bash
# Check pod status
kubectl get pods -l app=account-service -n pogo-system

# Check logs
kubectl logs -l app=account-service -n pogo-system

# Check service endpoints
kubectl get endpoints account-service -n pogo-system
```

**Common Fixes:**

```bash
# Restart deployment
kubectl rollout restart deployment/account-service -n pogo-system

# Check database connectivity
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach sql --insecure -e "SELECT 1;"
```

#### BFF Gateway Issues

**Symptoms:**

- BFF pods not ready
- API gateway routing failures

**Diagnosis:**

```bash
# Check BFF status
kubectl get pods -l app=bot-bff -n pogo-system
kubectl get pods -l app=app-bff -n pogo-system

# Check BFF logs
kubectl logs -l app=bot-bff -n pogo-system
kubectl logs -l app=app-bff -n pogo-system
```

**Common Fixes:**

```bash
# Restart BFF deployments
kubectl rollout restart deployment/bot-bff -n pogo-system
kubectl rollout restart deployment/app-bff -n pogo-system

# Check Ocelot configuration
kubectl exec -it <bff-pod> -n pogo-system -- cat /app/ocelot.json
```

### Frontend Application Issues

#### Discord Bot Issues

**Symptoms:**

- Bot pod in CrashLoopBackOff
- Bot not responding to Discord

**Diagnosis:**

```bash
# Check bot pod status
kubectl get pods -l app=pogo-bot -n pogo-system

# Check bot logs
kubectl logs -l app=pogo-bot -n pogo-system

# Check for dependency issues
kubectl logs -l app=pogo-bot -n pogo-system | grep -i "error\|missing\|not found"
```

**Common Fixes:**

```bash
# Known issue: reflect-metadata dependency
# Check if this error appears in logs:
# "Error: Cannot find module 'reflect-metadata'"

# Temporary fix: Restart pod
kubectl delete pod -l app=pogo-bot -n pogo-system

# Check Discord token configuration
kubectl get secret discord-secrets -n pogo-system -o yaml
```

#### Mobile App Issues

**Symptoms:**

- App pod not ready
- App not accessible via NodePort

**Diagnosis:**

```bash
# Check app pod status
kubectl get pods -l app=pogo-app -n pogo-system

# Check app logs
kubectl logs -l app=pogo-app -n pogo-system

# Check NodePort service
kubectl get service pogo-app -n pogo-system
```

**Common Fixes:**

```bash
# Restart app deployment
kubectl rollout restart deployment/pogo-app -n pogo-system

# Check NodePort accessibility
minikube service pogo-app -n pogo-system
```

## Database Issues

### CockroachDB Problems

> **📖 For detailed information about CockroachDB migration handling, see [CockroachDB Migrations Guide](./cockroachdb-migrations.md)**

#### Migration Errors

**Symptoms:**
- Services not becoming ready
- Migration lock errors in logs: `42601: at or near "lock": syntax error`
- SQL syntax errors: `42883: unknown function: getutcdate()`
- SQL syntax errors: `42601: at or near "discorduserid": syntax error`

**Explanation:**
CockroachDB requires special migration handling because:
1. **No LOCK TABLE support** - CockroachDB doesn't support PostgreSQL's `LOCK TABLE ... IN ACCESS EXCLUSIVE MODE` syntax
2. **SQL Server syntax** - Some legacy code uses SQL Server syntax (`[Identifier]`, `GETUTCDATE()`) that needs conversion

**Solution:**
Our microservices use a shared migration runner (`DatabaseMigrationRunner`) that:
- Tries standard EF Core migration first
- Falls back to direct SQL execution when lock errors occur
- Automatically converts SQL Server syntax to PostgreSQL syntax
- Handles errors gracefully without crashing the service

See [CockroachDB Migrations Guide](./cockroachdb-migrations.md) for complete details.

#### Database Not Starting

**Symptoms:**

- CockroachDB pods not ready
- Database initialization failures

**Diagnosis:**

```bash
# Check CockroachDB status
kubectl get pods -l app=cockroachdb -n pogo-system

# Check database logs
kubectl logs -l app=cockroachdb -n pogo-system

# Check persistent volume claims
kubectl get pvc -n pogo-system
```

**Common Fixes:**

```bash
# Restart CockroachDB
kubectl delete pod -l app=cockroachdb -n pogo-system

# Check storage capacity
kubectl describe pvc cockroachdb-storage-cockroachdb-0 -n pogo-system

# Recreate database if needed
kubectl delete -f k8s/databases/
kubectl apply -f k8s/databases/
```

#### Connection Issues

**Symptoms:**

- Microservices can't connect to database
- Connection timeout errors

**Diagnosis:**

```bash
# Test database connectivity
kubectl run test-pod --image=postgres:15 --rm -it --restart=Never -- psql -h cockroachdb-public.pogo-system.svc.cluster.local -p 26257 -U root

# Check database service
kubectl get service cockroachdb-public -n pogo-system

# Check database endpoints
kubectl get endpoints cockroachdb-public -n pogo-system
```

**Common Fixes:**

```bash
# Restart database service
kubectl rollout restart statefulset/cockroachdb -n pogo-system

# Check connection string configuration
kubectl get configmap common-config -n pogo-system -o yaml
```

## Monitoring Issues

### Prometheus Issues

**Symptoms:**

- Prometheus not collecting metrics
- Metrics endpoints not accessible

**Diagnosis:**

```bash
# Check Prometheus status
kubectl get pods -l app=prometheus -n pogo-system

# Check Prometheus logs
kubectl logs -l app=prometheus -n pogo-system

# Test metrics endpoint
kubectl port-forward service/prometheus 9090:9090 -n pogo-system
curl http://localhost:9090/api/v1/targets
```

**Common Fixes:**

```bash
# Restart Prometheus
kubectl rollout restart deployment/prometheus -n pogo-system

# Check scrape configuration
kubectl get configmap prometheus-config -n pogo-system -o yaml
```

### Grafana Issues

**Symptoms:**

- Grafana not accessible
- Dashboards not loading

**Diagnosis:**

```bash
# Check Grafana status
kubectl get pods -l app=grafana -n pogo-system

# Check Grafana logs
kubectl logs -l app=grafana -n pogo-system

# Test Grafana access
kubectl port-forward service/grafana 3000:3000 -n pogo-system
curl http://localhost:3000/api/health
```

**Common Fixes:**

```bash
# Restart Grafana
kubectl rollout restart deployment/grafana -n pogo-system

# Check datasource configuration
kubectl get configmap grafana-datasources -n pogo-system -o yaml
```

## Network Issues

### Service Discovery Problems

**Symptoms:**

- Services can't reach each other
- DNS resolution failures

**Diagnosis:**

```bash
# Test DNS resolution
kubectl run test-pod --image=busybox --rm -it --restart=Never -- nslookup account-service.pogo-system.svc.cluster.local

# Check service endpoints
kubectl get endpoints -n pogo-system

# Test service connectivity
kubectl run test-pod --image=curlimages/curl --rm -it --restart=Never -- curl http://account-service.pogo-system.svc.cluster.local:5001/health/ready
```

**Common Fixes:**

```bash
# Restart CoreDNS
kubectl rollout restart deployment/coredns -n kube-system

# Check network policies
kubectl get networkpolicies -n pogo-system
```

### Ingress Issues

**Symptoms:**

- Ingress not routing traffic
- External access not working

**Diagnosis:**

```bash
# Check ingress status
kubectl get ingress -n pogo-system

# Check ingress controller
kubectl get pods -n ingress-nginx

# Check ingress logs
kubectl logs -n ingress-nginx deployment/ingress-nginx-controller
```

**Common Fixes:**

```bash
# Restart ingress controller
kubectl rollout restart deployment/ingress-nginx-controller -n ingress-nginx

# Check ingress configuration
kubectl describe ingress pogo-ingress -n pogo-system
```

## Resource Issues

### Memory Issues

**Symptoms:**

- Pods being killed (OOMKilled)
- Slow performance

**Diagnosis:**

```bash
# Check memory usage
kubectl top pods -n pogo-system
kubectl top nodes

# Check resource limits
kubectl describe pod <pod-name> -n pogo-system | grep -A 5 "Limits:"
```

**Common Fixes:**

```bash
# Increase memory limits
kubectl patch deployment account-service -n pogo-system -p '{"spec":{"template":{"spec":{"containers":[{"name":"account-service","resources":{"limits":{"memory":"1Gi"}}}]}}}}'

# Restart Minikube with more memory
minikube stop
minikube start --memory=16384 --cpus=8
```

### CPU Issues

**Symptoms:**

- High CPU usage
- Slow response times

**Diagnosis:**

```bash
# Check CPU usage
kubectl top pods -n pogo-system
kubectl top nodes

# Check CPU limits
kubectl describe pod <pod-name> -n pogo-system | grep -A 5 "Limits:"
```

**Common Fixes:**

```bash
# Increase CPU limits
kubectl patch deployment account-service -n pogo-system -p '{"spec":{"template":{"spec":{"containers":[{"name":"account-service","resources":{"limits":{"cpu":"500m"}}}]}}}}'

# Scale deployment
kubectl scale deployment account-service --replicas=2 -n pogo-system
```

## Debugging Commands

### General Debugging

```bash
# Get detailed pod information
kubectl describe pod <pod-name> -n pogo-system

# Check pod events
kubectl get events -n pogo-system --sort-by='.lastTimestamp'

# Check resource usage
kubectl top pods -n pogo-system
kubectl top nodes

# Check all resources
kubectl get all -n pogo-system
```

### Service Debugging

```bash
# Check service configuration
kubectl describe service <service-name> -n pogo-system

# Check service endpoints
kubectl get endpoints <service-name> -n pogo-system

# Test service connectivity
kubectl run test-pod --image=curlimages/curl --rm -it --restart=Never -- curl http://<service-name>.pogo-system.svc.cluster.local:<port>/health/ready
```

### Database Debugging

```bash
# Connect to database
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach sql --insecure

# Check database status
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach node status --insecure

# Check database logs
kubectl logs cockroachdb-0 -n pogo-system
```

## Emergency Procedures

### Complete Reset

If everything is broken and you need to start fresh:

```bash
# Stop Minikube
minikube stop

# Delete Minikube cluster
minikube delete

# Start fresh
minikube start --memory=8192 --cpus=4
minikube addons enable ingress
minikube addons enable metrics-server

# Rebuild and deploy
make k8s-build
make k8s-deploy
```

### Partial Reset

If only some services are broken:

```bash
# Remove specific service
kubectl delete -f k8s/microservices/account-service-deployment.yaml

# Rebuild and redeploy
make k8s-build
kubectl apply -f k8s/microservices/account-service-deployment.yaml
```

### Data Recovery

If you need to recover data:

```bash
# Backup current data
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach dump --insecure --database=pogo_account > account_backup.sql

# Restore from backup
kubectl exec -i cockroachdb-0 -n pogo-system -- cockroach sql --insecure < account_backup.sql
```

## Getting Help

### Log Collection

Before asking for help, collect these logs:

```bash
# System information
minikube version
kubectl version
docker --version

# Cluster status
kubectl get nodes
kubectl get pods -n pogo-system
kubectl get services -n pogo-system

# Resource usage
kubectl top pods -n pogo-system
kubectl top nodes

# Recent events
kubectl get events -n pogo-system --sort-by='.lastTimestamp'

# Service logs
kubectl logs -l app=account-service -n pogo-system --tail=100
kubectl logs -l app=cockroachdb -n pogo-system --tail=100
```

### Common Error Messages

| Error Message            | Cause                  | Solution                          |
| ------------------------ | ---------------------- | --------------------------------- |
| `ImagePullBackOff`       | Image not found        | Run `make k8s-build`              |
| `CrashLoopBackOff`       | Application error      | Check logs with `kubectl logs`    |
| `Pending`                | No resources available | Check node capacity               |
| `OOMKilled`              | Out of memory          | Increase memory limits            |
| `Readiness probe failed` | Health check failing   | Check application health endpoint |

---

For more help, check the [Kubernetes Documentation](kubernetes.md) or contact the development team.
