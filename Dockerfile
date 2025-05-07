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


# Copy and restore dependencies
COPY ["src/RulesEngine.Api/RulesEngine.Api.csproj", "src/RulesEngine.Api/"]
RUN dotnet restore "src/RulesEngine.Api/RulesEngine.Api.csproj"

# Copy the rest of the code
COPY . .

# Build the app
WORKDIR "/src/src/RulesEngine.Api"
RUN dotnet build "RulesEngine.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RulesEngine.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Final stage for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RulesEngine.Api.dll"]
