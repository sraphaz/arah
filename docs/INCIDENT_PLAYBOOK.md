# Playbook de Incidentes - Araponga

Este documento contém procedimentos para responder a incidentes no sistema Araponga.

## 🚨 Classificação de Incidentes

### Severidade Crítica (P1)
- Sistema completamente indisponível
- Perda de dados
- Segurança comprometida

**Tempo de Resposta**: Imediato  
**Tempo de Resolução**: 1 hora

### Severidade Alta (P2)
- Funcionalidade crítica indisponível
- Performance severamente degradada (> 5s latência)
- Muitos usuários afetados

**Tempo de Resposta**: 15 minutos  
**Tempo de Resolução**: 4 horas

### Severidade Média (P3)
- Funcionalidade parcialmente indisponível
- Performance moderadamente degradada
- Poucos usuários afetados

**Tempo de Resposta**: 1 hora  
**Tempo de Resolução**: 1 dia

### Severidade Baixa (P4)
- Problemas menores
- Melhorias
- Documentação

**Tempo de Resposta**: 1 dia  
**Tempo de Resolução**: 1 semana

---

## 📋 Procedimento de Resposta

### 1. Identificação

**Sinais de Incidente**:
- Alertas do Prometheus/Grafana
- Notificações de usuários
- Monitoramento mostrando anomalias

**Ações Imediatas**:
1. Verificar dashboards de monitoramento
2. Verificar logs recentes
3. Verificar health checks
4. Classificar severidade

---

### 2. Contenção

**Objetivo**: Prevenir que o incidente se espalhe ou piore.

**Ações**:
1. **Se sistema indisponível**:
   - Verificar se é problema de infraestrutura
   - Considerar rollback se deploy recente
   - Verificar se é problema de banco de dados

2. **Se performance degradada**:
   - Verificar métricas de latência
   - Verificar queries lentas
   - Considerar escalar recursos

3. **Se dados comprometidos**:
   - Isolar sistema se necessário
   - Verificar backups
   - Documentar estado atual

---

### 3. Diagnóstico

**Checklist de Diagnóstico**:

- [ ] Health checks (`/health/ready`)
- [ ] Logs recentes (últimas 100 linhas)
- [ ] Métricas (request rate, error rate, latência)
- [ ] Cache hit rate
- [ ] Database connections
- [ ] Redis status (se configurado)
- [ ] Último deploy (quando foi?)
- [ ] Mudanças recentes (config, código)

**Ferramentas**:
```bash
# Health
curl https://api.araponga.com/health/ready

# Métricas
curl http://localhost:9090/metrics

# Logs
docker logs araponga-api --tail 100

# Database
psql -h localhost -U araponga -d araponga -c "SELECT COUNT(*) FROM outbox_messages WHERE processed_at_utc IS NULL;"
```

---

### 4. Resolução

**Estratégias Comuns**:

#### Sistema Indisponível
1. Verificar se é problema de infraestrutura
2. Rollback se deploy recente
3. Restart da aplicação
4. Verificar banco de dados

#### Performance Degradada
1. Verificar queries lentas
2. Verificar cache hit rate
3. Escalar recursos se necessário
4. Otimizar queries problemáticas

#### Erros 500
1. Verificar logs para exception específica
2. Verificar configuração
3. Verificar dependências (banco, Redis)
4. Aplicar hotfix se necessário

---

### 5. Pós-Incidente

**Ações Obrigatórias**:

1. **Documentar Incidente**:
   - O que aconteceu?
   - Quando aconteceu?
   - Como foi resolvido?
   - Tempo de resolução

2. **Post-Mortem** (para P1/P2):
   - Root cause analysis
   - Ações preventivas
   - Melhorias no sistema
   - Atualizar runbook se necessário

3. **Comunicação**:
   - Notificar stakeholders
   - Atualizar status page (se houver)
   - Documentar lições aprendidas

---

## 🔧 Procedimentos Específicos

### Sistema Completamente Indisponível

1. **Verificar Infraestrutura**:
   ```bash
   # Docker
   docker ps
   docker logs araponga-api
   
   # Kubernetes
   kubectl get pods
   kubectl describe pod <pod-name>
   ```

2. **Verificar Banco de Dados**:
   ```bash
   psql -h localhost -U araponga -d araponga -c "SELECT 1;"
   ```

3. **Rollback Imediato** (se deploy recente):
   ```bash
   kubectl rollout undo deployment/araponga-api
   ```

---

### Perda de Dados

1. **Isolar Sistema** (se necessário)
2. **Verificar Backups**:
   ```bash
   ls -lh /backups/
   ```
3. **Restaurar Backup** (se necessário):
   ```bash
   psql -h localhost -U araponga -d araponga < backup_<timestamp>.sql
   ```
4. **Documentar** o que foi perdido

---

### Segurança Comprometida

1. **Isolar Sistema Imediatamente**
2. **Revogar Credenciais Comprometidas**
3. **Verificar Logs de Acesso**
4. **Notificar Equipe de Segurança**
5. **Documentar Incidente**

---

## 📞 Contatos de Emergência

- **DevOps**: devops@araponga.com
- **Desenvolvimento**: dev@araponga.com
- **Emergência**: +55 (11) 99999-9999

---

## 🔗 Links Relacionados

- [RUNBOOK.md](./RUNBOOK.md) - Runbook de operações
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Troubleshooting comum
- [MONITORING.md](./MONITORING.md) - Dashboards e alertas
