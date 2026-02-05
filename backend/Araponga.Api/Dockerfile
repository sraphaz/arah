# SDK .NET 8 (tag 8.0 = último patch disponível; evita "not found" em algumas arquiteturas)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solução e todo o backend para garantir que todos os projetos (incl. módulos) existam no restore
COPY Araponga.sln ./
COPY backend/ backend/

RUN dotnet restore Araponga.sln

WORKDIR /src/backend/Araponga.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Araponga.Api.dll"]
