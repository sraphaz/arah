# Runbook de Operações - Araponga

Este documento contém procedimentos operacionais para o sistema Araponga.

## 📋 Índice

1. [Deploy](#deploy)
2. [Rollback](#rollback)
3. [Escalação](#escalação)
4. [Manutenção](#manutenção)
5. [Backup e Restore](#backup-e-restore)

---

## 🚀 Deploy

### Deploy em Produção

1. **Verificar Pré-requisitos**:
   ```bash
   # Verificar variáveis de ambiente
   env | grep -E "JWT__SIGNINGKEY|CORS__ALLOWEDORIGINS|ConnectionStrings"
   
   # Verificar health checks
   curl https://api.araponga.com/health
   ```

2. **Executar Migrations**:
   ```bash
   dotnet ef database update --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api
   ```

3. **Deploy da Aplicação**:
   ```bash
   # Docker
   docker-compose up -d --build
   
   # Kubernetes
   kubectl apply -f k8s/
   kubectl rollout status deployment/araponga-api
   ```

4. **Verificar Deploy**:
   ```bash
   # Health check
   curl https://api.araponga.com/health/ready
   
   # Verificar logs
   docker logs araponga-api --tail 100
   ```

---

## ⏪ Rollback

### Rollback Rápido

1. **Identificar Versão Anterior**:
   ```bash
   git log --oneline -10
   ```

2. **Rollback**:
   ```bash
   # Docker
   docker-compose down
   git checkout <commit-anterior>
   docker-compose up -d --build
   
   # Kubernetes
   kubectl rollout undo deployment/araponga-api
   ```

3. **Verificar Rollback**:
   ```bash
   curl https://api.araponga.com/health/ready
   ```

### Rollback de Migrations

**⚠️ ATENÇÃO**: Rollback de migrations pode causar perda de dados. Sempre fazer backup antes.

```bash
# Listar migrations
dotnet ef migrations list --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api

# Rollback para migration específica
dotnet ef database update <MigrationName> --project backend/Araponga.Infrastructure --startup-project backend/Araponga.Api
```

---

## 📞 Escalação

### Níveis de Escalação

1. **Nível 1 - Equipe de Desenvolvimento**
   - Problemas de funcionalidade
   - Bugs não críticos
   - Melhorias

2. **Nível 2 - DevOps/Infraestrutura**
   - Problemas de infraestrutura
   - Performance degradada
   - Problemas de deploy

3. **Nível 3 - Emergência**
   - Sistema indisponível
   - Perda de dados
   - Segurança comprometida

### Contatos

- **Desenvolvimento**: dev@araponga.com
- **DevOps**: devops@araponga.com
- **Emergência**: +55 (11) 99999-9999

---

## 🔧 Manutenção

### Manutenção Programada

1. **Notificar Usuários** (24h antes)
2. **Backup Completo**
3. **Executar Manutenção**
4. **Verificar Sistema**
5. **Notificar Conclusão**

### Limpeza de Logs

```bash
# Logs são rotacionados automaticamente (30 dias)
# Limpeza manual se necessário:
find logs/ -name "*.log" -mtime +30 -delete
```

### Limpeza de Cache

```bash
# Redis
redis-cli FLUSHDB

# IMemoryCache (reiniciar aplicação)
```

---

## 💾 Backup e Restore

### Backup do Banco de Dados

```bash
# PostgreSQL
pg_dump -h localhost -U araponga -d araponga > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup automático (cron)
0 2 * * * pg_dump -h localhost -U araponga -d araponga > /backups/araponga_$(date +\%Y\%m\%d).sql
```

### Restore do Banco de Dados

```bash
# PostgreSQL
psql -h localhost -U araponga -d araponga < backup_20250115_020000.sql
```

### Backup de Configuração

```bash
# Backup de appsettings.json e variáveis de ambiente
cp appsettings.Production.json appsettings.Production.json.backup
env | grep -E "JWT|CORS|ConnectionStrings" > env_backup.txt
```

---

## 🔍 Verificações Pós-Deploy

### Checklist

- [ ] Health checks passando (`/health/ready`)
- [ ] Métricas sendo coletadas (`/metrics`)
- [ ] Logs sendo gerados corretamente
- [ ] Cache funcionando
- [ ] Database conectado
- [ ] Redis conectado (se configurado)
- [ ] Sem erros nos logs recentes

---

## 🖥️ Interface Web de Monitoramento

A aplicação possui uma **interface web integrada** para monitoramento e auxílio à produção.

**Acesso**: `https://api.araponga.com/admin/monitoring` (requer autenticação e autorização)

**Funcionalidades**:
- ✅ Dashboard principal com status geral
- ✅ Visualizador de logs em tempo real
- ✅ Métricas e dashboards interativos
- ✅ Health checks visuais
- ✅ Troubleshooting assistido

**Ver documentação completa**: [`LOGS_MONITORAMENTO_ARQUITETURA.md`](./LOGS_MONITORAMENTO_ARQUITETURA.md)

---

## 🔗 Links Relacionados

- **Arquitetura de Logs e Monitoramento**: [`LOGS_MONITORAMENTO_ARQUITETURA.md`](./LOGS_MONITORAMENTO_ARQUITETURA.md) - Documentação completa sobre logs e monitoramento em diferentes arquiteturas, incluindo interface web
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Troubleshooting comum
- [INCIDENT_PLAYBOOK.md](./INCIDENT_PLAYBOOK.md) - Playbook de incidentes
- [METRICS.md](./METRICS.md) - Métricas disponíveis
- [MONITORING.md](./MONITORING.md) - Dashboards e alertas
- [FASE4.md](./backlog-api/FASE4.md) - Fase 4: Observabilidade e Monitoramento

---

**Última Atualização**: 2026-01-28  
**Status**: 📋 Runbook Completo - Atualizado com Interface Web