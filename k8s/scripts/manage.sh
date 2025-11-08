#!/bin/bash
# Unified main menu for Kubernetes management scripts

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/lib/gum-utils.sh"

check_gum_installed

while true; do
    category=$(gum_menu "POGO Kubernetes Management" \
        "📦 Microservices" \
        "🚪 BFFs" \
        "🚪 Gateways" \
        "📱 Apps" \
        "❌ Exit")

    case "$category" in
        "📦 Microservices")
            bash "${SCRIPT_DIR}/microservices.sh"
            ;;
        "🚪 BFFs")
            bash "${SCRIPT_DIR}/bffs.sh"
            ;;
        "🚪 Gateways")
            bash "${SCRIPT_DIR}/gateways.sh"
            ;;
        "📱 Apps")
            bash "${SCRIPT_DIR}/apps.sh"
            ;;
        "❌ Exit"|"")
            echo ""
            gum_info "Goodbye!"
            exit 0
            ;;
        *)
            gum_error "Invalid selection"
            sleep 1
            ;;
    esac

    echo ""
    gum_info "Press Enter to return to main menu..."
    read
done

