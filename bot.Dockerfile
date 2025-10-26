# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 2000

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["apps/frontend/bot/Bot.Service.csproj", "apps/frontend/bot/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Kernel/Pogo.Shared.Kernel.csproj", "packages/dotnet-shared/Pogo.Shared.Kernel/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Infrastructure/Pogo.Shared.Infrastructure.csproj", "packages/dotnet-shared/Pogo.Shared.Infrastructure/"]
COPY ["packages/dotnet-shared/Pogo.Shared.Application/Pogo.Shared.Application.csproj", "packages/dotnet-shared/Pogo.Shared.Application/"]
COPY ["packages/dotnet-shared/Pogo.Shared.API/Pogo.Shared.API.csproj", "packages/dotnet-shared/Pogo.Shared.API/"]

# Restore dependencies
RUN dotnet restore "apps/frontend/bot/Bot.Service.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/apps/frontend/bot"
RUN dotnet build "Bot.Service.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Bot.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:2000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Bot.Service.dll"]