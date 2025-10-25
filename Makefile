.PHONY: help install clean build dev test lint lint-fix
.PHONY: build-api build-bot build-mobile
.PHONY: run-api run-bot run-mobile-web run-mobile-android run-mobile-ios
.PHONY: test-api test-bot test-mobile
.PHONY: lint-api lint-bot lint-mobile
.PHONY: docker-build docker-build-api docker-build-bot docker-build-app docker-build-mysql docker-build-mssql
.PHONY: docker-up docker-up-build docker-down docker-down-volumes docker-restart
.PHONY: docker-restart-api docker-restart-bot docker-restart-app
.PHONY: docker-logs docker-logs-api docker-logs-bot docker-logs-app docker-logs-mysql docker-logs-mssql
.PHONY: docker-ps docker-status docker-db-mysql docker-db-mssql
.PHONY: docker-clean docker-clean-all

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

##@ Docker - Build

docker-build: ## Build all Docker images
	@echo "$(CYAN)Building all Docker images...$(NC)"
	docker-compose build
	@echo "$(GREEN)All images built successfully!$(NC)"

docker-build-api: ## Build API Docker image
	@echo "$(CYAN)Building API image...$(NC)"
	docker-compose build api
	@echo "$(GREEN)API image built successfully!$(NC)"

docker-build-bot: ## Build bot Docker image
	@echo "$(CYAN)Building bot image...$(NC)"
	docker-compose build bot
	@echo "$(GREEN)Bot image built successfully!$(NC)"

docker-build-app: ## Build mobile app Docker image
	@echo "$(CYAN)Building app image...$(NC)"
	docker-compose build app
	@echo "$(GREEN)App image built successfully!$(NC)"

docker-build-mysql: ## Build MySQL Docker image
	@echo "$(CYAN)Building MySQL image...$(NC)"
	docker-compose build mysql
	@echo "$(GREEN)MySQL image built successfully!$(NC)"

docker-build-mssql: ## Build MSSQL Docker image
	@echo "$(CYAN)Building MSSQL image...$(NC)"
	docker-compose build mssql
	@echo "$(GREEN)MSSQL image built successfully!$(NC)"

##@ Docker - Run

docker-up: ## Start all services
	@echo "$(CYAN)Starting all services...$(NC)"
	docker-compose up -d
	@echo "$(GREEN)All services started!$(NC)"
	@echo "$(YELLOW)API: http://localhost:1000$(NC)"
	@echo "$(YELLOW)App: http://localhost:3000$(NC)"
	@echo "$(YELLOW)MySQL: localhost:4000$(NC)"
	@echo "$(YELLOW)MSSQL: localhost:5000$(NC)"

docker-up-build: ## Build and start all services
	@echo "$(CYAN)Building and starting all services...$(NC)"
	docker-compose up -d --build
	@echo "$(GREEN)All services built and started!$(NC)"

docker-down: ## Stop all services
	@echo "$(CYAN)Stopping all services...$(NC)"
	docker-compose down
	@echo "$(GREEN)All services stopped!$(NC)"

docker-down-volumes: ## Stop all services and remove volumes
	@echo "$(RED)WARNING: This will delete all database data!$(NC)"
	@read -p "Are you sure? [y/N] " -n 1 -r; \
	echo; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker-compose down -v; \
		echo "$(GREEN)All services stopped and volumes removed!$(NC)"; \
	else \
		echo "$(YELLOW)Cancelled.$(NC)"; \
	fi

docker-restart: ## Restart all services
	@echo "$(CYAN)Restarting all services...$(NC)"
	docker-compose restart
	@echo "$(GREEN)All services restarted!$(NC)"

docker-restart-api: ## Restart API service
	@echo "$(CYAN)Restarting API...$(NC)"
	docker-compose restart api
	@echo "$(GREEN)API restarted!$(NC)"

docker-restart-bot: ## Restart bot service
	@echo "$(CYAN)Restarting bot...$(NC)"
	docker-compose restart bot
	@echo "$(GREEN)Bot restarted!$(NC)"

docker-restart-app: ## Restart app service
	@echo "$(CYAN)Restarting app...$(NC)"
	docker-compose restart app
	@echo "$(GREEN)App restarted!$(NC)"

##@ Docker - Logs

docker-logs: ## View logs for all services
	@docker-compose logs -f

docker-logs-api: ## View API logs
	@docker-compose logs -f api

docker-logs-bot: ## View bot logs
	@docker-compose logs -f bot

docker-logs-app: ## View app logs
	@docker-compose logs -f app

docker-logs-mysql: ## View MySQL logs
	@docker-compose logs -f mysql

docker-logs-mssql: ## View MSSQL logs
	@docker-compose logs -f mssql

##@ Docker - Status

docker-ps: ## Show running containers
	@docker-compose ps

docker-status: ## Show service status with health checks
	@echo "$(CYAN)Service Status:$(NC)"
	@docker-compose ps
	@echo ""
	@echo "$(CYAN)Health Checks:$(NC)"
	@docker ps --filter "name=pogo-" --format "table {{.Names}}\t{{.Status}}"

##@ Docker - Database

docker-db-mysql: ## Connect to MySQL database
	@echo "$(CYAN)Connecting to MySQL...$(NC)"
	@echo "$(YELLOW)Password: value of MYSQL_ROOT_PASSWORD from .env$(NC)"
	@docker-compose exec mysql mysql -u root -p

docker-db-mssql: ## Connect to MSSQL database
	@echo "$(CYAN)Connecting to MSSQL...$(NC)"
	@echo "$(YELLOW)Use password from MSSQL_SA_PASSWORD in .env$(NC)"
	@docker-compose exec mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa

##@ Docker - Cleanup

docker-clean: ## Remove stopped containers and unused images
	@echo "$(CYAN)Cleaning up Docker resources...$(NC)"
	docker-compose down --remove-orphans
	docker system prune -f
	@echo "$(GREEN)Cleanup complete!$(NC)"

docker-clean-all: ## Remove all Docker resources (containers, images, volumes)
	@echo "$(RED)WARNING: This will remove ALL Docker containers, images, and volumes!$(NC)"
	@read -p "Are you sure? [y/N] " -n 1 -r; \
	echo; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker-compose down -v --rmi all; \
		docker system prune -af --volumes; \
		echo "$(GREEN)All Docker resources removed!$(NC)"; \
	else \
		echo "$(YELLOW)Cancelled.$(NC)"; \
	fi

