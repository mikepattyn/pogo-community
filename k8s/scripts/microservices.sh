#!/bin/bash
# Interactive script to manage microservices: rebuild, remove, and deploy

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MICROSERVICES_DIR="${SCRIPT_DIR}/microservices"
source "${SCRIPT_DIR}/lib/gum-utils.sh"

check_gum_installed

# Microservices configuration
MICROSERVICE_NAMES=(
    "📦 account-service"
    "👤 player-service"
    "📍 location-service"
    "🏋️  gym-service"
    "🎯 raid-service"
    "🤖 ocr-service"
)

MICROSERVICE_NAMES_PLAIN=(
    "account-service"
    "player-service"
    "location-service"
    "gym-service"
    "raid-service"
    "ocr-service"
)

# Function to get directory name for a service
get_service_dir() {
    case "$1" in
        "account-service")
            echo "Account.Service"
            ;;
        "player-service")
            echo "Player.Service"
            ;;
        "location-service")
            echo "Location.Service"
            ;;
        "gym-service")
            echo "Gym.Service"
            ;;
        "raid-service")
            echo "Raid.Service"
            ;;
        "ocr-service")
            echo "OCR.Service"
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

select_microservice() {
    local selected=$(gum choose --header "Select Microservice" "${MICROSERVICE_NAMES[@]}" "🌟 All microservices")

    if [[ "$selected" == "🌟 All microservices" ]]; then
        echo "all"
    elif [[ -n "$selected" ]]; then
        get_plain_name "$selected"
    fi
}

rebuild_single() {
    local service=$1
    local service_dir=$(get_service_dir "$service")

    if [[ -z "$service_dir" ]]; then
        gum_error "Unknown microservice: ${service}"
        return 1
    fi

    configure_docker

    if gum_spinner "Rebuilding ${service} image..." \
        docker build -t "pogo/${service}:latest" \
        -f "apps/backend/microservices/${service_dir}/Dockerfile" .; then
        gum_success "${service} image rebuilt successfully!"
        return 0
    else
        gum_error "Failed to rebuild ${service} image"
        return 1
    fi
}

remove_single() {
    local service=$1

    if [[ -z "$service" ]]; then
        gum_error "Unknown microservice: ${service}"
        return 1
    fi

    gum_info "Removing ${service} deployment..."

    kubectl delete deployment "${service}" -n pogo-system --ignore-not-found=true

    gum_spinner "Waiting for pod to be removed..." \
        kubectl wait --for=delete pod -l app="${service}" -n pogo-system --timeout=60s || true

    gum_success "${service} pods removed!"
    return 0
}

deploy_single() {
    local service=$1

    if [[ -z "$service" ]]; then
        gum_error "Unknown microservice: ${service}"
        return 1
    fi

    gum_info "Deploying ${service}..."

    kubectl apply -f "k8s/microservices/${service}-deployment.yaml"

    if gum_spinner "Waiting for deployment to be ready..." \
        kubectl wait --for=condition=available "deployment/${service}" -n pogo-system --timeout=300s; then
        gum_success "${service} deployed successfully!"
        return 0
    else
        gum_error "Failed to deploy ${service}"
        return 1
    fi
}

run_rebuild() {
    local service=$1

    if [[ -n "$service" && "$service" != "all" ]]; then
        rebuild_single "$service"
        return $?
    fi

    gum_info "Running rebuild script..."
    bash "${MICROSERVICES_DIR}/rebuild.sh"
    return $?
}

run_remove() {
    local service=$1
    local confirmation_msg="This will delete all microservice deployments from the cluster."

    if [[ -n "$service" && "$service" != "all" ]]; then
        confirmation_msg="This will delete the ${service} deployment from the cluster."
    fi

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    if [[ -n "$service" && "$service" != "all" ]]; then
        remove_single "$service"
        return $?
    fi

    gum_info "Running remove script..."
    bash "${MICROSERVICES_DIR}/remove.sh"
    return $?
}

run_deploy() {
    local service=$1

    if [[ -n "$service" && "$service" != "all" ]]; then
        deploy_single "$service"
        return $?
    fi

    gum_info "Running deploy script..."
    bash "${MICROSERVICES_DIR}/deploy.sh"
    return $?
}

run_all() {
    local selected_service=$(select_microservice)

    if [[ -z "$selected_service" ]]; then
        gum_error "Invalid selection"
        return 1
    fi

    local action_desc="Run all operations"
    if [[ "$selected_service" == "all" ]]; then
        action_desc="Run all operations for all microservices"
    else
        action_desc="Run all operations for ${selected_service}"
    fi

    local confirmation_msg="This will: 1) Rebuild image(s), 2) Remove deployment(s), 3) Deploy service(s)"

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    gum_info "Running all operations in sequence..."

    # Step 1: Rebuild
    if ! run_rebuild "$selected_service"; then
        gum_error "Rebuild failed. Aborting."
        return 1
    fi

    gum_info "Waiting 2 seconds before removing..."
    sleep 2

    # Step 2: Remove
    if ! run_remove "$selected_service"; then
        gum_warning "Remove skipped or failed. Continuing with deploy..."
    fi

    gum_info "Waiting 2 seconds before deploying..."
    sleep 2

    # Step 3: Deploy
    if ! run_deploy "$selected_service"; then
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
                service=$(select_microservice)
                if [[ -n "$service" ]]; then
                    run_rebuild "$service"
                fi
                ;;
            "🗑️  Remove deployments")
                service=$(select_microservice)
                if [[ -n "$service" ]]; then
                    run_remove "$service"
                fi
                ;;
            "🚀 Deploy services")
                service=$(select_microservice)
                if [[ -n "$service" ]]; then
                    run_deploy "$service"
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
if [[ ! -d "${MICROSERVICES_DIR}" ]]; then
    gum_error "Microservices scripts directory not found: ${MICROSERVICES_DIR}"
    exit 1
fi

# Check if required scripts exist
REQUIRED_SCRIPTS=("rebuild.sh" "remove.sh" "deploy.sh")
for script in "${REQUIRED_SCRIPTS[@]}"; do
    if [[ ! -f "${MICROSERVICES_DIR}/${script}" ]]; then
        gum_error "Required script not found: ${MICROSERVICES_DIR}/${script}"
        exit 1
    fi
done

# Run main menu
main
