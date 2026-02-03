FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Araponga.sln ./
COPY backend/Araponga.Api/Araponga.Api.csproj backend/Araponga.Api/
COPY backend/Araponga.Application/Araponga.Application.csproj backend/Araponga.Application/
COPY backend/Araponga.Domain/Araponga.Domain.csproj backend/Araponga.Domain/
COPY backend/Araponga.Infrastructure/Araponga.Infrastructure.csproj backend/Araponga.Infrastructure/
COPY backend/Araponga.Infrastructure.Shared/Araponga.Infrastructure.Shared.csproj backend/Araponga.Infrastructure.Shared/
COPY backend/Araponga.Shared/Araponga.Shared.csproj backend/Araponga.Shared/
COPY backend/Araponga.Tests/Araponga.Tests.csproj backend/Araponga.Tests/
COPY backend/Araponga.Modules.Subscriptions.Infrastructure/Araponga.Modules.Subscriptions.Infrastructure.csproj backend/Araponga.Modules.Subscriptions.Infrastructure/
COPY backend/Araponga.Modules.Moderation.Infrastructure/Araponga.Modules.Moderation.Infrastructure.csproj backend/Araponga.Modules.Moderation.Infrastructure/
COPY backend/Araponga.Modules.Notifications.Infrastructure/Araponga.Modules.Notifications.Infrastructure.csproj backend/Araponga.Modules.Notifications.Infrastructure/
COPY backend/Araponga.Modules.Alerts.Infrastructure/Araponga.Modules.Alerts.Infrastructure.csproj backend/Araponga.Modules.Alerts.Infrastructure/
COPY backend/Araponga.Modules.Assets.Infrastructure/Araponga.Modules.Assets.Infrastructure.csproj backend/Araponga.Modules.Assets.Infrastructure/
COPY backend/Araponga.Modules.Admin.Infrastructure/Araponga.Modules.Admin.Infrastructure.csproj backend/Araponga.Modules.Admin.Infrastructure/
COPY backend/Araponga.Modules.Feed.Infrastructure/Araponga.Modules.Feed.Infrastructure.csproj backend/Araponga.Modules.Feed.Infrastructure/
COPY backend/Araponga.Modules.Marketplace.Infrastructure/Araponga.Modules.Marketplace.Infrastructure.csproj backend/Araponga.Modules.Marketplace.Infrastructure/
COPY backend/Araponga.Modules.Events.Infrastructure/Araponga.Modules.Events.Infrastructure.csproj backend/Araponga.Modules.Events.Infrastructure/
COPY backend/Araponga.Modules.Map.Infrastructure/Araponga.Modules.Map.Infrastructure.csproj backend/Araponga.Modules.Map.Infrastructure/
COPY backend/Araponga.Modules.Chat.Infrastructure/Araponga.Modules.Chat.Infrastructure.csproj backend/Araponga.Modules.Chat.Infrastructure/

RUN dotnet restore Araponga.sln

COPY . .
WORKDIR /src/backend/Araponga.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Araponga.Api.dll"]
