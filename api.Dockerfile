# Multi-stage build for POGO Community API
FROM node:22-alpine AS base

# Install pnpm and turbo
RUN npm install -g pnpm turbo

# Set working directory
WORKDIR /app

# Copy workspace files for turbo prune
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml turbo.json ./
COPY apps/ ./apps/

# Prune workspace for API
RUN turbo prune --scope=@pogo/api --docker

# Pruned workspace stage
FROM node:22-alpine AS pruned

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy pruned workspace
COPY --from=base /app/out/json/ ./
COPY --from=base /app/out/full/apps/backend/api/ ./apps/backend/api/
COPY --from=base /app/out/full/turbo.json ./

# Dependencies stage
FROM pruned AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN pnpm install --frozen-lockfile

# Build stage
FROM dependencies AS build

# Build TypeScript
RUN cd apps/backend/api && pnpm run build

# Production dependencies stage
FROM pruned AS production-deps

# Install only production dependencies
RUN pnpm install --prod --frozen-lockfile --ignore-scripts && pnpm store prune

# Run stage
FROM node:22-alpine AS run

# Set working directory
WORKDIR /app

# Copy production dependencies
COPY --from=production-deps /app/node_modules ./node_modules

# Copy built application
COPY --from=build /app/apps/backend/api/lib ./lib
COPY --from=build /app/apps/backend/api/package.json ./package.json

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001

# Change ownership of the app directory
RUN chown -R nodejs:nodejs /app
USER nodejs

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "require('http').get('http://localhost:8080/api/status', (res) => { process.exit(res.statusCode === 200 ? 0 : 1) })"

# Start the application
CMD ["node", "lib/index.js"]

