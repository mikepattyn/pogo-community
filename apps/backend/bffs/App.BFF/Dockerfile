# Use the official .NET 10 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 6002
EXPOSE 6002

# Use the official .NET 10 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["apps/backend/bffs/App.BFF/App.BFF.csproj", "apps/backend/bffs/App.BFF/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Kernel/Pogo.Shared.Kernel.csproj", "packages/dotnet-shared/Pogo.Shared.Kernel/"]
COPY ["packages/dotnet-shared/Pogo.Shared.API/Pogo.Shared.API.csproj", "packages/dotnet-shared/Pogo.Shared.API/"]

# Restore dependencies
RUN dotnet restore "apps/backend/bffs/App.BFF/App.BFF.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/apps/backend/bffs/App.BFF"
RUN dotnet build "App.BFF.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "App.BFF.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:6002
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "App.BFF.dll"]
