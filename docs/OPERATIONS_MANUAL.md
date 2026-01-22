# Manual de Operação - Araponga

**Última Atualização**: 2026-01-21  
**Versão**: 1.0

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Deploy](#deploy)
3. [Rollback](#rollback)
4. [Backup e Restore](#backup-e-restore)
5. [Monitoramento](#monitoramento)
6. [Escalabilidade](#escalabilidade)
7. [Manutenção](#manutenção)

---

## 1. Visão Geral

Araponga é uma API ASP.NET Core 8.0 que utiliza:
- **Banco de Dados**: PostgreSQL
- **Cache**: Redis (opcional, fallback para IMemoryCache)
- **Autenticação**: JWT
- **Observabilidade**: OpenTelemetry, Prometheus, Serilog
- **Containerização**: Docker

---

## 2. Deploy

### 2.1 Pré-requisitos

- Docker e Docker Compose instalados
- Acesso ao repositório de imagens (GHCR)
- Variáveis de ambiente configuradas
- Banco de dados PostgreSQL acessível

### 2.2 Deploy via Docker

```bash
# 1. Fazer pull da imagem mais recente
docker pull ghcr.io/[seu-org]/araponga-api:latest

# 2. Parar container existente (se houver)
docker stop araponga-api || true
docker rm araponga-api || true

# 3. Executar novo container
docker run -d \
  --name araponga-api \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Postgres="Host=postgres;Database=araponga;Username=..." \
  -e JWT__SIGNINGKEY="[seu-jwt-secret]" \
  -e Cors__AllowedOrigins__0="https://app.araponga.com" \
  ghcr.io/[seu-org]/araponga-api:latest
```

### 2.3 Deploy via Docker Compose

```yaml
version: '3.8'
services:
  araponga-api:
    image: ghcr.io/[seu-org]/araponga-api:latest
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Postgres=Host=postgres;Database=araponga;...
      - JWT__SIGNINGKEY=${JWT_SIGNINGKEY}
    depends_on:
      - postgres
      - redis
    restart: unless-stopped
```

```bash
docker-compose up -d
```

### 2.4 Deploy via Kubernetes

Ver `docs/kubernetes/` para manifests completos.

### 2.5 Variáveis de Ambiente Críticas

| Variável | Descrição | Obrigatória |
|----------|-----------|-------------|
| `JWT__SIGNINGKEY` | Chave secreta para assinatura JWT (mín. 32 chars) | ✅ Sim |
| `ConnectionStrings__Postgres` | String de conexão PostgreSQL | ✅ Sim |
| `ASPNETCORE_ENVIRONMENT` | Ambiente (Development/Staging/Production) | ✅ Sim |
| `Cors__AllowedOrigins__0` | Origem permitida para CORS | ✅ Sim (prod) |
| `ConnectionStrings__Redis` | String de conexão Redis (opcional) | ❌ Não |
| `OpenTelemetry__Otlp__Endpoint` | Endpoint OTLP para traces (opcional) | ❌ Não |
| `Logging__Seq__ServerUrl` | URL do Seq para logs (opcional) | ❌ Não |

---

## 3. Rollback

### 3.1 Rollback Rápido (Docker)

```bash
# 1. Identificar versão anterior
docker images ghcr.io/[seu-org]/araponga-api --format "table {{.Tag}}\t{{.CreatedAt}}"

# 2. Parar container atual
docker stop araponga-api

# 3. Executar versão anterior
docker run -d \
  --name araponga-api \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  [mesmas variáveis de ambiente] \
  ghcr.io/[seu-org]/araponga-api:[tag-anterior]
```

### 3.2 Rollback com Migrations

⚠️ **Atenção**: Rollback de migrations pode causar perda de dados.

```bash
# 1. Conectar ao banco
psql -h [host] -U [user] -d araponga

# 2. Listar migrations aplicadas
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId" DESC LIMIT 5;

# 3. Reverter última migration (CUIDADO!)
# Use dotnet ef migrations script para gerar script de rollback
dotnet ef migrations script [migration-anterior] [migration-atual] \
  --project backend/Araponga.Infrastructure \
  --startup-project backend/Araponga.Api \
  --output rollback.sql

# 4. Revisar script e executar manualmente
psql -h [host] -U [user] -d araponga -f rollback.sql
```

---

## 4. Backup e Restore

### 4.1 Backup do Banco de Dados

```bash
# Backup completo
pg_dump -h [host] -U [user] -d araponga -F c -f backup_$(date +%Y%m%d_%H%M%S).dump

# Backup apenas schema
pg_dump -h [host] -U [user] -d araponga --schema-only -f schema_$(date +%Y%m%d).sql

# Backup apenas dados
pg_dump -h [host] -U [user] -d araponga --data-only -F c -f data_$(date +%Y%m%d).dump
```

### 4.2 Restore do Banco de Dados

```bash
# Restore completo
pg_restore -h [host] -U [user] -d araponga -c backup_20260121_120000.dump

# Restore apenas schema
psql -h [host] -U [user] -d araponga -f schema_20260121.sql

# Restore apenas dados
pg_restore -h [host] -U [user] -d araponga --data-only backup_20260121_120000.dump
```

### 4.3 Backup Automatizado (Cron)

```bash
# Adicionar ao crontab
0 2 * * * pg_dump -h [host] -U [user] -d araponga -F c -f /backups/araponga_$(date +\%Y\%m\%d).dump
```

### 4.4 Retenção de Backups

- **Produção**: 30 dias diários + 12 mensais
- **Staging**: 7 dias diários
- **Desenvolvimento**: 3 dias diários

---

## 5. Monitoramento

### 5.1 Health Checks

A API expõe health checks em `/health`:

```bash
# Health check básico
curl http://localhost:8080/health

# Health check detalhado
curl http://localhost:8080/health/detailed
```

### 5.2 Métricas Prometheus

Métricas disponíveis em `/metrics`:

```bash
curl http://localhost:8080/metrics
```

Principais métricas:
- `http_requests_total` - Total de requisições HTTP
- `http_request_duration_seconds` - Duração de requisições
- `database_connections_active` - Conexões ativas ao banco
- `cache_hits_total` - Cache hits
- `cache_misses_total` - Cache misses

### 5.3 Logs

Logs são escritos em:
- **Console**: stdout/stderr
- **Arquivo**: `logs/araponga-YYYYMMDD.log` (30 dias de retenção)
- **Seq** (se configurado): Via `Logging__Seq__ServerUrl`

Níveis de log:
- **Development**: Information
- **Staging**: Warning
- **Production**: Warning (Error para exceções)

### 5.4 Alertas Recomendados

| Métrica | Threshold | Ação |
|---------|-----------|------|
| Taxa de erro HTTP | > 5% | Investigar logs |
| Latência P95 | > 1000ms | Investigar queries lentas |
| Conexões DB | > 80% do pool | Escalar ou otimizar |
| Uso de memória | > 80% | Escalar ou investigar leaks |
| CPU | > 80% por 5min | Escalar |

---

## 6. Escalabilidade

### 6.1 Escala Horizontal

A API é **stateless** e pode ser escalada horizontalmente:

```bash
# Docker Compose - múltiplas instâncias
docker-compose up -d --scale araponga-api=3

# Kubernetes - HPA
kubectl autoscale deployment araponga-api --min=2 --max=10 --cpu-percent=70
```

### 6.2 Cache Distribuído

Para escalar horizontalmente, **Redis é obrigatório**:

```bash
# Configurar Redis
-e ConnectionStrings__Redis="[redis-connection-string]"
```

Sem Redis, cada instância usa IMemoryCache (não compartilhado).

### 6.3 Read Replicas

Para alta carga de leitura, configurar read replicas do PostgreSQL:

```csharp
// Em Program.cs, configurar múltiplas connection strings
// ConnectionStrings__PostgresReadReplica
```

---

## 7. Manutenção

### 7.1 Atualização de Dependências

```bash
# Verificar dependências desatualizadas
dotnet list package --outdated

# Atualizar pacotes
dotnet add package [package-name] --version [version]
```

### 7.2 Limpeza de Cache

```bash
# Limpar cache Redis (se usado)
redis-cli FLUSHDB

# Limpar logs antigos
find logs/ -name "*.log" -mtime +30 -delete
```

### 7.3 Vacuum do PostgreSQL

```sql
-- Vacuum manual (recomendado mensalmente)
VACUUM ANALYZE;

-- Vacuum completo (recomendado trimestralmente)
VACUUM FULL;
```

### 7.4 Rotação de Logs

Logs são rotacionados automaticamente (diário, 30 dias de retenção).

Para rotação manual:
```bash
# Comprimir logs antigos
find logs/ -name "*.log" -mtime +7 -exec gzip {} \;

# Remover logs muito antigos
find logs/ -name "*.log.gz" -mtime +30 -delete
```

---

## 8. Troubleshooting

Ver `docs/TROUBLESHOOTING.md` para problemas comuns e soluções.

---

**Última Atualização**: 2026-01-21
