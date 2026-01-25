# Ambiente Docker de Desenvolvimento/Pré-Produção

Este documento descreve como usar o ambiente Docker completo do Araponga para desenvolvimento e pré-produção.

## 📋 Visão Geral

O ambiente Docker inclui:

- **PostgreSQL 16 com PostGIS** - Banco de dados principal com suporte geoespacial
- **Redis 7** - Cache distribuído para melhor performance
- **MinIO** - Storage S3-compatible para desenvolvimento/pré-produção
- **API Araponga** - Aplicação .NET 8 com todas as dependências configuradas

## 🚀 Início Rápido

### 1. Pré-requisitos

- Docker Desktop (Windows/Mac) ou Docker Engine + Docker Compose (Linux)
- PowerShell (Windows) ou Bash (Linux/Mac)

### 2. Configuração Inicial

```powershell
# Copiar arquivo de exemplo de variáveis de ambiente
cp .env.example .env

# Editar .env e configurar JWT_SIGNINGKEY (obrigatório!)
# Gere um secret forte com:
openssl rand -base64 32
# OU no PowerShell:
[Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```

### 3. Iniciar Ambiente

**Opção 1: Usando o script PowerShell (recomendado)**

```powershell
# Iniciar todos os serviços
.\scripts\docker-dev.ps1 up -Build

# Iniciar em background
.\scripts\docker-dev.ps1 up -Build -Detached
```

**Opção 2: Usando Docker Compose diretamente**

```bash
# Iniciar todos os serviços
docker-compose -f docker-compose.dev.yml up --build

# Iniciar em background
docker-compose -f docker-compose.dev.yml up -d --build
```

### 4. Verificar Status

```powershell
# Ver status dos containers
.\scripts\docker-dev.ps1 status

# Ver logs
.\scripts\docker-dev.ps1 logs

# Ver logs de um serviço específico
.\scripts\docker-dev.ps1 logs -Service api
```

## 📍 Endpoints Disponíveis

Após iniciar o ambiente, os seguintes endpoints estarão disponíveis:

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **API** | http://localhost:8080 | - |
| **Swagger** | http://localhost:8080/swagger | - |
| **Health Check** | http://localhost:8080/health | - |
| **MinIO API** | http://localhost:9000 | minioadmin / minioadmin |
| **MinIO Console** | http://localhost:9001 | minioadmin / minioadmin |
| **PostgreSQL** | localhost:5432 | araponga / araponga |
| **Redis** | localhost:6379 | senha: araponga |

## 🛠️ Comandos Úteis

### Script PowerShell

```powershell
# Iniciar serviços
.\scripts\docker-dev.ps1 up

# Parar serviços
.\scripts\docker-dev.ps1 down

# Reiniciar serviços
.\scripts\docker-dev.ps1 restart

# Ver logs
.\scripts\docker-dev.ps1 logs

# Ver logs de um serviço específico
.\scripts\docker-dev.ps1 logs -Service api

# Ver status
.\scripts\docker-dev.ps1 status

# Abrir shell no container da API
.\scripts\docker-dev.ps1 shell

# Aplicar migrações do banco
.\scripts\docker-dev.ps1 db-migrate

# Resetar banco de dados (CUIDADO!)
.\scripts\docker-dev.ps1 db-reset

# Limpar tudo (containers, volumes, imagens)
.\scripts\docker-dev.ps1 clean
```

### Docker Compose Direto

```bash
# Iniciar
docker-compose -f docker-compose.dev.yml up -d

# Parar
docker-compose -f docker-compose.dev.yml down

# Ver logs
docker-compose -f docker-compose.dev.yml logs -f

# Ver logs de um serviço
docker-compose -f docker-compose.dev.yml logs -f api

# Rebuild forçado
docker-compose -f docker-compose.dev.yml up -d --build --force-recreate

# Parar e remover volumes
docker-compose -f docker-compose.dev.yml down -v
```

## ⚙️ Configuração

### Variáveis de Ambiente

O arquivo `.env` contém todas as configurações. Principais variáveis:

```env
# JWT (OBRIGATÓRIO - gere um secret forte!)
JWT_SIGNINGKEY=seu-secret-aqui-minimo-32-caracteres

# Database
POSTGRES_USER=araponga
POSTGRES_PASSWORD=araponga
POSTGRES_DB=araponga

# Redis
REDIS_PASSWORD=araponga

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
MINIO_BUCKET_NAME=araponga-media

# CORS (desenvolvimento pode usar *)
CORS_ALLOWED_ORIGIN_0=*
```

