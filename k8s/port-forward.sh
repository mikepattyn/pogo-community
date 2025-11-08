#!/bin/bash

# POGO Community - Port Forwarding Management
# This script manages automatic port forwarding for monitoring and gateway services

set -e

# Colors for output
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Port forwarding configuration
# Format: "service-name:local-port:service-port"
SERVICES=(
    "swagger-gateway:10000:10000"
    "grafana:10001:3000"
    "prometheus:10002:9090"
    "cockroachdb-public:10003:26257"
)

# Helper function to get service name from config
get_service_name() {
    echo "$1" | cut -d: -f1
}

# Helper function to get local port from config
get_local_port() {
    echo "$1" | cut -d: -f2
}

# Helper function to get service port from config
get_service_port() {
    echo "$1" | cut -d: -f3
}

# PID file directory
PID_DIR="/tmp"
NAMESPACE="pogo-system"

# Function to check if kubectl is available
check_kubectl() {
    if ! command -v kubectl &> /dev/null; then
        echo -e "${RED}❌ kubectl is not installed or not in PATH${NC}"
        exit 1
    fi
}

# Function to check if namespace exists
check_namespace() {
    if ! kubectl get namespace "$NAMESPACE" &> /dev/null; then
        echo -e "${RED}❌ Namespace '$NAMESPACE' does not exist${NC}"
        exit 1
    fi
}

# Function to check if service exists and is ready
check_service() {
    local service_name=$1
    if ! kubectl get service "$service_name" -n "$NAMESPACE" &> /dev/null; then
        echo -e "${RED}❌ Service '$service_name' does not exist in namespace '$NAMESPACE'${NC}"
        return 1
    fi

    # Check if service has endpoints
    local endpoints=$(kubectl get endpoints "$service_name" -n "$NAMESPACE" -o jsonpath='{.subsets[*].addresses[*].ip}' 2>/dev/null)
    if [ -z "$endpoints" ]; then
        echo -e "${YELLOW}⚠️  Service '$service_name' has no endpoints yet${NC}"
        return 1
    fi

    return 0
}

# Function to start port forwarding for a service
start_service_port_forward() {
    local service_config=$1
    local service_name=$(get_service_name "$service_config")
    local local_port=$(get_local_port "$service_config")
    local service_port=$(get_service_port "$service_config")
    local pid_file="$PID_DIR/pogo-port-forward-$service_name.pid"

    # Check if already running
    if [ -f "$pid_file" ] && kill -0 "$(cat "$pid_file")" 2>/dev/null; then
        echo -e "${YELLOW}⚠️  Port forwarding for '$service_name' is already running (PID: $(cat "$pid_file"))${NC}"
        return 0
    fi

    # Check if service is ready
    if ! check_service "$service_name"; then
        echo -e "${RED}❌ Cannot start port forwarding for '$service_name' - service not ready${NC}"
        return 1
    fi

    # Start port forwarding in background
    local port_mapping="$local_port:$service_port"
    echo -e "${CYAN}  → Starting port forwarding for '$service_name' (localhost:$port_mapping)...${NC}"
    kubectl port-forward "service/$service_name" "$port_mapping" -n "$NAMESPACE" > /dev/null 2>&1 &
    local pid=$!

    # Save PID
    echo "$pid" > "$pid_file"

    # Wait a moment and check if process is still running
    sleep 2
    if kill -0 "$pid" 2>/dev/null; then
        echo -e "${GREEN}  ✅ Port forwarding for '$service_name' started successfully (PID: $pid)${NC}"
        return 0
    else
        echo -e "${RED}  ❌ Failed to start port forwarding for '$service_name'${NC}"
        rm -f "$pid_file"
        return 1
    fi
}

# Function to stop port forwarding for a service
stop_service_port_forward() {
    local service_name=$1
    local pid_file="$PID_DIR/pogo-port-forward-$service_name.pid"

    if [ -f "$pid_file" ]; then
        local pid=$(cat "$pid_file")
        if kill -0 "$pid" 2>/dev/null; then
            echo -e "${CYAN}  → Stopping port forwarding for '$service_name' (PID: $pid)...${NC}"
            kill "$pid" 2>/dev/null || true
            sleep 1
            # Force kill if still running
            if kill -0 "$pid" 2>/dev/null; then
                kill -9 "$pid" 2>/dev/null || true
            fi
            echo -e "${GREEN}  ✅ Port forwarding for '$service_name' stopped${NC}"
        else
            echo -e "${YELLOW}  ⚠️  Port forwarding for '$service_name' was not running${NC}"
        fi
        rm -f "$pid_file"
    else
        echo -e "${YELLOW}  ⚠️  No PID file found for '$service_name'${NC}"
    fi
}

