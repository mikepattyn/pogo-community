#!/bin/bash

# ===========================================
# POGO Community - Kubernetes Secret Generator
# ===========================================
# This script creates all required Kubernetes secrets securely
# Usage: ./k8s/create-secrets.sh [--auto] [--from-env] [--update SECRET_NAME] [--dry-run]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
NAMESPACE="pogo-system"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Command line options
AUTO_GENERATE=false
FROM_ENV=false
DRY_RUN=false
UPDATE_SECRET=""

# Function to show help
show_help() {
    cat << EOF
POGO Community - Kubernetes Secret Generator

USAGE:
    $0 [OPTIONS]

OPTIONS:
    --auto          Auto-generate JWT and DB secrets, prompt for Discord token
    --from-env      Read secrets from environment variables
    --update NAME   Update specific secret (discord-secrets, jwt-secrets, db-secrets)
    --dry-run       Show what would be created without applying
    -h, --help      Show this help message

EXAMPLES:
    $0                          # Interactive mode
    $0 --auto                   # Auto-generate mode
    $0 --from-env               # From environment variables
    $0 --update discord-secrets # Update specific secret
    $0 --dry-run                # Preview changes

ENVIRONMENT VARIABLES (for --from-env):
    DISCORD_BOT_TOKEN
    JWT_SECRET_KEY
    JWT_ISSUER
    JWT_AUDIENCE
    JWT_EXPIRY_MINUTES
    DB_USERNAME
    DB_PASSWORD
    MSSQL_SA_PASSWORD

EOF
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --auto)
            AUTO_GENERATE=true
            shift
            ;;
        --from-env)
            FROM_ENV=true
            shift
            ;;
        --update)
            UPDATE_SECRET="$2"
            shift 2
            ;;
        --dry-run)
            DRY_RUN=true
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

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to check prerequisites
check_prerequisites() {
    print_status $BLUE "🔍 Checking prerequisites..."

    # Check kubectl
    if ! command -v kubectl &> /dev/null; then
        print_status $RED "❌ kubectl is not installed or not in PATH"
        exit 1
    fi

    # Check cluster access
    if ! kubectl cluster-info &> /dev/null; then
        print_status $RED "❌ Cannot access Kubernetes cluster"
        print_status $YELLOW "   Make sure kubectl is configured and cluster is running"
        exit 1
    fi

    # Check openssl for auto-generation
    if [ "$AUTO_GENERATE" = true ] && ! command -v openssl &> /dev/null; then
        print_status $RED "❌ openssl is required for auto-generation mode"
        exit 1
    fi

    print_status $GREEN "✅ Prerequisites check passed"
}

# Function to ensure namespace exists
ensure_namespace() {
    print_status $BLUE "📦 Ensuring namespace exists..."

    if ! kubectl get namespace "$NAMESPACE" &> /dev/null; then
        print_status $YELLOW "   Creating namespace: $NAMESPACE"
        if [ "$DRY_RUN" = false ]; then
            kubectl create namespace "$NAMESPACE"
        fi
    else
        print_status $GREEN "✅ Namespace $NAMESPACE exists"
    fi
}

