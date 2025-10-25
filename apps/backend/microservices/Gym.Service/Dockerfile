# Use the official .NET 10 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5004
EXPOSE 5004

# Use the official .NET 10 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["apps/backend/microservices/Gym.Service/Gym.Service.csproj", "apps/backend/microservices/Gym.Service/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Kernel/Pogo.Shared.Kernel.csproj", "packages/dotnet-shared/Pogo.Shared.Kernel/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Infrastructure/Pogo.Shared.Infrastructure.csproj", "packages/dotnet-shared/Pogo.Shared.Infrastructure/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Application/Pogo.Shared.Application.csproj", "packages/dotnet-shared/Pogo.Shared.Application/"]
COPY ["packages/dotnet-shared/Pogo.Shared.API/Pogo.Shared.API.csproj", "packages/dotnet-shared/Pogo.Shared.API/"]

# Restore dependencies
RUN dotnet restore "apps/backend/microservices/Gym.Service/Gym.Service.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/apps/backend/microservices/Gym.Service"
RUN dotnet build "Gym.Service.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Gym.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Gym.Service.dll"]
