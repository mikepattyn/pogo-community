#!/bin/bash
# Interactive script to manage apps: rebuild, remove, and deploy

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APPS_DIR="${SCRIPT_DIR}/apps"
source "${SCRIPT_DIR}/lib/gum-utils.sh"

check_gum_installed

# Apps configuration
APP_NAMES=(
    "🤖 pogo-bot"
)

APP_NAMES_PLAIN=(
    "pogo-bot"
)

# Function to get Dockerfile path for an app
get_app_dockerfile() {
    case "$1" in
        "pogo-bot")
            echo "bot.Dockerfile"
            ;;
        *)
            echo ""
            ;;
    esac
}

# Function to get deployment name for an app
get_app_deployment() {
    case "$1" in
        "pogo-bot")
            echo "pogo-bot"
            ;;
        *)
            echo ""
            ;;
    esac
}

# Function to get deployment YAML filename for an app
get_app_deployment_file() {
    case "$1" in
        "pogo-bot")
            echo "bot-deployment.yaml"
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

select_app() {
    local selected=$(gum choose --header "Select App" "${APP_NAMES[@]}" "🌟 All apps")

    if [[ "$selected" == "🌟 All apps" ]]; then
        echo "all"
    elif [[ -n "$selected" ]]; then
        get_plain_name "$selected"
    fi
}

rebuild_single() {
    local app=$1
    local dockerfile=$(get_app_dockerfile "$app")
    local image_name="pogo/bot"

    if [[ "$app" == "pogo-bot" ]]; then
        image_name="pogo/bot"
    fi

    if [[ -z "$dockerfile" ]]; then
        gum_error "Unknown app: ${app}"
        return 1
    fi

    configure_docker

    if gum_spinner "Rebuilding ${app} image..." \
        docker build -t "${image_name}:latest" \
        -f "${dockerfile}" .; then
        gum_success "${app} image rebuilt successfully!"
        return 0
    else
        gum_error "Failed to rebuild ${app} image"
        return 1
    fi
}

remove_single() {
    local app=$1
    local deployment=$(get_app_deployment "$app")

    if [[ -z "$deployment" ]]; then
        gum_error "Unknown app: ${app}"
        return 1
    fi

    gum_info "Removing ${deployment} deployment..."

    kubectl delete deployment "${deployment}" -n pogo-system --ignore-not-found=true

    gum_spinner "Waiting for pod to be removed..." \
        kubectl wait --for=delete pod -l app="${deployment}" -n pogo-system --timeout=60s || true

    gum_success "${deployment} pods removed!"
    return 0
}

deploy_single() {
    local app=$1
    local deployment=$(get_app_deployment "$app")
    local deployment_file=$(get_app_deployment_file "$app")

    if [[ -z "$deployment" ]]; then
        gum_error "Unknown app: ${app}"
        return 1
    fi

    gum_info "Deploying ${deployment}..."

    kubectl apply -f "k8s/apps/${deployment_file}"

    gum_info "Waiting for deployment to be created..."
    sleep 2

    if gum_spinner "Waiting for deployment to be ready..." \
        kubectl wait --for=condition=available "deployment/${deployment}" -n pogo-system --timeout=300s; then
        gum_success "${deployment} deployed successfully!"
        return 0
    else
        gum_error "Failed to deploy ${deployment}"
        return 1
    fi
}

run_rebuild() {
    local app=$1

    if [[ -n "$app" && "$app" != "all" ]]; then
        rebuild_single "$app"
        return $?
    fi

    gum_info "Running rebuild script..."
    bash "${APPS_DIR}/rebuild.sh"
    return $?
}

run_remove() {
    local app=$1
    local confirmation_msg="This will delete all app deployments from the cluster."

    if [[ -n "$app" && "$app" != "all" ]]; then
        confirmation_msg="This will delete the ${app} deployment from the cluster."
    fi

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    if [[ -n "$app" && "$app" != "all" ]]; then
        remove_single "$app"
        return $?
    fi

    gum_info "Running remove script..."
    bash "${APPS_DIR}/remove.sh"
    return $?
}

run_deploy() {
    local app=$1

    if [[ -n "$app" && "$app" != "all" ]]; then
        deploy_single "$app"
        return $?
    fi

    gum_info "Running deploy script..."
    bash "${APPS_DIR}/deploy.sh"
    return $?
}

run_all() {
    local selected_app=$(select_app)

    if [[ -z "$selected_app" ]]; then
        gum_error "Invalid selection"
        return 1
    fi

    local action_desc="Run all operations"
    if [[ "$selected_app" == "all" ]]; then
        action_desc="Run all operations for all apps"
    else
        action_desc="Run all operations for ${selected_app}"
    fi

    local confirmation_msg="This will: 1) Rebuild image(s), 2) Remove deployment(s), 3) Deploy service(s)"

    if ! gum_confirm "$confirmation_msg"; then
        gum_info "Action cancelled"
        return 1
    fi

    gum_info "Running all operations in sequence..."

    # Step 1: Rebuild
    if ! run_rebuild "$selected_app"; then
        gum_error "Rebuild failed. Aborting."
        return 1
    fi

    gum_info "Waiting 2 seconds before removing..."
    sleep 2

    # Step 2: Remove
    if ! run_remove "$selected_app"; then
        gum_warning "Remove skipped or failed. Continuing with deploy..."
    fi

    gum_info "Waiting 2 seconds before deploying..."
    sleep 2

    # Step 3: Deploy
    if ! run_deploy "$selected_app"; then
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
                app=$(select_app)
                if [[ -n "$app" ]]; then
                    run_rebuild "$app"
                fi
                ;;
            "🗑️  Remove deployments")
                app=$(select_app)
                if [[ -n "$app" ]]; then
                    run_remove "$app"
                fi
                ;;
            "🚀 Deploy services")
                app=$(select_app)
                if [[ -n "$app" ]]; then
                    run_deploy "$app"
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
if [[ ! -d "${APPS_DIR}" ]]; then
    gum_error "Apps scripts directory not found: ${APPS_DIR}"
    exit 1
fi

# Check if required scripts exist
REQUIRED_SCRIPTS=("rebuild.sh" "remove.sh" "deploy.sh")
for script in "${REQUIRED_SCRIPTS[@]}"; do
    if [[ ! -f "${APPS_DIR}/${script}" ]]; then
        gum_error "Required script not found: ${APPS_DIR}/${script}"
        exit 1
    fi
done

# Run main menu
main
