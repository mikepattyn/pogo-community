.PHONY: help install clean build dev test lint lint-fix
.PHONY: build-api build-bot build-mobile
.PHONY: run-api run-bot run-mobile-web run-mobile-android run-mobile-ios
.PHONY: test-api test-bot test-mobile
.PHONY: lint-api lint-bot lint-mobile

# Default target
.DEFAULT_GOAL := help

# Colors for output
CYAN := \033[0;36m
GREEN := \033[0;32m
YELLOW := \033[0;33m
RED := \033[0;31m
NC := \033[0m # No Color

##@ General

help: ## Display this help message
	@echo "$(CYAN)POGO Community Monorepo - Available Commands$(NC)"
	@echo ""
	@awk 'BEGIN {FS = ":.*##"; printf ""} /^[a-zA-Z_-]+:.*?##/ { printf "  $(GREEN)%-20s$(NC) %s\n", $$1, $$2 } /^##@/ { printf "\n$(YELLOW)%s$(NC)\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

install: ## Install all dependencies
	@echo "$(CYAN)Installing dependencies...$(NC)"
	pnpm install

clean: ## Clean build artifacts and dependencies
	@echo "$(CYAN)Cleaning build artifacts...$(NC)"
	pnpm run clean
	@echo "$(GREEN)Clean complete!$(NC)"

##@ Build

build: ## Build all apps
	@echo "$(CYAN)Building all apps...$(NC)"
	pnpm run build
	@echo "$(GREEN)Build complete!$(NC)"

build-api: ## Build API backend
	@echo "$(CYAN)Building API...$(NC)"
	pnpm --filter @pogo/api run build
	@echo "$(GREEN)API build complete!$(NC)"

build-bot: ## Build Discord bot
	@echo "$(CYAN)Building bot...$(NC)"
	pnpm --filter @pogo/bot run build
	@echo "$(GREEN)Bot build complete!$(NC)"

build-mobile: ## Build mobile app (web)
	@echo "$(CYAN)Building mobile app...$(NC)"
	pnpm --filter @pogo/mobile run build:web
	@echo "$(GREEN)Mobile build complete!$(NC)"

##@ Development

dev: ## Run all apps in development mode
	@echo "$(CYAN)Starting all apps in development mode...$(NC)"
	pnpm run dev

run-api: build-api ## Build and run API backend
	@echo "$(CYAN)Starting API...$(NC)"
	pnpm --filter @pogo/api run start

run-bot: build-bot ## Build and run Discord bot
	@echo "$(CYAN)Starting bot...$(NC)"
	pnpm --filter @pogo/bot run start

run-mobile-web: ## Run mobile app (web)
	@echo "$(CYAN)Starting mobile app (web)...$(NC)"
	pnpm --filter @pogo/mobile run web

run-mobile-android: ## Run mobile app (Android)
	@echo "$(CYAN)Starting mobile app (Android)...$(NC)"
	pnpm --filter @pogo/mobile run android

run-mobile-ios: ## Run mobile app (iOS)
	@echo "$(CYAN)Starting mobile app (iOS)...$(NC)"
	pnpm --filter @pogo/mobile run ios

##@ Testing

test: ## Run tests for all apps
	@echo "$(CYAN)Running tests...$(NC)"
	pnpm run test
	@echo "$(GREEN)Tests complete!$(NC)"

test-api: ## Run API tests
	@echo "$(CYAN)Running API tests...$(NC)"
	pnpm --filter @pogo/api run test

test-bot: ## Run bot tests
	@echo "$(CYAN)Running bot tests...$(NC)"
	pnpm --filter @pogo/bot run test

test-mobile: ## Run mobile tests
	@echo "$(CYAN)Running mobile tests...$(NC)"
	pnpm --filter @pogo/mobile run test

##@ Linting

lint: ## Lint all apps
	@echo "$(CYAN)Linting all apps...$(NC)"
	pnpm run lint
	@echo "$(GREEN)Linting complete!$(NC)"

lint-fix: ## Lint and auto-fix all apps
	@echo "$(CYAN)Linting and fixing all apps...$(NC)"
	pnpm run lint:fix
	@echo "$(GREEN)Linting and fixing complete!$(NC)"

lint-api: ## Lint API backend
	@echo "$(CYAN)Linting API...$(NC)"
	pnpm --filter @pogo/api run lint

lint-bot: ## Lint Discord bot
	@echo "$(CYAN)Linting bot...$(NC)"
	pnpm --filter @pogo/bot run lint

lint-mobile: ## Lint mobile app
	@echo "$(CYAN)Linting mobile...$(NC)"
	pnpm --filter @pogo/mobile run lint

##@ Type Checking

type-check: ## Run TypeScript type checking for all apps
	@echo "$(CYAN)Running type checks...$(NC)"
	pnpm run type-check
	@echo "$(GREEN)Type checking complete!$(NC)"

