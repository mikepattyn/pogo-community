# Multi-stage build for POGO Community Mobile Web App
FROM node:18-alpine AS base

# Set working directory
WORKDIR /app

# Copy package files from mobile directory
COPY apps/frontend/mobile/package*.json ./
COPY apps/frontend/mobile/tsconfig.json ./

# Dependencies stage
FROM base AS dependencies

# Install all dependencies (including dev dependencies for build)
RUN npm install --legacy-peer-deps

# Build stage
FROM dependencies AS build

# Copy source code and configuration from mobile directory
COPY apps/frontend/mobile/App.tsx ./
COPY apps/frontend/mobile/index.js ./
COPY apps/frontend/mobile/app.json ./
COPY apps/frontend/mobile/babel.config.js ./
COPY apps/frontend/mobile/metro.config.js ./
COPY apps/frontend/mobile/inversify.config.ts ./
COPY apps/frontend/mobile/react-native/ ./react-native/

# Export web build using Expo
RUN npx expo export --platform web

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
COPY --from=build /app/dist /usr/share/nginx/html

# Expose port
EXPOSE 3000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:3000 || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]