# Function to start all port forwards
start_port_forwards() {
    echo -e "${CYAN}🚀 Starting port forwarding for POGO services...${NC}"
    echo ""

    check_kubectl
    check_namespace

    local success_count=0
    local total_count=${#SERVICES[@]}

    for service_config in "${SERVICES[@]}"; do
        if start_service_port_forward "$service_config"; then
            ((success_count++))
        fi
        echo ""
    done

    echo -e "${CYAN}📊 Port forwarding summary: $success_count/$total_count services started${NC}"
    echo ""

    if [ "$success_count" -eq "$total_count" ]; then
        echo -e "${GREEN}✅ All port forwarding started successfully!${NC}"
        echo ""
        echo -e "${YELLOW}📋 Access your services:${NC}"
        echo -e "  Swagger Gateway: http://localhost:10000/swagger"
        echo -e "  Grafana:         http://localhost:10001"
        echo -e "  Prometheus:      http://localhost:10002"
        echo -e "  CockroachDB:     localhost:10003"
        echo ""
        echo -e "${YELLOW}💡 To stop port forwarding: ./k8s/port-forward.sh stop${NC}"
        echo -e "${YELLOW}💡 To check status: ./k8s/port-forward.sh status${NC}"
        return 0
    else
        echo -e "${RED}❌ Some port forwarding failed to start${NC}"
        return 1
    fi
}

# Function to stop all port forwards
stop_port_forwards() {
    echo -e "${CYAN}🛑 Stopping port forwarding for POGO services...${NC}"
    echo ""

    for service_config in "${SERVICES[@]}"; do
        local service_name=$(get_service_name "$service_config")
        stop_service_port_forward "$service_name"
    done

    echo ""
    echo -e "${GREEN}✅ All port forwarding stopped${NC}"
}

# Function to show status of port forwards
status_port_forwards() {
    echo -e "${CYAN}📊 Port forwarding status for POGO services:${NC}"
    echo ""

    local running_count=0
    local total_count=${#SERVICES[@]}

    for service_config in "${SERVICES[@]}"; do
        local service_name=$(get_service_name "$service_config")
        local local_port=$(get_local_port "$service_config")
        local pid_file="$PID_DIR/pogo-port-forward-$service_name.pid"

        if [ -f "$pid_file" ] && kill -0 "$(cat "$pid_file")" 2>/dev/null; then
            local pid=$(cat "$pid_file")
            echo -e "  ${GREEN}✅${NC} $service_name: localhost:$local_port (PID: $pid)"
            ((running_count++))
        else
            echo -e "  ${RED}❌${NC} $service_name: not running"
        fi
    done

    echo ""
    echo -e "${CYAN}Summary: $running_count/$total_count services running${NC}"

    if [ "$running_count" -gt 0 ]; then
        echo ""
        echo -e "${YELLOW}📋 Running services:${NC}"
        for service_config in "${SERVICES[@]}"; do
            local service_name=$(get_service_name "$service_config")
            local local_port=$(get_local_port "$service_config")
            local pid_file="$PID_DIR/pogo-port-forward-$service_name.pid"

            if [ -f "$pid_file" ] && kill -0 "$(cat "$pid_file")" 2>/dev/null; then
                echo -e "  http://localhost:$local_port - $service_name"
            fi
        done
    fi
}

# Function to clean up orphaned PID files
cleanup_orphaned_pids() {
    echo -e "${CYAN}🧹 Cleaning up orphaned PID files...${NC}"

    for service_config in "${SERVICES[@]}"; do
        local service_name=$(get_service_name "$service_config")
        local pid_file="$PID_DIR/pogo-port-forward-$service_name.pid"
        if [ -f "$pid_file" ]; then
            local pid=$(cat "$pid_file")
            if ! kill -0 "$pid" 2>/dev/null; then
                echo -e "  ${YELLOW}→ Removing orphaned PID file for '$service_name'${NC}"
                rm -f "$pid_file"
            fi
        fi
    done
}

# Main script logic
case "${1:-}" in
    "start")
        start_port_forwards
        ;;
    "stop")
        stop_port_forwards
        ;;
    "status")
        cleanup_orphaned_pids
        status_port_forwards
        ;;
    "restart")
        stop_port_forwards
        sleep 2
        start_port_forwards
        ;;
    *)
        echo -e "${CYAN}POGO Community - Port Forwarding Management${NC}"
        echo ""
        echo -e "${YELLOW}Usage:${NC}"
        echo "  $0 start    - Start port forwarding for all services"
        echo "  $0 stop     - Stop all port forwarding"
        echo "  $0 status   - Show status of port forwarding"
        echo "  $0 restart  - Restart all port forwarding"
        echo ""
        echo -e "${YELLOW}Services:${NC}"
        for service_config in "${SERVICES[@]}"; do
            local service_name=$(get_service_name "$service_config")
            local local_port=$(get_local_port "$service_config")
            local service_port=$(get_service_port "$service_config")
            echo "  $service_name: localhost:$local_port -> service:$service_port"
        done
        echo ""
        echo -e "${YELLOW}Examples:${NC}"
        echo "  ./k8s/port-forward.sh start"
        echo "  ./k8s/port-forward.sh status"
        echo "  ./k8s/port-forward.sh stop"
        exit 1
        ;;
esac
