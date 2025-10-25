# MySQL Dockerfile for POGO Community API Database
FROM mysql:8.0

# Set environment variables for MySQL
ENV MYSQL_DATABASE=pogo_api

# Copy initialization scripts
COPY init/*.sql /docker-entrypoint-initdb.d/

# Expose MySQL port
EXPOSE 3306

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
  CMD mysqladmin ping -h localhost -u root -p$MYSQL_ROOT_PASSWORD || exit 1

