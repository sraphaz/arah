# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0.13 AS build
WORKDIR /src

# 1) Copiar solution e csproj (para cache)
COPY *.sln ./

COPY backend/Core/Araponga.Api/*.csproj backend/Core/Araponga.Api/
COPY backend/Core/Araponga.Application/*.csproj backend/Core/Araponga.Application/
COPY backend/Core/Araponga.Domain/*.csproj backend/Core/Araponga.Domain/
COPY backend/Core/Araponga.Infrastructure/*.csproj backend/Core/Araponga.Infrastructure/
COPY backend/Core/Araponga.Infrastructure.Shared/*.csproj backend/Core/Araponga.Infrastructure.Shared/
COPY backend/Core/Araponga.Shared/*.csproj backend/Core/Araponga.Shared/
COPY backend/Modules/ backend/Modules/
COPY backend/Araponga.Tests/*.csproj backend/Araponga.Tests/

# 2) Restore
WORKDIR /src/backend/Core/Araponga.Api
RUN dotnet restore

# 3) Copiar o resto do código
WORKDIR /src
COPY . .

# 4) Publish sem restore (agora funciona)
WORKDIR /src/backend/Core/Araponga.Api
RUN dotnet publish -c Release -o /app/publish --no-restore


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0.13 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "Araponga.Api.dll"]
