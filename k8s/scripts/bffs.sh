#!/bin/bash
# Interactive script to manage BFFs: rebuild, remove, and deploy

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BFFS_DIR="${SCRIPT_DIR}/bffs"
source "${SCRIPT_DIR}/lib/gum-utils.sh"

check_gum_installed

# BFFs configuration
BFF_NAMES=(
    "📱 app-bff"
    "🤖 bot-bff"
)

BFF_NAMES_PLAIN=(
    "app-bff"
    "bot-bff"
)

# Function to get directory name for a BFF
get_bff_dir() {
    case "$1" in
        "app-bff")
            echo "App.BFF"
            ;;
        "bot-bff")
            echo "Bot.BFF"
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

select_bff() {
    local selected=$(gum choose --header "Select BFF" "${BFF_NAMES[@]}" "🌟 All BFFs")

    if [[ "$selected" == "🌟 All BFFs" ]]; then
        echo "all"
    elif [[ -n "$selected" ]]; then
        get_plain_name "$selected"
    fi
}

rebuild_single() {
    local bff=$1
    local bff_dir=$(get_bff_dir "$bff")

    if [[ -z "$bff_dir" ]]; then
        gum_error "Unknown BFF: ${bff}"
        return 1
    fi

    configure_docker

    if gum_spinner "Rebuilding ${bff} image..." \
        docker build -t "pogo/${bff}:latest" \
        -f "apps/backend/bffs/${bff_dir}/Dockerfile" .; then
        gum_success "${bff} image rebuilt successfully!"
        return 0
    else
        gum_error "Failed to rebuild ${bff} image"
        return 1
    fi
}

remove_single() {
    local bff=$1

    if [[ -z "$bff" ]]; then
        gum_error "Unknown BFF: ${bff}"
        return 1
    fi

    gum_info "Removing ${bff} deployment..."

    kubectl delete deployment "${bff}" -n pogo-system --ignore-not-found=true

    gum_spinner "Waiting for pod to be removed..." \
        kubectl wait --for=delete pod -l app="${bff}" -n pogo-system --timeout=60s || true

    gum_success "${bff} pods removed!"
    return 0
}

deploy_single() {
    local bff=$1

    if [[ -z "$bff" ]]; then
        gum_error "Unknown BFF: ${bff}"
        return 1
    fi

    gum_info "Deploying ${bff}..."

    kubectl apply -f "k8s/bffs/${bff}-deployment.yaml"

    if gum_spinner "Waiting for deployment to be ready..." \
        kubectl wait --for=condition=available "deployment/${bff}" -n pogo-system --timeout=300s; then
        gum_success "${bff} deployed successfully!"
        return 0
    else
        gum_error "Failed to deploy ${bff}"
        return 1
    fi
}

run_rebuild() {
    local bff=$1

    if [[ -n "$bff" && "$bff" != "all" ]]; then
        rebuild_single "$bff"
        return $?
    fi

    gum_info "Running rebuild script..."
    bash "${BFFS_DIR}/rebuild.sh"
    return $?
}

run_remove() {
    local bff=$1
    local confirmation_msg="This will delete all BFF deployments from the cluster."

    if [[ -n "$bff" && "$bff" != "all" ]]; then
        confirmation_msg="This will delete the ${bff} deployment from the cluster."
    fi

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    if [[ -n "$bff" && "$bff" != "all" ]]; then
        remove_single "$bff"
        return $?
    fi

    gum_info "Running remove script..."
    bash "${BFFS_DIR}/remove.sh"
    return $?
}

run_deploy() {
    local bff=$1

    if [[ -n "$bff" && "$bff" != "all" ]]; then
        deploy_single "$bff"
        return $?
    fi

    gum_info "Running deploy script..."
    bash "${BFFS_DIR}/deploy.sh"
    return $?
}

run_all() {
    local selected_bff=$(select_bff)

    if [[ -z "$selected_bff" ]]; then
        gum_error "Invalid selection"
        return 1
    fi

    local action_desc="Run all operations"
    if [[ "$selected_bff" == "all" ]]; then
        action_desc="Run all operations for all BFFs"
    else
        action_desc="Run all operations for ${selected_bff}"
    fi

    local confirmation_msg="This will: 1) Rebuild image(s), 2) Remove deployment(s), 3) Deploy service(s)"

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    gum_info "Running all operations in sequence..."

    # Step 1: Rebuild
    if ! run_rebuild "$selected_bff"; then
        gum_error "Rebuild failed. Aborting."
        return 1
    fi

    gum_info "Waiting 2 seconds before removing..."
    sleep 2

    # Step 2: Remove
    if ! run_remove "$selected_bff"; then
        gum_warning "Remove skipped or failed. Continuing with deploy..."
    fi

    gum_info "Waiting 2 seconds before deploying..."
    sleep 2

    # Step 3: Deploy
    if ! run_deploy "$selected_bff"; then
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
                bff=$(select_bff)
                if [[ -n "$bff" ]]; then
                    run_rebuild "$bff"
                fi
                ;;
            "🗑️  Remove deployments")
                bff=$(select_bff)
                if [[ -n "$bff" ]]; then
                    run_remove "$bff"
                fi
                ;;
            "🚀 Deploy services")
                bff=$(select_bff)
                if [[ -n "$bff" ]]; then
                    run_deploy "$bff"
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
if [[ ! -d "${BFFS_DIR}" ]]; then
    gum_error "BFFs scripts directory not found: ${BFFS_DIR}"
    exit 1
fi

# Check if required scripts exist
REQUIRED_SCRIPTS=("rebuild.sh" "remove.sh" "deploy.sh")
for script in "${REQUIRED_SCRIPTS[@]}"; do
    if [[ ! -f "${BFFS_DIR}/${script}" ]]; then
        gum_error "Required script not found: ${BFFS_DIR}/${script}"
        exit 1
    fi
done

# Run main menu
main
