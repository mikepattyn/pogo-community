# Multi-stage build for POGO Community Discord Bot
FROM node:22-alpine AS base

# Install pnpm and turbo
RUN npm install -g pnpm turbo

# Set working directory
WORKDIR /app

# Copy workspace files for turbo prune
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml turbo.json ./
COPY apps/ ./apps/

# Prune workspace for Bot
RUN turbo prune --scope=@pogo/bot --docker

# Pruned workspace stage
FROM node:22-alpine AS pruned

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy pruned workspace
COPY --from=base /app/out/json/ ./
COPY --from=base /app/out/full/apps/frontend/bot/ ./apps/frontend/bot/
COPY --from=base /app/out/full/turbo.json ./

# Dependencies stage
FROM pruned AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN pnpm install --frozen-lockfile

# Build stage
FROM dependencies AS build

# Build TypeScript
RUN cd apps/frontend/bot && pnpm run build

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
COPY --from=build /app/apps/frontend/bot/lib ./lib
COPY --from=build /app/apps/frontend/bot/package.json ./package.json

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001

# Change ownership of the app directory
RUN chown -R nodejs:nodejs /app
USER nodejs

# Set environment variables
ENV BOT_BFF_URL=http://bot-bff:6001
ENV NODE_ENV=production

# Expose port for health checks
EXPOSE 2000

# Health check (simple process check since bot doesn't have HTTP endpoint)
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD pgrep -f "node.*bot.js" > /dev/null || exit 1

# Start the application
CMD ["node", "lib/bot.js"]

