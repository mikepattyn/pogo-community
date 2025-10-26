# POGO Community - Kubernetes Quick Reference

Quick reference guide for common Kubernetes operations with the POGO Community platform.

## 🚀 Quick Start Commands

```bash
# Start Minikube
minikube start --memory=8192 --cpus=4
minikube addons enable ingress

# Build and deploy everything
make k8s-build
make k8s-deploy
make k8s-validate
```

## 📊 Status Commands

```bash
# Check overall status
make k8s-status

# Check specific resources
kubectl get pods -n pogo-system
kubectl get services -n pogo-system
kubectl get ingress -n pogo-system

# Check resource usage
kubectl top pods -n pogo-system
kubectl top nodes
```

## 🔧 Service Management

```bash
# Restart services
kubectl rollout restart deployment/account-service -n pogo-system
kubectl rollout restart deployment/bot-bff -n pogo-system

# Scale services
kubectl scale deployment/account-service --replicas=3 -n pogo-system

# Check service status
kubectl get pods -l app=account-service -n pogo-system
kubectl get endpoints account-service -n pogo-system
```

## 📝 Logs and Debugging

```bash
# View logs
make k8s-logs
kubectl logs -l app=account-service -n pogo-system
kubectl logs -l app=cockroachdb -n pogo-system

# Follow logs
kubectl logs -l app=account-service -n pogo-system -f

# Debug pods
kubectl describe pod <pod-name> -n pogo-system
kubectl exec -it <pod-name> -n pogo-system -- /bin/bash
```

## 🌐 Access Applications

```bash
# Get Minikube IP
minikube ip

# Port forward for local access
kubectl port-forward service/bot-bff 6001:6001 -n pogo-system
kubectl port-forward service/app-bff 6002:6002 -n pogo-system
kubectl port-forward service/prometheus 9090:9090 -n pogo-system
kubectl port-forward service/grafana 3000:3000 -n pogo-system

# Access via NodePort
minikube service pogo-app -n pogo-system
```

## 🗄️ Database Operations

```bash
# Connect to database
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach sql --insecure

# Check database status
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach node status --insecure

# Backup database
kubectl exec -it cockroachdb-0 -n pogo-system -- cockroach dump --insecure --database=pogo_account > backup.sql
```

## 📈 Monitoring

```bash
# Access Prometheus
kubectl port-forward service/prometheus 9090:9090 -n pogo-system
# Open: http://localhost:9090

# Access Grafana
kubectl port-forward service/grafana 3000:3000 -n pogo-system
# Open: http://localhost:3000 (admin/admin)

# Check metrics
curl http://localhost:9090/api/v1/query?query=up
```

## 🧹 Cleanup Commands

```bash
# Remove all resources
make k8s-teardown

# Clean specific service
kubectl delete deployment/account-service -n pogo-system

# Clean Minikube
minikube stop
minikube delete
```

## 🔍 Troubleshooting

```bash
# Check events
kubectl get events -n pogo-system --sort-by='.lastTimestamp'

# Test connectivity
kubectl run test-pod --image=curlimages/curl --rm -it --restart=Never -- curl http://account-service.pogo-system.svc.cluster.local:5001/health/ready

# Check DNS resolution
kubectl run test-pod --image=busybox --rm -it --restart=Never -- nslookup account-service.pogo-system.svc.cluster.local
```

## 📋 Service Ports

| Service | Port | Type | Access |
|---------|------|------|--------|
| Account Service | 5001 | ClusterIP | Internal |
| Player Service | 5002 | ClusterIP | Internal |
| Location Service | 5003 | ClusterIP | Internal |
| Gym Service | 5004 | ClusterIP | Internal |
| Raid Service | 5005 | ClusterIP | Internal |
| Bot BFF | 6001 | ClusterIP | Port Forward |
| App BFF | 6002 | ClusterIP | Port Forward |
| Discord Bot | 2000 | ClusterIP | Port Forward |
| Mobile App | 3000 | NodePort | minikube service |
| CockroachDB | 26257 | ClusterIP | Internal |
| Prometheus | 9090 | NodePort | Port Forward |
| Grafana | 3000 | NodePort | Port Forward |

## 🎯 Health Check Endpoints

```bash
# Check service health
curl http://localhost:5001/health/ready  # Account Service
curl http://localhost:5002/health/ready  # Player Service
curl http://localhost:5003/health/ready  # Location Service
curl http://localhost:5004/health/ready  # Gym Service
curl http://localhost:5005/health/ready  # Raid Service
curl http://localhost:6001/health/ready  # Bot BFF
curl http://localhost:6002/health/ready  # App BFF

# Check metrics
curl http://localhost:5001/metrics  # Account Service metrics
curl http://localhost:6001/metrics  # Bot BFF metrics
```

## 🚨 Emergency Commands

```bash
# Complete reset
minikube delete
minikube start --memory=8192 --cpus=4
make k8s-build
make k8s-deploy

# Restart all deployments
kubectl rollout restart deployment --all -n pogo-system

# Force delete stuck pods
kubectl delete pod <pod-name> -n pogo-system --force --grace-period=0
```

---

For detailed information, see [Kubernetes Documentation](kubernetes.md) and [Troubleshooting Guide](troubleshooting.md).
