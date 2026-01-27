# Documentação Operacional - Araponga API

**Data de Criação**: 2026-01-26  
**Versão**: 1.0  
**Ambiente**: Produção

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Deploy e Configuração Inicial](#deploy-e-configuração-inicial)
3. [Configuração de Ambiente](#configuração-de-ambiente)
4. [Monitoramento e Observabilidade](#monitoramento-e-observabilidade)
5. [Manutenção e Operação](#manutenção-e-operação)
6. [Escalabilidade](#escalabilidade)
7. [Segurança Operacional](#segurança-operacional)
8. [Troubleshooting](#troubleshooting)
9. [Procedimentos de Emergência](#procedimentos-de-emergência)
10. [Checklist de Produção](#checklist-de-produção)

---

## 🎯 Visão Geral

Esta documentação fornece informações básicas para operação da API Araponga em produção.

### Componentes Principais

- **API REST**: ASP.NET Core 8
- **Banco de Dados**: PostgreSQL
- **Cache**: Redis
- **Storage**: S3-compatible (mídias)
- **Queue**: Background workers (emails, outbox)
- **Observabilidade**: Serilog, Prometheus, OpenTelemetry

---

## 🚀 Deploy e Configuração Inicial

### Pré-requisitos

- **.NET 8 SDK** instalado
- **PostgreSQL 14+** configurado e acessível
- **Redis** (opcional, mas recomendado para cache)
- **S3-compatible storage** (ou Local para desenvolvimento)
- **SMTP Server** configurado para emails

### Deploy via Docker (Recomendado)

```bash
# Build da imagem
docker build -t araponga-api:latest .

# Executar com docker-compose
docker-compose up -d

# Verificar logs
docker-compose logs -f araponga-api
```

### Deploy Manual

```bash
# 1. Publicar aplicação
dotnet publish backend/Araponga.Api/Araponga.Api.csproj -c Release -o ./publish

# 2. Configurar variáveis de ambiente (ver seção abaixo)

# 3. Executar migrações
cd publish
dotnet Araponga.Api.dll --migrate

# 4. Executar seed inicial
dotnet Araponga.Api.dll --seed

# 5. Iniciar aplicação
dotnet Araponga.Api.dll
```

---

## ⚙️ Configuração de Ambiente

### Variáveis de Ambiente Críticas

#### Banco de Dados (PostgreSQL)

```bash
ConnectionStrings__Postgres=Host=<host>;Port=5432;Database=araponga;Username=<user>;Password=<password>;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;Command Timeout=30
Persistence__Provider=Postgres
Persistence__ApplyMigrations=true
```

**Recomendações**:
- Connection pooling: Min 5, Max 100 conexões
- Timeout: 30 segundos
- SSL obrigatório em produção

#### JWT (Segurança)

```bash
JWT__SIGNINGKEY=<strong_secret_key_min_32_chars>
JWT__Issuer=Araponga
JWT__Audience=Araponga
JWT__ExpirationMinutes=60
```

**⚠️ CRÍTICO**: 
- Secret deve ter **mínimo 32 caracteres** em produção
- Nunca usar valor padrão em produção
- Rotacionar periodicamente

#### Redis (Cache)

```bash
ConnectionStrings__Redis=<redis_connection_string>
```

**Formato**: `host:port` ou `host:port,password=xxx`

**Fallback**: Se Redis não estiver disponível, aplicação usa cache em memória (degradação graciosa)

#### Storage (Mídias)

**Opção 1: S3-compatible (Produção)**

```bash
Storage__Provider=S3
Storage__S3__AccessKey=<access_key>
Storage__S3__SecretKey=<secret_key>
Storage__S3__BucketName=<bucket_name>
Storage__S3__Endpoint=<endpoint_url>
Storage__S3__Region=<region>
```

**Opção 2: Local (Desenvolvimento)**

```bash
Storage__Provider=Local
Storage__LocalPath=/app/wwwroot/media
```

#### Email (SMTP)

```bash
Email__Smtp__Host=<smtp_host>
Email__Smtp__Port=587
Email__Smtp__Username=<smtp_username>
Email__Smtp__Password=<smtp_password>
Email__Smtp__EnableSsl=true
Email__FromAddress=<from_email>
Email__FromName=Araponga
```

**Provedores Testados**:
- SendGrid
- Amazon SES
- Mailgun
- SMTP genérico (Gmail, Outlook, etc.)

#### Rate Limiting

```bash
RateLimiting__PermitLimit=1000
RateLimiting__WindowSeconds=60
RateLimiting__QueueLimit=100
```

**Limites por Endpoint**:
- **Default**: 60 req/min
- **Feed**: 100 req/min
- **Read**: 100 req/min
- **Write**: 30 req/min

#### Base URL e CORS

```bash
BaseUrl=https://araponga.com
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
```

#### Observabilidade

```bash
# Prometheus
Metrics__Prometheus__Port=9090

# OpenTelemetry (Opcional)
OpenTelemetry__Otlp__Endpoint=<otlp_endpoint>
OpenTelemetry__Jaeger__Endpoint=<jaeger_endpoint>

# Seq (Opcional)
Logging__Seq__ServerUrl=<seq_url>
Logging__Seq__ApiKey=<seq_api_key>
```

### Migrações do Banco de Dados

#### Primeira Execução

```bash
# Aplicar todas as migrações
dotnet ef database update --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api

# Ou via aplicação (se configurado)
Persistence__ApplyMigrations=true
```

#### Migrações Incrementais

```bash
# Verificar migrações pendentes
dotnet ef migrations list --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api

# Aplicar migrações pendentes
dotnet ef database update --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api
```

### Seed Inicial

#### Plano FREE Padrão

```bash
# Via endpoint administrativo (requer autenticação admin)
POST /api/v1/admin/seed/default-plan
Authorization: Bearer <admin_token>

# Ou via script
dotnet run --project backend/Araponga.Api -- seed-default-plan
```

**Nota**: O plano FREE é criado automaticamente na primeira execução se não existir.

---

## 📊 Monitoramento e Observabilidade

### Health Checks

**Endpoint**: `GET /health`

**Status Codes**:
- `200 OK`: Todos os serviços saudáveis
- `503 Service Unavailable`: Algum serviço indisponível

**Componentes Verificados**:
- ✅ PostgreSQL (conexão e query básica)
- ✅ Redis (conexão e ping)
- ✅ Storage (S3 ou Local - verificação de acesso)

**Resposta JSON**:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "postgres": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0012345"
    },
    "storage": {
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    }
  }
}
```

**Uso em Load Balancers**:
- Configurar health check endpoint no load balancer
- Intervalo recomendado: 30 segundos
- Timeout: 5 segundos
- Threshold: 2 falhas consecutivas para marcar como unhealthy

### Métricas (Prometheus)

**Endpoint**: `GET /metrics`

**Porta**: 9090 (configurável via `Metrics__Prometheus__Port`)

**Métricas de Negócio**:
- `araponga.posts.created` - Total de posts criados
- `araponga.events.created` - Total de eventos criados
- `araponga.memberships.created` - Total de memberships criados
- `araponga.territories.created` - Total de territórios criados
- `araponga.reports.created` - Total de reports criados

**Métricas de Cache**:
- `araponga.cache.hits` - Cache hits
- `araponga.cache.misses` - Cache misses
- Taxa de hit: `cache_hits / (cache_hits + cache_misses)`

**Métricas HTTP** (Prometheus padrão):
- `http_requests_total` - Total de requisições HTTP por método, rota, status
- `http_request_duration_seconds` - Duração de requisições (histograma)
- `http_requests_in_flight` - Requisições em processamento

**Métricas de Sistema**:
- `dotnet_gc_collections_total` - Coletas de GC
- `dotnet_gc_seconds` - Tempo em GC
- `process_cpu_seconds_total` - CPU usado
- `process_working_set_bytes` - Memória usada

**Grafana Dashboards**:
- Importar dashboards padrão do .NET
- Criar dashboards customizados para métricas de negócio

### Logs (Serilog)

**Formato**: JSON estruturado (produção) ou texto formatado (desenvolvimento)

**Níveis**:
- `Information`: Operações normais, fluxo de aplicação
- `Warning`: Situações que requerem atenção (rate limit, cache miss, etc.)
- `Error`: Erros que não interrompem a aplicação (falhas de integração, etc.)
- `Critical`: Erros críticos que podem interromper a aplicação

**Sinks Configurados**:
- **Console**: Desenvolvimento e debug
- **File**: `logs/araponga-YYYYMMDD.log` (rotação diária, 30 dias de retenção)
- **Seq**: Opcional, se `Logging__Seq__ServerUrl` configurado

**Enriquecimento**:
- `CorrelationId`: ID de correlação para rastreamento
- `MachineName`: Nome da máquina
- `ThreadId`: ID da thread
- `EnvironmentName`: Nome do ambiente (Development, Production, etc.)
- `Application`: "Araponga"
- `Version`: Versão da aplicação

**Filtros Recomendados**:
```bash
# Buscar erros
grep '"Level":"Error"' logs/araponga-*.log

# Buscar por CorrelationId
grep '"CorrelationId":"<id>"' logs/araponga-*.log

# Buscar por componente
grep '"SourceContext":"Araponga.Application.Services.EmailQueueService"' logs/araponga-*.log
```

### Tracing (OpenTelemetry)

**Configuração**:
- **OTLP Endpoint**: Para exportar traces para coletor OpenTelemetry
- **Jaeger Endpoint**: Para exportar diretamente para Jaeger

**Spans Capturados**:
- Requisições HTTP (automático)
- Operações de banco de dados
- Chamadas de serviços externos
- Processamento de background workers

### Alertas Recomendados

#### Críticos (P0)
- Health check retorna 503 por mais de 2 minutos
- Taxa de erro HTTP > 5% por 5 minutos
- CPU > 90% por 10 minutos
- Memória > 90% por 10 minutos
- PostgreSQL indisponível

#### Importantes (P1)
- Taxa de erro HTTP > 1% por 15 minutos
- Latência P95 > 2 segundos por 10 minutos
- Cache hit rate < 70% por 30 minutos
- Email queue com mais de 1000 itens pendentes

#### Informativos (P2)
- Taxa de erro HTTP > 0.1% por 1 hora
- Latência P95 > 1 segundo por 30 minutos
- Workers processando mais lento que o esperado

---

## 🔧 Manutenção e Operação

### Background Workers

#### EmailQueueWorker
- **Função**: Processa fila de emails pendentes
- **Intervalo**: 30 segundos
- **Batch Size**: 10 emails por ciclo
- **Rate Limit**: 100 emails/minuto
- **Retry Policy**: 3 tentativas (5min, 15min, 1h)
- **Dead Letter**: Após 3 tentativas falhas

**Monitoramento**:
```bash
# Verificar logs do worker
grep "EmailQueueWorker" logs/araponga-*.log | tail -20

# Verificar emails processados
grep "Processed.*emails from queue" logs/araponga-*.log
```

**Troubleshooting**:
- Se emails não são processados: Verificar se worker está rodando
- Se rate limit atingido: Aumentar `EmailQueueWorker._maxEmailsPerMinute` ou reduzir batch size
- Se muitos dead letters: Verificar configuração SMTP e credenciais

#### OutboxDispatcherWorker
- **Função**: Processa mensagens do Outbox (notificações, eventos)
- **Intervalo**: 5 segundos
- **Batch Size**: 100 mensagens por ciclo
- **Retry Policy**: 5 tentativas com backoff exponencial (5s, 30s, 2min, 10min, 30min)

**Monitoramento**:
```bash
# Verificar processamento
grep "OutboxDispatcherWorker" logs/araponga-*.log | tail -20

# Verificar mensagens com erro
grep "Outbox processing failed" logs/araponga-*.log
```

**Troubleshooting**:
- Se mensagens não são processadas: Verificar se worker está rodando
- Se muitas falhas: Verificar integridade dos dados no Outbox
- Se processamento lento: Verificar performance do banco de dados

#### SubscriptionRenewalWorker
- **Função**: Processa renovações de assinaturas
- **Intervalo**: 1 hora
- **Janela**: 3 dias antes do vencimento
- **Gateway**: Verifica status no gateway de pagamento (Stripe/MercadoPago)

**Monitoramento**:
```bash
# Verificar renovações processadas
grep "SubscriptionRenewalWorker" logs/araponga-*.log | tail -20

# Verificar falhas de renovação
grep "Failed to renew subscription" logs/araponga-*.log
```

#### PayoutProcessingWorker
- **Função**: Processa payouts para vendedores
- **Intervalo**: Configurável (padrão: 1 hora)
- **Gateway**: Integração com gateway de payout

#### EventReminderWorker
- **Função**: Envia lembretes de eventos
- **Intervalo**: 1 hora
- **Janela**: Eventos que começam nas próximas 24 horas

### Limpeza e Retenção de Dados

#### Cache Redis
- **TTL**: Configurado por tipo de cache
- **Limpeza**: Automática via TTL
- **Monitoramento**: Verificar uso de memória Redis

**Comandos Úteis**:
```bash
# Verificar uso de memória
redis-cli INFO memory

# Limpar cache manualmente (emergência)
redis-cli FLUSHDB
```

#### Outbox Messages
- **Retenção**: 30 dias após processamento
- **Limpeza**: Script manual ou job agendado

**Script de Limpeza**:
```sql
-- Limpar mensagens processadas há mais de 30 dias
DELETE FROM outbox_messages 
WHERE processed_at_utc IS NOT NULL 
  AND processed_at_utc < NOW() - INTERVAL '30 days';
```

#### Email Queue
- **Retenção**: 7 dias após processamento
- **Dead Letter**: Após 3 tentativas falhas
- **Limpeza**: Script manual ou job agendado

**Script de Limpeza**:
```sql
-- Limpar emails processados há mais de 7 dias
DELETE FROM email_queue_items 
WHERE status = 'Completed' 
  AND completed_at_utc < NOW() - INTERVAL '7 days';

-- Limpar dead letters antigos (após análise)
DELETE FROM email_queue_items 
WHERE status = 'Failed' 
  AND attempts >= 3
  AND failed_at_utc < NOW() - INTERVAL '30 days';
```

#### Logs
- **Retenção**: 30 dias (configurado no Serilog)
- **Rotação**: Diária
- **Limpeza**: Automática via `retainedFileCountLimit`

### Backup

#### Banco de Dados (PostgreSQL)

**Frequência**: Diária (recomendado)  
**Retenção**: 30 dias (recomendado)  
**Ferramenta**: `pg_dump` ou ferramenta de backup do PostgreSQL

**Script de Backup**:
```bash
#!/bin/bash
# backup-postgres.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/postgres"
DB_NAME="araponga"
DB_USER="araponga"
DB_HOST="localhost"

# Criar diretório se não existir
mkdir -p $BACKUP_DIR

# Backup completo
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME -F c -f $BACKUP_DIR/araponga_$DATE.backup

# Comprimir
gzip $BACKUP_DIR/araponga_$DATE.backup

# Remover backups antigos (manter últimos 30 dias)
find $BACKUP_DIR -name "*.backup.gz" -mtime +30 -delete

echo "Backup concluído: araponga_$DATE.backup.gz"
```

**Restauração**:
```bash
# Restaurar backup
pg_restore -h localhost -U araponga -d araponga -c araponga_YYYYMMDD_HHMMSS.backup
```

**Backup Contínuo** (WAL Archiving):
- Configurar `archive_mode = on` no PostgreSQL
- Configurar `archive_command` para copiar WAL files
- Permite Point-in-Time Recovery (PITR)

#### Storage (Mídias)

**S3**:
- **Replicação**: Configurar replicação cross-region no S3
- **Versionamento**: Habilitar versionamento para recuperação
- **Lifecycle**: Configurar políticas de lifecycle para arquivamento

**Local**:
- **Backup**: Rsync ou ferramenta similar para backup incremental
- **Frequência**: Diária ou contínua

**Script de Backup Local**:
```bash
#!/bin/bash
# backup-media.sh

SOURCE_DIR="/app/wwwroot/media"
BACKUP_DIR="/backups/media"
DATE=$(date +%Y%m%d)

mkdir -p $BACKUP_DIR

# Backup incremental
rsync -av --delete $SOURCE_DIR/ $BACKUP_DIR/$DATE/

# Comprimir backup diário
tar -czf $BACKUP_DIR/media_$DATE.tar.gz -C $BACKUP_DIR $DATE/

# Remover backups antigos (manter últimos 30 dias)
find $BACKUP_DIR -name "media_*.tar.gz" -mtime +30 -delete
```

### Manutenção Preventiva

#### Verificações Diárias
- ✅ Health checks retornando 200
- ✅ Workers processando normalmente
- ✅ Logs sem erros críticos
- ✅ Uso de recursos (CPU, memória, disco) dentro dos limites

#### Verificações Semanais
- ✅ Backup de banco de dados executado com sucesso
- ✅ Backup de mídias executado com sucesso
- ✅ Limpeza de dados antigos executada
- ✅ Revisão de métricas de performance

#### Verificações Mensais
- ✅ Revisão de logs para padrões de erro
- ✅ Análise de métricas de negócio
- ✅ Verificação de segurança (tokens, secrets)
- ✅ Atualização de dependências (se aplicável)

---

## 🐛 Troubleshooting

### Problemas Comuns

#### 1. Erro de Conexão com PostgreSQL

**Sintomas**:
- Health check retorna 503
- Logs mostram "Connection refused" ou timeout

**Solução**:
1. Verificar se PostgreSQL está rodando
2. Verificar `ConnectionStrings__DefaultConnection`
3. Verificar firewall/rede
4. Verificar credenciais

#### 2. Cache Redis Indisponível

**Sintomas**:
- Aplicação continua funcionando (fallback)
- Logs mostram erros de conexão Redis
- Performance degradada

**Solução**:
1. Verificar se Redis está rodando
2. Verificar `Redis__ConnectionString`
3. Aplicação funciona sem cache (degradação graciosa)

#### 3. Emails Não Sendo Enviados

**Sintomas**:
- Emails na fila não são processados
- Logs mostram erros SMTP

**Solução**:
1. Verificar configuração SMTP
2. Verificar se `EmailQueueWorker` está rodando
3. Verificar logs do worker
4. Verificar rate limiting (100 emails/minuto)

#### 4. Storage (Mídias) Indisponível

**Sintomas**:
- Uploads falham
- Health check retorna 503 para storage

**Solução**:
1. Verificar credenciais S3
2. Verificar conectividade com endpoint
3. Verificar permissões do bucket

### Logs Importantes

#### Verificar Status de Workers

```bash
# Buscar logs do EmailQueueWorker
grep "EmailQueueWorker" logs/app.log

# Buscar logs do OutboxDispatcherWorker
grep "OutboxDispatcherWorker" logs/app.log
```

#### Verificar Erros

```bash
# Buscar erros críticos
grep "Critical" logs/app.log

# Buscar erros de email
grep "Email" logs/app.log | grep "Error"
```

---

## 📈 Escalabilidade

### Arquitetura Horizontal

**Load Balancer**:
- Distribuir requisições entre múltiplas instâncias
- Health check: `GET /health` a cada 30 segundos
- Sticky sessions: Não necessário (stateless)

**Múltiplas Instâncias**:
- Cada instância é stateless
- Compartilham: PostgreSQL, Redis, S3
- Workers podem rodar em instância dedicada ou distribuídos

### Otimizações Implementadas

- ✅ **Cache Redis**: Queries frequentes, feature flags, territórios
- ✅ **Connection Pooling**: PostgreSQL (Min: 5, Max: 100)
- ✅ **Índices Otimizados**: Chaves primárias, foreign keys, queries frequentes
- ✅ **Paginação**: Todas as listagens com paginação
- ✅ **Response Compression**: Gzip automático
- ✅ **Rate Limiting**: Por endpoint e por usuário
- ✅ **Async Processing**: Background workers para operações pesadas

### Escalando Componentes

#### API (Horizontal)
- Adicionar mais instâncias atrás do load balancer
- Cada instância: 2-4 CPUs, 4-8GB RAM (depende do tráfego)

#### PostgreSQL (Vertical/Horizontal)
- **Vertical**: Aumentar CPU/RAM da instância
- **Horizontal**: Read replicas para queries de leitura
- **Connection Pooling**: Ajustar `Maximum Pool Size` conforme número de instâncias

#### Redis (Cluster)
- **Standalone**: Até ~10GB de dados
- **Cluster**: Para escalar além de 10GB ou alta disponibilidade
- **Sentinel**: Para failover automático

#### Storage (S3)
- Escala automaticamente
- Considerar CDN para mídias estáticas

### Monitoramento de Performance

**Métricas Principais**:
- **Latência P50, P95, P99**: Via Prometheus `http_request_duration_seconds`
- **Throughput**: Requisições por segundo
- **Taxa de Erro**: Erros 5xx / Total de requisições
- **Cache Hit Rate**: `cache_hits / (cache_hits + cache_misses)`
- **Database Connection Pool**: Conexões ativas vs. disponíveis

**Alertas de Performance**:
- Latência P95 > 2 segundos
- Taxa de erro > 1%
- Cache hit rate < 70%
- Connection pool esgotado
- CPU > 80% por 10 minutos

---

## 🔐 Segurança Operacional

### Configurações Críticas

#### Autenticação e Autorização
- ✅ **JWT**: Secret forte (mínimo 32 caracteres)
- ✅ **Expiração**: 60 minutos (configurável)
- ✅ **Refresh Tokens**: Implementar se necessário
- ✅ **2FA**: Suportado (Fase 5)

#### Rate Limiting
- ✅ **Ativo**: Por endpoint e por usuário
- ✅ **Limites**: Configuráveis por ambiente
- ✅ **Headers**: `Retry-After` retornado quando limite excedido

#### Security Headers
- ✅ **CSP**: Content Security Policy configurado
- ✅ **HSTS**: HTTP Strict Transport Security
- ✅ **X-Frame-Options**: DENY
- ✅ **X-Content-Type-Options**: nosniff
- ✅ **X-XSS-Protection**: 1; mode=block

#### Validação e Sanitização
- ✅ **Input Validation**: FluentValidation em todos os endpoints
- ✅ **Sanitização**: HTML sanitization para conteúdo do usuário
- ✅ **SQL Injection**: Protegido via EF Core (parameterized queries)
- ✅ **XSS**: Sanitização de conteúdo HTML

### Gestão de Secrets

#### Secrets Críticos
- `JWT__SIGNINGKEY` - **NUNCA** commitar no código
- `ConnectionStrings__Postgres` - Credenciais do banco
- `Storage__S3__SecretKey` - Credenciais S3
- `Email__Smtp__Password` - Senha SMTP

#### Boas Práticas
- ✅ Usar variáveis de ambiente ou secret manager (Azure Key Vault, AWS Secrets Manager)
- ✅ Rotacionar secrets periodicamente
- ✅ Não logar secrets (configurado no Serilog)
- ✅ Usar diferentes secrets por ambiente (dev, staging, prod)

### Auditoria

#### Logs de Autenticação
- Login bem-sucedido
- Login falho (com IP e user agent)
- Logout
- Token expirado/inválido

#### Logs de Ações Administrativas
- Criação/atualização de planos de assinatura
- Criação/atualização de cupons
- Mudanças em políticas de termos
- Ações de moderação (sanções, bloqueios)

#### Histórico de Mudanças
- **Planos**: `SubscriptionPlanHistory` (tabela dedicada)
- **Políticas**: Timestamps em `TermsOfService` e `PrivacyPolicy`
- **Configurações**: Logs de mudanças em `SystemConfig`

### Compliance (LGPD)

#### Exportação de Dados
- **Endpoint**: `GET /api/v1/users/me/export`
- **Formato**: JSON com todos os dados do usuário
- **Prazo Legal**: 15 dias úteis

#### Exclusão de Conta
- **Endpoint**: `DELETE /api/v1/users/me`
- **Processo**: Anonimização ou exclusão completa
- **Retenção**: Conforme política legal

#### Políticas de Termos
- **Obrigatório**: Usuários devem aceitar termos ativos
- **Bloqueio**: Funcionalidades bloqueadas até aceite
- **Histórico**: Aceites registrados com timestamp e IP

---

## 🚨 Procedimentos de Emergência

### Incidente: API Indisponível

**Sintomas**: Health check retorna 503, aplicação não responde

**Ações Imediatas**:
1. Verificar logs: `tail -f logs/araponga-*.log`
2. Verificar recursos: CPU, memória, disco
3. Verificar dependências: PostgreSQL, Redis, Storage
4. Restart da aplicação (se necessário)
5. Escalar para instâncias adicionais (se load balancer disponível)

**Rollback**:
```bash
# Parar aplicação atual
systemctl stop araponga-api

# Reverter para versão anterior (se deploy recente)
# Restaurar backup do banco (se corrupção de dados)
pg_restore -h localhost -U araponga -d araponga backup_anterior.backup

# Reiniciar aplicação
systemctl start araponga-api
```

### Incidente: Banco de Dados Indisponível

**Sintomas**: Health check PostgreSQL falha, queries timeout

**Ações Imediatas**:
1. Verificar status do PostgreSQL: `systemctl status postgresql`
2. Verificar logs do PostgreSQL: `/var/log/postgresql/`
3. Verificar espaço em disco: `df -h`
4. Verificar conexões: `SELECT count(*) FROM pg_stat_activity;`
5. Matar conexões antigas se necessário: `SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE state = 'idle' AND now() - state_change > interval '1 hour';`

**Recuperação**:
- Se PostgreSQL crashou: Reiniciar serviço
- Se disco cheio: Limpar logs, backups antigos, ou expandir disco
- Se corrupção: Restaurar do último backup

### Incidente: Cache Redis Indisponível

**Sintomas**: Logs mostram erros de conexão Redis, performance degradada

**Impacto**: Baixo - Aplicação funciona com cache em memória (degradação graciosa)

**Ações**:
1. Verificar status do Redis: `systemctl status redis`
2. Reiniciar Redis se necessário
3. Aplicação continua funcionando (fallback para cache em memória)

### Incidente: Storage (Mídias) Indisponível

**Sintomas**: Uploads falham, health check storage retorna 503

**Ações Imediatas**:
1. Verificar credenciais S3
2. Verificar conectividade: `curl <s3_endpoint>`
3. Verificar permissões do bucket
4. Verificar quota/limites do S3

**Workaround**:
- Aplicação pode funcionar sem storage (algumas funcionalidades desabilitadas)
- Usuários não conseguirão fazer uploads até resolução

### Incidente: Workers Não Processando

**Sintomas**: Fila de emails/outbox crescendo, nenhum processamento

**Ações Imediatas**:
1. Verificar se workers estão rodando: `ps aux | grep Worker`
2. Verificar logs dos workers
3. Reiniciar workers se necessário
4. Verificar recursos (CPU, memória)

**Recuperação**:
- Workers são reiniciados automaticamente com a aplicação
- Processamento retoma do ponto onde parou

---

## ✅ Checklist de Produção

### Pré-Deploy

- [ ] Variáveis de ambiente configuradas
- [ ] Secrets configurados (JWT, DB, S3, SMTP)
- [ ] Banco de dados migrado
- [ ] Seed inicial executado (plano FREE)
- [ ] Health checks configurados no load balancer
- [ ] Monitoramento configurado (Prometheus, logs)
- [ ] Backup configurado (banco e mídias)
- [ ] Rate limiting ajustado para produção
- [ ] CORS configurado com origens corretas
- [ ] SSL/TLS configurado

### Pós-Deploy

- [ ] Health checks retornando 200
- [ ] Workers processando normalmente
- [ ] Logs sendo gerados corretamente
- [ ] Métricas sendo coletadas
- [ ] Testes de smoke básicos passando
- [ ] Backup inicial executado com sucesso

### Monitoramento Contínuo

- [ ] Health checks verificados diariamente
- [ ] Logs revisados para erros
- [ ] Métricas de performance monitoradas
- [ ] Uso de recursos dentro dos limites
- [ ] Backups executando com sucesso
- [ ] Workers processando normalmente

---

## 📞 Suporte e Contatos

### Documentação

- **Documentação Técnica**: `docs/` no repositório
- **API Docs**: `/swagger` (desenvolvimento) ou `/devportal` (produção)
- **Health Dashboard**: `/health` (JSON) ou `/health/index.html` (HTML)

### Logs e Debugging

- **Logs da Aplicação**: `logs/araponga-YYYYMMDD.log`
- **Logs do PostgreSQL**: `/var/log/postgresql/` (Linux)
- **Logs do Redis**: Configurado no Redis
- **Métricas**: `http://<host>:9090/metrics` (Prometheus)

### Issues e Suporte

- **GitHub Issues**: Para bugs e feature requests
- **Documentação de Fases**: `docs/backlog-api/` para roadmap
- **Status de Implementação**: `docs/STATUS_FASES.md`

---

## 📚 Referências Rápidas

### Comandos Úteis

```bash
# Verificar status da aplicação
curl http://localhost:5000/health

# Verificar métricas
curl http://localhost:9090/metrics

# Verificar logs em tempo real
tail -f logs/araponga-$(date +%Y%m%d).log

# Verificar workers
ps aux | grep -E "Worker|BackgroundService"

# Verificar conexões PostgreSQL
psql -U araponga -d araponga -c "SELECT count(*) FROM pg_stat_activity;"

# Verificar cache Redis
redis-cli INFO stats
```

### Endpoints Importantes

- `GET /health` - Health check
- `GET /metrics` - Métricas Prometheus
- `GET /swagger` - Swagger UI (desenvolvimento)
- `GET /devportal` - Developer Portal
- `GET /api/v1/analytics/platform/stats` - Stats da plataforma
- `GET /api/v1/analytics/marketplace/stats` - Stats do marketplace

---

**Última Atualização**: 2026-01-26  
**Versão da Documentação**: 1.0  
**Próxima Revisão**: Após mudanças significativas na infraestrutura
