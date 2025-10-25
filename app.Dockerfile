# Multi-stage build for POGO Community Mobile Web App
FROM node:22-alpine AS base

# Install pnpm and turbo
RUN npm install -g pnpm turbo

# Set working directory
WORKDIR /app

# Copy workspace files for turbo prune
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml turbo.json ./
COPY apps/ ./apps/

# Prune workspace for Mobile App
RUN turbo prune --scope=@pogo/mobile --docker

# Pruned workspace stage
FROM node:22-alpine AS pruned

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy pruned workspace
COPY --from=base /app/out/json/ ./
COPY --from=base /app/out/full/apps/frontend/mobile/ ./apps/frontend/mobile/
COPY --from=base /app/out/full/turbo.json ./

# Dependencies stage
FROM pruned AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN pnpm install --frozen-lockfile --ignore-scripts

# Build stage
FROM dependencies AS build

# Export web build using Expo
RUN cd apps/frontend/mobile && pnpm exec expo export --platform web

# Run stage with nginx
FROM nginx:alpine AS run

# Copy custom nginx configuration
COPY <<EOF /etc/nginx/conf.d/default.conf
server {
    listen 3000;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files \$uri \$uri/ /index.html;
    }

    # Enable gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/x-javascript application/xml+rss application/javascript application/json;

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
EOF

# Copy built web files from build stage
COPY --from=build /app/apps/frontend/mobile/dist /usr/share/nginx/html

# Expose port
EXPOSE 3000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:3000 || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]

