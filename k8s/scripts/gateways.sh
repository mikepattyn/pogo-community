#!/bin/bash
# Interactive script to manage gateways: rebuild, remove, and deploy

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
GATEWAYS_DIR="${SCRIPT_DIR}/gateways"
source "${SCRIPT_DIR}/lib/gum-utils.sh"

check_gum_installed

# Gateways configuration
GATEWAY_NAMES=(
    "📚 swagger-gateway"
)

GATEWAY_NAMES_PLAIN=(
    "swagger-gateway"
)

# Function to get directory name for a gateway
get_gateway_dir() {
    case "$1" in
        "swagger-gateway")
            echo "Swagger.Gateway"
            ;;
        *)
            echo ""
            ;;
    esac
}

# Extract plain name from emoji-prefixed name
get_plain_name() {
    echo "$1" | sed 's/^[^a-z]*//'
}

select_gateway() {
    local selected=$(gum choose --header "Select Gateway" "${GATEWAY_NAMES[@]}" "🌟 All gateways")

    if [[ "$selected" == "🌟 All gateways" ]]; then
        echo "all"
    elif [[ -n "$selected" ]]; then
        get_plain_name "$selected"
    fi
}

rebuild_single() {
    local gateway=$1
    local gateway_dir=$(get_gateway_dir "$gateway")

    if [[ -z "$gateway_dir" ]]; then
        gum_error "Unknown gateway: ${gateway}"
        return 1
    fi

    configure_docker

    if gum_spinner "Rebuilding ${gateway} image..." \
        docker build -t "pogo/${gateway}:latest" \
        -f "apps/backend/gateways/${gateway_dir}/Dockerfile" .; then
        gum_success "${gateway} image rebuilt successfully!"
        return 0
    else
        gum_error "Failed to rebuild ${gateway} image"
        return 1
    fi
}

remove_single() {
    local gateway=$1

    if [[ -z "$gateway" ]]; then
        gum_error "Unknown gateway: ${gateway}"
        return 1
    fi

    gum_info "Removing ${gateway} deployment..."

    kubectl delete deployment "${gateway}" -n pogo-system --ignore-not-found=true

    gum_spinner "Waiting for pod to be removed..." \
        kubectl wait --for=delete pod -l app="${gateway}" -n pogo-system --timeout=60s || true

    gum_success "${gateway} pods removed!"
    return 0
}

deploy_single() {
    local gateway=$1

    if [[ -z "$gateway" ]]; then
        gum_error "Unknown gateway: ${gateway}"
        return 1
    fi

    gum_info "Deploying ${gateway}..."

    kubectl apply -f "k8s/gateways/${gateway}-deployment.yaml"

    if gum_spinner "Waiting for deployment to be ready..." \
        kubectl wait --for=condition=available "deployment/${gateway}" -n pogo-system --timeout=300s; then
        gum_success "${gateway} deployed successfully!"
        return 0
    else
        gum_error "Failed to deploy ${gateway}"
        return 1
    fi
}

run_rebuild() {
    local gateway=$1

    if [[ -n "$gateway" && "$gateway" != "all" ]]; then
        rebuild_single "$gateway"
        return $?
    fi

    gum_info "Running rebuild script..."
    bash "${GATEWAYS_DIR}/rebuild.sh"
    return $?
}

run_remove() {
    local gateway=$1
    local confirmation_msg="This will delete all gateway deployments from the cluster."

    if [[ -n "$gateway" && "$gateway" != "all" ]]; then
        confirmation_msg="This will delete the ${gateway} deployment from the cluster."
    fi

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    if [[ -n "$gateway" && "$gateway" != "all" ]]; then
        remove_single "$gateway"
        return $?
    fi

    gum_info "Running remove script..."
    bash "${GATEWAYS_DIR}/remove.sh"
    return $?
}

run_deploy() {
    local gateway=$1

    if [[ -n "$gateway" && "$gateway" != "all" ]]; then
        deploy_single "$gateway"
        return $?
    fi

    gum_info "Running deploy script..."
    bash "${GATEWAYS_DIR}/deploy.sh"
    return $?
}

run_all() {
    local selected_gateway=$(select_gateway)

    if [[ -z "$selected_gateway" ]]; then
        gum_error "Invalid selection"
        return 1
    fi

    local action_desc="Run all operations"
    if [[ "$selected_gateway" == "all" ]]; then
        action_desc="Run all operations for all gateways"
    else
        action_desc="Run all operations for ${selected_gateway}"
    fi

    local confirmation_msg="This will: 1) Rebuild image(s), 2) Remove deployment(s), 3) Deploy service(s)"

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    gum_info "Running all operations in sequence..."

    # Step 1: Rebuild
    if ! run_rebuild "$selected_gateway"; then
        gum_error "Rebuild failed. Aborting."
        return 1
    fi

    gum_info "Waiting 2 seconds before removing..."
    sleep 2

    # Step 2: Remove
    if ! run_remove "$selected_gateway"; then
        gum_warning "Remove skipped or failed. Continuing with deploy..."
    fi

    gum_info "Waiting 2 seconds before deploying..."
    sleep 2

    # Step 3: Deploy
    if ! run_deploy "$selected_gateway"; then
        gum_error "Deploy failed."
        return 1
    fi

    gum_success "All operations completed successfully!"
    return 0
}

main() {
    while true; do
        action=$(gum_action_menu)

        case "$action" in
            "🔨 Rebuild images")
                gateway=$(select_gateway)
                if [[ -n "$gateway" ]]; then
                    run_rebuild "$gateway"
                fi
                ;;
            "🗑️  Remove deployments")
                gateway=$(select_gateway)
                if [[ -n "$gateway" ]]; then
                    run_remove "$gateway"
                fi
                ;;
            "🚀 Deploy services")
                gateway=$(select_gateway)
                if [[ -n "$gateway" ]]; then
                    run_deploy "$gateway"
                fi
                ;;
            "🔄 Run all (rebuild → remove → deploy)")
                run_all
                ;;
            "❌ Exit"|"")
                gum_info "Goodbye!"
                exit 0
                ;;
            *)
                gum_error "Invalid option"
                sleep 1
                ;;
        esac

        echo ""
        gum_info "Press Enter to continue..."
        read
    done
}

# Check if scripts directory exists
if [[ ! -d "${GATEWAYS_DIR}" ]]; then
    gum_error "Gateways scripts directory not found: ${GATEWAYS_DIR}"
    exit 1
fi

# Check if required scripts exist
REQUIRED_SCRIPTS=("rebuild.sh" "remove.sh" "deploy.sh")
for script in "${REQUIRED_SCRIPTS[@]}"; do
    if [[ ! -f "${GATEWAYS_DIR}/${script}" ]]; then
        gum_error "Required script not found: ${GATEWAYS_DIR}/${script}"
        exit 1
    fi
done

# Run main menu
main
