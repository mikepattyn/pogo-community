#!/bin/bash

# ===========================================
# POGO Community - Complete Fresh Start
# ===========================================
# This script performs a complete cleanup of Docker, Minikube, and Kubernetes
# WARNING: This will delete ALL data including volumes and images

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to confirm destructive action
confirm_action() {
    local message=$1
    echo ""
    print_status $YELLOW "⚠️  WARNING: This will perform a complete cleanup!"
    echo ""
    echo "$message"
    echo ""
    read -p "Are you sure you want to continue? (yes/no): " -r
    echo ""
    if [[ ! $REPLY =~ ^[Yy]es$ ]]; then
        print_status $YELLOW "Cleanup cancelled."
        exit 0
    fi
}

# Main cleanup process
main() {
    echo "=========================================="
    echo "🧹 POGO Community - Complete Fresh Start"
    echo "=========================================="
    echo ""

    confirm_action "This will:\n- Stop all Docker containers\n- Remove all Docker images and volumes\n- Delete Minikube cluster\n- Clean Docker system\n- Clear Docker build cache"

    # Step 1: Stop Minikube
    print_status $BLUE "🛑 Stopping Minikube..."
    if minikube status &> /dev/null; then
        minikube stop
        print_status $GREEN "✅ Minikube stopped"
    else
        print_status $YELLOW "   Minikube was not running"
    fi

    # Step 2: Delete Minikube cluster
    print_status $BLUE "🗑️  Deleting Minikube cluster..."
    if minikube status --format '{{.Host}}' &> /dev/null; then
        minikube delete
        print_status $GREEN "✅ Minikube cluster deleted"
    else
        print_status $YELLOW "   No Minikube cluster found"
    fi

    # Step 3: Stop all Docker containers
    print_status $BLUE "🛑 Stopping all Docker containers..."
    if command -v docker &> /dev/null; then
        docker stop $(docker ps -aq) 2>/dev/null || print_status $YELLOW "   No running containers"
        print_status $GREEN "✅ All containers stopped"
    fi

    # Step 4: Remove all Docker containers
    print_status $BLUE "🗑️  Removing all Docker containers..."
    docker rm $(docker ps -aq) 2>/dev/null || print_status $YELLOW "   No containers to remove"
    print_status $GREEN "✅ All containers removed"

    # Step 5: Remove all Docker images
    print_status $BLUE "🗑️  Removing all Docker images..."
    docker rmi $(docker images -q) 2>/dev/null || print_status $YELLOW "   No images to remove"
    print_status $GREEN "✅ All images removed"

    # Step 6: Remove all Docker volumes (optional)
    if [ "$REMOVE_VOLUMES" = true ]; then
        print_status $YELLOW "🗑️  Removing all Docker volumes (database data will be lost)..."
        docker volume rm $(docker volume ls -q) 2>/dev/null || print_status $YELLOW "   No volumes to remove"
        print_status $GREEN "✅ All volumes removed"
    else
        print_status $YELLOW "⏭️  Skipping volume removal (use --volumes to remove)"
    fi

    # Step 7: Prune Docker system
    print_status $BLUE "🧹 Pruning Docker system..."
    if [ "$REMOVE_VOLUMES" = true ]; then
        docker system prune -af --volumes
        print_status $GREEN "✅ Docker system pruned (including volumes)"
    else
        docker system prune -af
        print_status $GREEN "✅ Docker system pruned (volumes preserved)"
    fi

    # Step 8: Clear Docker build cache
    print_status $BLUE "🧹 Clearing Docker build cache..."
    docker builder prune -af
    print_status $GREEN "✅ Docker build cache cleared"

    echo ""
    print_status $GREEN "🎉 Complete cleanup finished!"
    echo ""
    print_status $BLUE "📋 Next steps:"
    echo "  1. Start Minikube: minikube start --memory=8192 --cpus=12"
    echo "  2. Set docker context: eval \$(minikube docker-env)"
    echo "  3. Build images: make k8s-build"
    echo "  4. Create secrets: ./k8s/create-secrets.sh --auto"
    echo "  5. Deploy: ./k8s/deploy.sh"
    echo ""
    print_status $YELLOW "💡 Tips:"
    echo "  - Use './scripts/fresh-start.sh --volumes' to also remove database data"
    echo "  - Use './scripts/fresh-start.sh --reboot' to auto-start Minikube"
    echo "  - Use './scripts/fresh-start.sh --volumes --reboot' for complete reset"
    echo ""
}

# Function to show help
show_help() {
    cat << EOF
POGO Community - Fresh Start Script

USAGE:
    $0 [OPTIONS]

OPTIONS:
    --volumes      Remove Docker volumes (deletes all database data)
    --reboot       Start Minikube after cleanup (8GB RAM, 12 CPUs)
    -h, --help     Show this help message

EXAMPLES:
    $0                         # Standard cleanup (keeps volumes)
    $0 --volumes               # Remove volumes too
    $0 --reboot                # Auto-start Minikube after cleanup
    $0 --volumes --reboot      # Full cleanup and auto-start

EOF
}

# Parse command line arguments
REMOVE_VOLUMES=false
REBOOT=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --volumes)
            REMOVE_VOLUMES=true
            shift
            ;;
        --reboot)
            REBOOT=true
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Run main function
main

# Optionally start Minikube
if [ "$REBOOT" = true ]; then
    echo ""
    print_status $BLUE "🚀 Starting Minikube with 8GB RAM and 12 CPUs..."
    minikube start --memory=8192 --cpus=12

    print_status $GREEN "✅ Minikube started"
    echo ""
    print_status $BLUE "📋 Setting Docker context for Minikube..."
    eval $(minikube docker-env)
    print_status $GREEN "✅ Docker context set"
    echo ""
    print_status $BLUE "📋 Next steps:"
    echo "  1. Build images: make k8s-build"
    echo "  2. Create secrets: ./k8s/create-secrets.sh --auto"
    echo "  3. Deploy: ./k8s/deploy.sh"
    echo ""
fi