### Configuração de Storage

Por padrão, o ambiente usa **MinIO** (S3-compatible) para armazenamento de mídias. O bucket `araponga-media` é criado automaticamente.

Para usar storage local, altere no `.env`:

```env
MEDIA_STORAGE_PROVIDER=Local
```

### Configuração de Cache

O Redis é configurado automaticamente. Se não estiver disponível, o sistema faz fallback para cache em memória.

## 🔍 Troubleshooting

### API não inicia

1. Verifique se o JWT_SIGNINGKEY está configurado no `.env`
2. Verifique logs: `.\scripts\docker-dev.ps1 logs -Service api`
3. Verifique se o PostgreSQL está saudável: `.\scripts\docker-dev.ps1 status`

### Banco de dados não conecta

1. Verifique se o PostgreSQL está rodando: `docker ps`
2. Verifique as credenciais no `.env`
3. Verifique os logs: `.\scripts\docker-dev.ps1 logs -Service postgres`

### MinIO não acessível

1. Verifique se o MinIO está rodando: `docker ps`
2. Acesse o console: http://localhost:9001
3. Verifique os logs: `.\scripts\docker-dev.ps1 logs -Service minio`

### Portas já em uso

Se alguma porta estiver em uso, altere no `.env`:

```env
API_PORT=8081
POSTGRES_PORT=5433
REDIS_PORT=6380
MINIO_API_PORT=9002
MINIO_CONSOLE_PORT=9002
```

### Resetar ambiente completamente

```powershell
# Parar e remover tudo
.\scripts\docker-dev.ps1 clean

# Ou manualmente:
docker-compose -f docker-compose.dev.yml down -v --remove-orphans
docker system prune -a --volumes
```

## 📦 Migrações do Banco de Dados

As migrações são aplicadas automaticamente na inicialização (`Persistence__ApplyMigrations=true`).

Para aplicar manualmente:

```powershell
.\scripts\docker-dev.ps1 db-migrate
```

Ou manualmente:

```bash
docker exec -it araponga-api dotnet ef database update \
  --project /src/backend/Araponga.Infrastructure \
  --startup-project /src/backend/Araponga.Api
```

## 🔐 Segurança

### Desenvolvimento

- CORS permite wildcard (`*`)
- Senhas padrão (araponga/araponga)
- JWT secret deve ser configurado mas pode ser simples

### Pré-Produção

⚠️ **IMPORTANTE**: Antes de usar em pré-produção, configure:

1. **JWT Secret forte** (mínimo 32 caracteres)
   ```bash
   openssl rand -base64 32
   ```

2. **CORS específico** (não use wildcard)
   ```env
   CORS_ALLOWED_ORIGIN_0=https://app.araponga.com
   CORS_ALLOWED_ORIGIN_1=https://www.araponga.com
   ```

3. **Senhas fortes** para PostgreSQL, Redis e MinIO

4. **HTTPS** (configure reverse proxy com certificado SSL)

5. **Variáveis de ambiente seguras** (não commitar `.env` no git)

## 📊 Monitoramento

### Health Checks

```bash
# Health check da API
curl http://localhost:8080/health

# Health check de readiness (verifica dependências)
curl http://localhost:8080/health/ready
```

### Logs

```powershell
# Todos os logs
.\scripts\docker-dev.ps1 logs

# Logs da API
.\scripts\docker-dev.ps1 logs -Service api

# Logs do PostgreSQL
.\scripts\docker-dev.ps1 logs -Service postgres
```

### Métricas

A API expõe métricas Prometheus em `/metrics` (se configurado).

## 🚢 Produção

Este ambiente é adequado para **desenvolvimento e pré-produção**. Para produção:

1. Use serviços gerenciados (RDS, ElastiCache, S3)
2. Configure HTTPS com certificado válido
3. Use secrets management (AWS Secrets Manager, Azure Key Vault)
4. Configure backup automático do banco
5. Configure monitoramento e alertas
6. Use load balancer e múltiplas instâncias

Veja a documentação completa em:
- [Avaliação para Produção](./50_PRODUCAO_AVALIACAO_COMPLETA.md)
- [Configuração de Segurança](./SECURITY_CONFIGURATION.md)

## 📚 Referências

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostGIS Documentation](https://postgis.net/documentation/)
- [Redis Documentation](https://redis.io/documentation)
- [MinIO Documentation](https://min.io/docs/)
