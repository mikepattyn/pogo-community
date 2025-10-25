# Multi-stage build for POGO Community Discord Bot
FROM node:22-alpine AS base

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy package files from bot directory
COPY apps/frontend/bot/package*.json ./
COPY apps/frontend/bot/tsconfig.json ./

# Dependencies stage
FROM base AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN pnpm install

# Build stage
FROM dependencies AS build

# Copy source code from bot directory
COPY apps/frontend/bot/src/ ./src/

# Build TypeScript
RUN pnpm run build

# Production dependencies stage
FROM base AS production-deps

# Install only production dependencies
RUN pnpm install --prod && pnpm store prune

# Run stage
FROM node:22-alpine AS run

# Set working directory
WORKDIR /app

# Copy production dependencies
COPY --from=production-deps /app/node_modules ./node_modules

# Copy built application
COPY --from=build /app/lib ./lib
COPY --from=build /app/package*.json ./

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001

# Change ownership of the app directory
RUN chown -R nodejs:nodejs /app
USER nodejs

# Expose port for health checks
EXPOSE 2000

# Health check (simple process check since bot doesn't have HTTP endpoint)
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD pgrep -f "node.*bot.js" > /dev/null || exit 1

# Start the application
CMD ["node", "lib/bot.js"]

