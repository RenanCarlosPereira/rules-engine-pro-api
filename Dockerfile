# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

# Render requires apps to bind to this dynamic port
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config and local packages FIRST
COPY NuGet.config ./
COPY .local-packages/ ./.local-packages/

# Copy and restore dependencies
COPY ["src/rules-engine-api/RulesEngine.Api.csproj", "src/rules-engine-api/"]
RUN dotnet restore "src/rules-engine-api/RulesEngine.Api.csproj"

# Copy the rest of the code
COPY . .

# Build the app
WORKDIR "/src/src/rules-engine-api"
RUN dotnet build "RulesEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RulesEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RulesEngine.Api.dll"]
