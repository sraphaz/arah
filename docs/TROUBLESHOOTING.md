# Troubleshooting - Araponga

Este documento contém soluções para problemas comuns do sistema Araponga.

## 🔍 Problemas Comuns

### 1. Aplicação não inicia

**Sintomas**:
- Aplicação não responde
- Erro ao iniciar

**Diagnóstico**:
```bash
# Verificar logs
docker logs araponga-api --tail 100

# Verificar variáveis de ambiente
env | grep -E "JWT__SIGNINGKEY|ConnectionStrings"
```

**Soluções**:
1. Verificar se `JWT__SIGNINGKEY` está configurado (obrigatório)
2. Verificar se `ConnectionStrings__Postgres` está configurado (se usando Postgres)
3. Verificar se porta não está em uso
4. Verificar logs para erros específicos

---

### 2. Erro 500 Internal Server Error

**Sintomas**:
- Requisições retornando 500
- Logs mostrando exceptions

**Diagnóstico**:
```bash
# Verificar logs recentes
docker logs araponga-api --tail 50 | grep -i error

# Verificar health checks
curl https://api.araponga.com/health
```

**Soluções**:
1. Verificar logs para exception específica
2. Verificar conexão com banco de dados
3. Verificar configuração de Redis (se configurado)
4. Verificar se migrations foram aplicadas

---

### 3. Alta Latência

**Sintomas**:
- Requisições lentas
- Timeouts

**Diagnóstico**:
```bash
# Verificar métricas
curl http://localhost:9090/metrics | grep http_requests_duration

# Verificar queries lentas (logs)
docker logs araponga-api | grep -i "slow"
```

**Soluções**:
1. Verificar cache hit rate (deve ser > 70%)
2. Verificar queries N+1
3. Verificar índices no banco de dados
4. Verificar uso de memória/CPU
5. Considerar read replicas para queries de leitura

---

### 4. Cache não funcionando

**Sintomas**:
- Cache hit rate baixo
- Queries repetidas ao banco

**Diagnóstico**:
```bash
# Verificar métricas de cache
curl http://localhost:9090/metrics | grep cache

# Verificar logs do Redis (se configurado)
docker logs redis | grep -i error
```

**Soluções**:
1. Verificar se Redis está rodando (se configurado)
2. Verificar connection string do Redis
3. Verificar se fallback para IMemoryCache está funcionando
4. Verificar TTLs de cache

---

### 5. Conflitos de Concorrência

**Sintomas**:
- `DbUpdateConcurrencyException` nos logs
- Operações falhando com "concurrency conflict"

**Diagnóstico**:
```bash
# Verificar métricas
curl http://localhost:9090/metrics | grep concurrency
```

**Soluções**:
1. Verificar se `RowVersion` está sendo atualizado corretamente
2. Implementar retry logic usando `ConcurrencyHelper`
3. Verificar se entidades estão sendo rastreadas corretamente no EF Core

---

### 6. Eventos não sendo processados

**Sintomas**:
- Eventos na dead letter queue
- Handlers não executando

**Diagnóstico**:
```bash
# Verificar logs do BackgroundEventProcessor
docker logs araponga-api | grep -i "BackgroundEventProcessor"

# Verificar métricas
curl http://localhost:9090/metrics | grep events
```

**Soluções**:
1. Verificar se `BackgroundEventProcessor` está registrado
2. Verificar se handlers estão registrados no DI
3. Verificar dead letter queue
4. Verificar logs para erros específicos nos handlers

---

### 7. Problemas de Autenticação

**Sintomas**:
- Erro 401 Unauthorized
- Tokens inválidos

**Diagnóstico**:
```bash
# Verificar JWT secret
env | grep JWT__SIGNINGKEY

# Verificar logs
docker logs araponga-api | grep -i "jwt\|auth"
```

**Soluções**:
1. Verificar se `JWT__SIGNINGKEY` está configurado e tem pelo menos 32 caracteres
2. Verificar se token não expirou
3. Verificar formato do token (Bearer token)
4. Verificar se usuário existe no sistema

---

### 8. Problemas de Performance

**Sintomas**:
- Requisições lentas
- Timeouts
- Alta utilização de recursos

**Diagnóstico**:
```bash
# Verificar métricas
curl http://localhost:9090/metrics

# Verificar uso de recursos
docker stats araponga-api
```

**Soluções**:
1. Verificar queries N+1
2. Verificar índices no banco
3. Verificar cache hit rate
4. Considerar read replicas
5. Verificar connection pooling
6. Verificar rate limiting (pode estar limitando muito)

---

## 🔧 Comandos Úteis

### Verificar Health
```bash
curl https://api.araponga.com/health/ready
```

### Verificar Métricas
```bash
curl http://localhost:9090/metrics
```

### Verificar Logs
```bash
# Docker
docker logs araponga-api --tail 100 -f

# Kubernetes
kubectl logs -f deployment/araponga-api
```

### Verificar Cache
```bash
# Redis
redis-cli
> KEYS *
> GET <key>
```

### Verificar Banco de Dados
```bash
# PostgreSQL
psql -h localhost -U araponga -d araponga
> \dt
> SELECT COUNT(*) FROM community_posts;
```

---

## 🔗 Links Relacionados

- [RUNBOOK.md](./RUNBOOK.md) - Runbook de operações
- [INCIDENT_PLAYBOOK.md](./INCIDENT_PLAYBOOK.md) - Playbook de incidentes
- [MONITORING.md](./MONITORING.md) - Dashboards e alertas
