# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copiar solution e csproj (para cache de layers)
COPY Araponga.sln ./
COPY backend/Araponga.Domain/*.csproj backend/Araponga.Domain/
COPY backend/Araponga.Shared/*.csproj backend/Araponga.Shared/
COPY backend/Araponga.Application/*.csproj backend/Araponga.Application/
COPY backend/Araponga.Infrastructure/*.csproj backend/Araponga.Infrastructure/
COPY backend/Araponga.Infrastructure.Shared/*.csproj backend/Araponga.Infrastructure.Shared/
COPY backend/Araponga.Api/*.csproj backend/Araponga.Api/
COPY backend/Araponga.Modules.Admin.Infrastructure/*.csproj backend/Araponga.Modules.Admin.Infrastructure/
COPY backend/Araponga.Modules.Alerts/*.csproj backend/Araponga.Modules.Alerts/
COPY backend/Araponga.Modules.Assets/*.csproj backend/Araponga.Modules.Assets/
COPY backend/Araponga.Modules.Chat/*.csproj backend/Araponga.Modules.Chat/
COPY backend/Araponga.Modules.Connections/*.csproj backend/Araponga.Modules.Connections/
COPY backend/Araponga.Modules.Events/*.csproj backend/Araponga.Modules.Events/
COPY backend/Araponga.Modules.Feed/*.csproj backend/Araponga.Modules.Feed/
COPY backend/Araponga.Modules.Map/*.csproj backend/Araponga.Modules.Map/
COPY backend/Araponga.Modules.Marketplace/*.csproj backend/Araponga.Modules.Marketplace/
COPY backend/Araponga.Modules.Moderation/*.csproj backend/Araponga.Modules.Moderation/
COPY backend/Araponga.Modules.Notifications/*.csproj backend/Araponga.Modules.Notifications/
COPY backend/Araponga.Modules.Subscriptions/*.csproj backend/Araponga.Modules.Subscriptions/

# 2) Restore
WORKDIR /src/backend/Araponga.Api
RUN dotnet restore

# 3) Copiar o resto do código
WORKDIR /src
COPY . .

# 4) Publish
WORKDIR /src/backend/Araponga.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "Araponga.Api.dll"]
