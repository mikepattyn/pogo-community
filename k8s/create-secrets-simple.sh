#!/bin/bash

# ===========================================
# POGO Community - Kubernetes Secret Generator (Fixed)
# ===========================================
# This script creates all required Kubernetes secrets securely

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
NAMESPACE="pogo-system"

# Command line options
DISCORD_ONLY=false
GOOGLE_ONLY=false

# Function to show help
show_help() {
    cat << EOF
POGO Community - Kubernetes Secret Generator (Simple)

USAGE:
    $0 [--discord-only | --google-only]

OPTIONS:
    --discord-only    Update only the Discord bot token secret
    --google-only     Update only the Google API key secret
    -h, --help        Show this help message

EOF
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --discord-only)
            DISCORD_ONLY=true
            shift
            ;;
        --google-only)
            GOOGLE_ONLY=true
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

# Validate that both flags are not set
if [ "$DISCORD_ONLY" = true ] && [ "$GOOGLE_ONLY" = true ]; then
    echo "Error: Cannot use --discord-only and --google-only together"
    show_help
    exit 1
fi

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to generate secure random string
generate_random_string() {
    local length=$1
    openssl rand -base64 "$length" | tr -d "=+/" | cut -c1-"$length"
}

# Function to create secret
create_secret() {
    local secret_name=$1
    local secret_data=$2

    print_status $BLUE "🔐 Creating secret: $secret_name"

    # Delete existing secret if it exists
    if kubectl get secret "$secret_name" -n "$NAMESPACE" &> /dev/null; then
        kubectl delete secret "$secret_name" -n "$NAMESPACE"
    fi

    # Create secret from data using a temporary file
    local temp_file=$(mktemp)
    echo "$secret_data" > "$temp_file"

    kubectl create secret generic "$secret_name" \
        --from-env-file="$temp_file" \
        -n "$NAMESPACE"

    # Clean up temp file
    rm -f "$temp_file"

    print_status $GREEN "✅ Secret '$secret_name' created successfully"
}

# Main execution
main() {
    echo "=========================================="
    echo "🔐 POGO Community - Secret Generator"

    if [ "$DISCORD_ONLY" = true ]; then
        echo "   (Discord-only mode)"
    elif [ "$GOOGLE_ONLY" = true ]; then
        echo "   (Google-only mode)"
    fi

    echo "=========================================="
    echo ""

    # Check prerequisites
    print_status $BLUE "🔍 Checking prerequisites..."
    if ! command -v kubectl &> /dev/null; then
        print_status $RED "❌ kubectl is not installed"
        exit 1
    fi

    if ! kubectl cluster-info &> /dev/null; then
        print_status $RED "❌ Cannot access Kubernetes cluster"
        exit 1
    fi

    print_status $GREEN "✅ Prerequisites check passed"

    # Ensure namespace exists
    print_status $BLUE "📦 Ensuring namespace exists..."
    if ! kubectl get namespace "$NAMESPACE" &> /dev/null; then
        kubectl create namespace "$NAMESPACE"
    fi
    print_status $GREEN "✅ Namespace $NAMESPACE exists"

    echo ""

    # Handle google-only mode
    if [ "$GOOGLE_ONLY" = true ]; then
        # Get Google API Key
        print_status $BLUE "🔑 Google API Key"
        print_status $YELLOW "   Get your API key from: https://console.cloud.google.com"
        print_status $YELLOW "   Navigate to: APIs & Services > Credentials > Create Credentials > API Key"
        echo ""

        local google_api_key=""
        while [ -z "$google_api_key" ]; do
            read -s -p "Enter Google API Key: " google_api_key
            echo ""

            if [ -z "$google_api_key" ]; then
                print_status $RED "❌ API key cannot be empty"
            fi
        done

        echo ""

        # Create Google secret
        create_secret "google-secrets" "GOOGLE_API_KEY=$google_api_key"
    else
        # Get Discord token
        print_status $BLUE "🤖 Discord Bot Token"
        print_status $YELLOW "   Get your token from: https://discord.com/developers/applications"
        print_status $YELLOW "   Select your application > Bot > Token > Reset Token"
        echo ""

        local discord_token=""
        while [ -z "$discord_token" ]; do
            read -s -p "Enter Discord Bot Token: " discord_token
            echo ""

            if [ -z "$discord_token" ]; then
                print_status $RED "❌ Token cannot be empty"
            elif [[ ! "$discord_token" =~ ^MTA[0-9A-Za-z._-]{40,}$ ]]; then
                print_status $RED "❌ Invalid Discord token format"
                discord_token=""
            fi
        done

        echo ""

        # Create Discord secret
        create_secret "discord-secrets" "DISCORD_BOT_TOKEN=$discord_token"

        # Only create JWT and DB secrets if not in discord-only mode
        if [ "$DISCORD_ONLY" = false ]; then
            # Generate JWT secret
            print_status $BLUE "🔐 Generating JWT secret..."
            local jwt_secret=$(generate_random_string 32)
            print_status $GREEN "✅ JWT secret generated"

            # Get Google API Key
            print_status $BLUE "🔑 Google API Key"
            print_status $YELLOW "   Get your API key from: https://console.cloud.google.com"
            print_status $YELLOW "   Navigate to: APIs & Services > Credentials > Create Credentials > API Key"
            echo ""

            local google_api_key=""
            while [ -z "$google_api_key" ]; do
                read -s -p "Enter Google API Key: " google_api_key
                echo ""

                if [ -z "$google_api_key" ]; then
                    print_status $RED "❌ API key cannot be empty"
                fi
            done

            # Get database password
            print_status $BLUE "🗄️  Database Password"
            print_status $YELLOW "   Enter a strong password for SQL Server databases"
            echo ""

            local db_password=""
            while [ -z "$db_password" ]; do
                read -s -p "Enter MSSQL SA Password: " db_password
                echo ""

                if [ -z "$db_password" ]; then
                    print_status $RED "❌ Password cannot be empty"
                elif [ ${#db_password} -lt 8 ]; then
                    print_status $RED "❌ Password must be at least 8 characters"
                    db_password=""
                fi
            done
            print_status $GREEN "✅ Database password set"

            echo ""

            # Create JWT, Google, and DB secrets
            create_secret "jwt-secrets" "JWT_SECRET_KEY=$jwt_secret
JWT_ISSUER=pogo-community
JWT_AUDIENCE=pogo-community-users
JWT_EXPIRY_MINUTES=60"
            create_secret "google-secrets" "GOOGLE_API_KEY=$google_api_key"
            create_secret "db-secrets" "DB_USERNAME=root
DB_PASSWORD=
MSSQL_SA_PASSWORD=$db_password"
        fi
    fi

    echo ""
    print_status $GREEN "🎉 Secret(s) created successfully!"
    echo ""
    print_status $BLUE "📋 Next steps:"
    echo "  1. Run deployment: ./k8s/deploy.sh"
    echo "  2. Check pod status: kubectl get pods -n $NAMESPACE"
    echo ""
}

# Run main function
main "$@"