# Function to check existing secrets
check_existing_secrets() {
    print_status $BLUE "🔍 Checking existing secrets..."

    local secrets=("discord-secrets" "jwt-secrets" "db-secrets")
    local existing_secrets=()

    for secret in "${secrets[@]}"; do
        if kubectl get secret "$secret" -n "$NAMESPACE" &> /dev/null; then
            existing_secrets+=("$secret")
            print_status $YELLOW "   Found existing secret: $secret"
        fi
    done

    if [ ${#existing_secrets[@]} -gt 0 ]; then
        echo ""
        print_status $YELLOW "⚠️  Existing secrets found: ${existing_secrets[*]}"

        if [ -n "$UPDATE_SECRET" ]; then
            if [[ " ${existing_secrets[*]} " =~ " $UPDATE_SECRET " ]]; then
                print_status $BLUE "   Will update: $UPDATE_SECRET"
            else
                print_status $RED "❌ Secret '$UPDATE_SECRET' not found in existing secrets"
                exit 1
            fi
        else
            echo ""
            read -p "Do you want to update existing secrets? (y/N): " -n 1 -r
            echo ""
            if [[ ! $REPLY =~ ^[Yy]$ ]]; then
                print_status $YELLOW "   Keeping existing secrets"
                return 1
            fi
        fi
    fi

    return 0
}

# Function to generate secure random string
generate_random_string() {
    local length=$1
    openssl rand -base64 "$length" | tr -d "=+/" | cut -c1-"$length"
}

# Function to get Discord bot token
get_discord_token() {
    local token=""

    if [ "$FROM_ENV" = true ] && [ -n "$DISCORD_BOT_TOKEN" ]; then
        token="$DISCORD_BOT_TOKEN"
        print_status $GREEN "✅ Using Discord token from environment"
    else
        echo ""
        print_status $BLUE "🤖 Discord Bot Token"
        print_status $YELLOW "   Get your token from: https://discord.com/developers/applications"
        print_status $YELLOW "   Select your application > Bot > Token > Reset Token"
        echo ""

        while [ -z "$token" ]; do
            read -s -p "Enter Discord Bot Token: " token
            echo ""

            if [ -z "$token" ]; then
                print_status $RED "❌ Token cannot be empty"
            elif [[ ! "$token" =~ ^MTA[0-9A-Za-z._-]{40,}$ ]]; then
                print_status $RED "❌ Invalid Discord token format"
                token=""
            fi
        done
    fi

    echo "$token"
}

# Function to get JWT configuration
get_jwt_config() {
    local jwt_secret=""
    local jwt_issuer="pogo-community"
    local jwt_audience="pogo-community-users"
    local jwt_expiry="60"

    if [ "$AUTO_GENERATE" = true ]; then
        jwt_secret=$(generate_random_string 32)
        print_status $GREEN "✅ Auto-generated JWT secret"
    elif [ "$FROM_ENV" = true ] && [ -n "$JWT_SECRET_KEY" ]; then
        jwt_secret="$JWT_SECRET_KEY"
        print_status $GREEN "✅ Using JWT secret from environment"
    else
        echo ""
        print_status $BLUE "🔐 JWT Configuration"

        while [ -z "$jwt_secret" ]; do
            read -s -p "Enter JWT Secret Key (min 32 chars): " jwt_secret
            echo ""

            if [ ${#jwt_secret} -lt 32 ]; then
                print_status $RED "❌ JWT secret must be at least 32 characters"
                jwt_secret=""
            fi
        done

        read -p "JWT Issuer [$jwt_issuer]: " input_issuer
        jwt_issuer="${input_issuer:-$jwt_issuer}"

        read -p "JWT Audience [$jwt_audience]: " input_audience
        jwt_audience="${input_audience:-$jwt_audience}"

        read -p "JWT Expiry Minutes [$jwt_expiry]: " input_expiry
        jwt_expiry="${input_expiry:-$jwt_expiry}"
    fi

    # Override with environment variables if available
    [ -n "$JWT_ISSUER" ] && jwt_issuer="$JWT_ISSUER"
    [ -n "$JWT_AUDIENCE" ] && jwt_audience="$JWT_AUDIENCE"
    [ -n "$JWT_EXPIRY_MINUTES" ] && jwt_expiry="$JWT_EXPIRY_MINUTES"

    echo "JWT_SECRET_KEY=$jwt_secret"
    echo "JWT_ISSUER=$jwt_issuer"
    echo "JWT_AUDIENCE=$jwt_audience"
    echo "JWT_EXPIRY_MINUTES=$jwt_expiry"
}

# Function to get database configuration
get_db_config() {
    local db_username="root"
    local db_password=""
    local mssql_password=""

    if [ "$AUTO_GENERATE" = true ]; then
        mssql_password=$(generate_random_string 24)
        print_status $GREEN "✅ Auto-generated MSSQL password"
    elif [ "$FROM_ENV" = true ]; then
        [ -n "$DB_USERNAME" ] && db_username="$DB_USERNAME"
        [ -n "$DB_PASSWORD" ] && db_password="$DB_PASSWORD"
        [ -n "$MSSQL_SA_PASSWORD" ] && mssql_password="$MSSQL_SA_PASSWORD"
        print_status $GREEN "✅ Using database config from environment"
    else
        echo ""
        print_status $BLUE "🗄️  Database Configuration"

        read -p "DB Username [$db_username]: " input_username
        db_username="${input_username:-$db_username}"

        read -s -p "DB Password (empty for CockroachDB): " db_password
        echo ""

        while [ -z "$mssql_password" ]; do
            read -s -p "MSSQL SA Password: " mssql_password
            echo ""

            if [ -z "$mssql_password" ]; then
                print_status $RED "❌ MSSQL password cannot be empty"
            elif [ ${#mssql_password} -lt 8 ]; then
                print_status $RED "❌ Password must be at least 8 characters"
                mssql_password=""
            fi
        done
    fi

    echo "DB_USERNAME=$db_username"
    echo "DB_PASSWORD=$db_password"
    echo "MSSQL_SA_PASSWORD=$mssql_password"
}

# Function to create secret
create_secret() {
    local secret_name=$1
    local secret_data=$2

    print_status $BLUE "🔐 Creating secret: $secret_name"

    if [ "$DRY_RUN" = true ]; then
        print_status $YELLOW "   [DRY RUN] Would create secret with keys:"
        echo "$secret_data" | cut -d'=' -f1 | sed 's/^/     - /'
        return 0
    fi

    # Delete existing secret if it exists
    if kubectl get secret "$secret_name" -n "$NAMESPACE" &> /dev/null; then
        kubectl delete secret "$secret_name" -n "$NAMESPACE"
    fi

    # Create secret from data using a temporary file to avoid shell issues
    local temp_file=$(mktemp)
    echo "$secret_data" > "$temp_file"

    kubectl create secret generic "$secret_name" \
        --from-env-file="$temp_file" \
        -n "$NAMESPACE"

    # Clean up temp file
    rm -f "$temp_file"

    print_status $GREEN "✅ Secret '$secret_name' created successfully"
}

# Function to verify secrets
verify_secrets() {
    print_status $BLUE "🔍 Verifying secrets..."

    local secrets=("discord-secrets" "jwt-secrets" "db-secrets")
    local all_good=true

    for secret in "${secrets[@]}"; do
        if kubectl get secret "$secret" -n "$NAMESPACE" &> /dev/null; then
            print_status $GREEN "✅ $secret exists"
        else
            print_status $RED "❌ $secret missing"
            all_good=false
        fi
    done

    if [ "$all_good" = true ]; then
        print_status $GREEN "✅ All secrets verified"
        return 0
    else
        print_status $RED "❌ Some secrets are missing"
        return 1
    fi
}

# Main execution
main() {
    echo "=========================================="
    echo "🔐 POGO Community - Secret Generator"
    echo "=========================================="
    echo ""

    # Check if we're updating a specific secret
    if [ -n "$UPDATE_SECRET" ]; then
        print_status $BLUE "🔄 Update mode: $UPDATE_SECRET"
    elif [ "$AUTO_GENERATE" = true ]; then
        print_status $BLUE "🤖 Auto-generate mode"
    elif [ "$FROM_ENV" = true ]; then
        print_status $BLUE "🌍 Environment variable mode"
    else
        print_status $BLUE "💬 Interactive mode"
    fi

    if [ "$DRY_RUN" = true ]; then
        print_status $YELLOW "🔍 DRY RUN MODE - No changes will be made"
    fi

    echo ""

    # Run checks
    check_prerequisites
    ensure_namespace

    # Check existing secrets
    if ! check_existing_secrets; then
        print_status $YELLOW "   Exiting without changes"
        exit 0
    fi

    echo ""

    # Create secrets
    if [ -z "$UPDATE_SECRET" ] || [ "$UPDATE_SECRET" = "discord-secrets" ]; then
        discord_token=$(get_discord_token)
        create_secret "discord-secrets" "DISCORD_BOT_TOKEN=$discord_token"
    fi

    if [ -z "$UPDATE_SECRET" ] || [ "$UPDATE_SECRET" = "jwt-secrets" ]; then
        jwt_config=$(get_jwt_config)
        create_secret "jwt-secrets" "$jwt_config"
    fi

    if [ -z "$UPDATE_SECRET" ] || [ "$UPDATE_SECRET" = "db-secrets" ]; then
        db_config=$(get_db_config)
        create_secret "db-secrets" "$db_config"
    fi

    echo ""

    # Verify secrets
    if [ "$DRY_RUN" = false ]; then
        verify_secrets
    fi

    echo ""
    print_status $GREEN "🎉 Secret generation complete!"
    echo ""

    if [ "$DRY_RUN" = false ]; then
        print_status $BLUE "📋 Next steps:"
        echo "  1. Run deployment: ./k8s/deploy.sh"
        echo "  2. Check pod status: kubectl get pods -n $NAMESPACE"
        echo "  3. View logs: kubectl logs -f deployment/pogo-bot -n $NAMESPACE"
    fi

    echo ""
    print_status $YELLOW "⚠️  Remember: Never commit secret values to git!"
}

# Run main function
main "$@"
