# Setup

## Pré-requisitos

- .NET 8.0
- Node.js 18+
- PostgreSQL 14+
- Docker (opcional)

## Instalação Local

### Backend

```bash
# Restaurar dependências .NET
dotnet restore

# Build
dotnet build Araponga.sln

# Aplicar migrações
dotnet ef database update --project backend/Araponga.Infrastructure
```

### Frontend

```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build produção
npm run build
```

### Wiki

```bash
cd frontend/wiki
npm install
npm run dev
# Acessa http://localhost:3000/wiki
```

## Variáveis de Ambiente

Criar arquivo `.env` na raiz:

```env
# Database
DATABASE_URL=postgresql://user:password@localhost/araponga

# Email
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USER=seu-email@gmail.com
EMAIL_PASSWORD=sua-senha

# Discord
DISCORD_BOT_TOKEN=seu-token
```

## Docker

```bash
# Build
docker-compose build

# Executar
docker-compose up -d

# Logs
docker-compose logs -f
```

## Problemas Comuns

### Build falha
- Limpar cache: `dotnet clean`
- Restaurar: `dotnet restore`

### Testes falham
- Banco de dados não está rodando
- Verificar `appsettings.json` em testes

### Wiki não carrega
- Certifique-se que `/docs` existe
- Rodou `npm install` em `frontend/wiki`?
