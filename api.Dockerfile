# Multi-stage build for POGO Community API
FROM node:22-alpine AS base

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy package files from API directory
COPY apps/backend/api/package*.json ./
COPY apps/backend/api/tsconfig.json ./

# Dependencies stage
FROM base AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN pnpm install

# Build stage
FROM dependencies AS build

# Copy source code from API directory
COPY apps/backend/api/src/ ./src/
COPY apps/backend/api/custom.d.ts ./

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

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "require('http').get('http://localhost:8080/api/status', (res) => { process.exit(res.statusCode === 200 ? 0 : 1) })"

# Start the application
CMD ["node", "lib/index.js"]

