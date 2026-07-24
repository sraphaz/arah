# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copiar solution e csproj (para cache de layers)
COPY Arah.sln ./
COPY backend/Arah.Domain/*.csproj backend/Arah.Domain/
COPY backend/Arah.Shared/*.csproj backend/Arah.Shared/
COPY backend/Arah.Application/*.csproj backend/Arah.Application/
COPY backend/Arah.Infrastructure/*.csproj backend/Arah.Infrastructure/
COPY backend/Arah.Infrastructure.Shared/*.csproj backend/Arah.Infrastructure.Shared/
COPY backend/Arah.Api/*.csproj backend/Arah.Api/
COPY backend/Arah.Modules.Admin.Infrastructure/*.csproj backend/Arah.Modules.Admin.Infrastructure/
COPY backend/Arah.Modules.Alerts/*.csproj backend/Arah.Modules.Alerts/
COPY backend/Arah.Modules.Assets/*.csproj backend/Arah.Modules.Assets/
COPY backend/Arah.Modules.Chat/*.csproj backend/Arah.Modules.Chat/
COPY backend/Arah.Modules.Connections/*.csproj backend/Arah.Modules.Connections/
COPY backend/Arah.Modules.Events/*.csproj backend/Arah.Modules.Events/
COPY backend/Arah.Modules.Feed/*.csproj backend/Arah.Modules.Feed/
COPY backend/Arah.Modules.Map/*.csproj backend/Arah.Modules.Map/
COPY backend/Arah.Modules.Marketplace/*.csproj backend/Arah.Modules.Marketplace/
COPY backend/Arah.Modules.Moderation/*.csproj backend/Arah.Modules.Moderation/
COPY backend/Arah.Modules.Notifications/*.csproj backend/Arah.Modules.Notifications/
COPY backend/Arah.Modules.Subscriptions/*.csproj backend/Arah.Modules.Subscriptions/
COPY backend/Arah.Modules.Alerts.Infrastructure/*.csproj backend/Arah.Modules.Alerts.Infrastructure/
COPY backend/Arah.Modules.Assets.Infrastructure/*.csproj backend/Arah.Modules.Assets.Infrastructure/
COPY backend/Arah.Modules.Chat.Infrastructure/*.csproj backend/Arah.Modules.Chat.Infrastructure/
COPY backend/Arah.Modules.Connections.Infrastructure/*.csproj backend/Arah.Modules.Connections.Infrastructure/
COPY backend/Arah.Modules.Events.Infrastructure/*.csproj backend/Arah.Modules.Events.Infrastructure/
COPY backend/Arah.Modules.Feed.Infrastructure/*.csproj backend/Arah.Modules.Feed.Infrastructure/
COPY backend/Arah.Modules.Map.Infrastructure/*.csproj backend/Arah.Modules.Map.Infrastructure/
COPY backend/Arah.Modules.Marketplace.Infrastructure/*.csproj backend/Arah.Modules.Marketplace.Infrastructure/
COPY backend/Arah.Modules.Moderation.Infrastructure/*.csproj backend/Arah.Modules.Moderation.Infrastructure/
COPY backend/Arah.Modules.Notifications.Infrastructure/*.csproj backend/Arah.Modules.Notifications.Infrastructure/
COPY backend/Arah.Modules.Subscriptions.Infrastructure/*.csproj backend/Arah.Modules.Subscriptions.Infrastructure/

# 2) Restore
WORKDIR /src/backend/Arah.Api
RUN dotnet restore

# 3) Copiar o resto do código
WORKDIR /src
COPY . .

# 4) Publish
WORKDIR /src/backend/Arah.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "Arah.Api.dll"]
