# MSSQL Dockerfile for POGO Community Bot Database
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express

# Create directory for initialization scripts
USER root
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# Copy initialization scripts from databases/mssql/init/
COPY databases/mssql/init/ ./init/

# Copy entrypoint script
COPY <<'EOF' ./entrypoint.sh
#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 30s

# Run initialization scripts
echo "Running initialization scripts..."
for script in /usr/src/app/init/*.sql; do
    if [ -f "$script" ]; then
        echo "Executing $script..."
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i "$script"
    fi
done

echo "Database initialization complete"

# Keep SQL Server running in the foreground
wait
EOF

RUN chmod +x ./entrypoint.sh

# Expose MSSQL port
EXPOSE 1433

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" || exit 1

# Run entrypoint script
ENTRYPOINT ["./entrypoint.sh"]

