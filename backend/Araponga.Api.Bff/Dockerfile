# SDK .NET 8 (mesmo padrão da API; BFF é consumido pelo app em porta 5001)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solução e todo o backend para restore e publish do BFF (depende de outros projetos)
COPY Araponga.sln ./
COPY backend/ backend/

RUN dotnet restore Araponga.sln

WORKDIR /src/backend/Araponga.Api.Bff
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "Araponga.Bff.dll"]
