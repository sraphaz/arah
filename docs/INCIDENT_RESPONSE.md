# Plano de Resposta a Incidentes - Araponga

**Última Atualização**: 2026-01-21  
**Versão**: 1.0

---

## 📋 Índice

1. [Classificação de Incidentes](#classificação-de-incidentes)
2. [Processo de Resposta](#processo-de-resposta)
3. [Procedimentos por Tipo de Incidente](#procedimentos-por-tipo-de-incidente)
4. [Comunicação](#comunicação)
5. [Pós-Incidente](#pós-incidente)

---

## 1. Classificação de Incidentes

### Severidade P0 - Crítico
- **Impacto**: Sistema completamente indisponível
- **Tempo de Resposta**: Imediato (< 15 minutos)
- **Tempo de Resolução**: < 1 hora
- **Exemplos**: 
  - API completamente offline
  - Banco de dados inacessível
  - Perda de dados

### Severidade P1 - Alto
- **Impacto**: Funcionalidade crítica degradada
- **Tempo de Resposta**: < 1 hora
- **Tempo de Resolução**: < 4 horas
- **Exemplos**:
  - Autenticação falhando
  - Feed não carregando
  - Marketplace offline

### Severidade P2 - Médio
- **Impacto**: Funcionalidade não crítica degradada
- **Tempo de Resposta**: < 4 horas
- **Tempo de Resolução**: < 24 horas
- **Exemplos**:
  - Analytics não funcionando
  - Notificações atrasadas
  - Performance degradada

### Severidade P3 - Baixo
- **Impacto**: Funcionalidade menor afetada
- **Tempo de Resposta**: < 24 horas
- **Tempo de Resolução**: < 72 horas
- **Exemplos**:
  - Erros em endpoints não críticos
  - Warnings em logs
  - Melhorias de UX

---

## 2. Processo de Resposta

### 2.1 Detecção

**Fontes de Detecção**:
- Alertas de monitoramento (Prometheus, Grafana)
- Logs de erro (Seq, arquivos)
- Relatórios de usuários
- Health checks falhando

### 2.2 Triagem

1. **Classificar severidade** (P0-P3)
2. **Identificar escopo** (quais funcionalidades afetadas)
3. **Estimar impacto** (quantos usuários afetados)
4. **Atribuir responsável** (on-call engineer)

### 2.3 Investigação

1. **Coletar informações**:
   - Logs relevantes
   - Métricas de monitoramento
   - Status de dependências (DB, Redis, etc.)
   - Últimas mudanças (deploy, configuração)

2. **Identificar causa raiz**:
   - Analisar stack traces
   - Verificar queries lentas
   - Verificar uso de recursos (CPU, memória, conexões)

### 2.4 Mitigação

1. **Ações imediatas** (se necessário):
   - Rollback de deploy recente
   - Reiniciar instâncias problemáticas
   - Escalar recursos (CPU, memória)

2. **Correção temporária**:
   - Hotfix se possível
   - Workaround documentado

### 2.5 Resolução

1. **Implementar correção permanente**
2. **Validar correção** (testes, monitoramento)
3. **Documentar incidente** (post-mortem)

---

## 3. Procedimentos por Tipo de Incidente

### 3.1 API Offline (P0)

**Sintomas**:
- Health checks retornando 503/500
- Todas as requisições falhando
- Logs mostrando crashes

**Ações**:
1. Verificar status de containers/pods
2. Verificar logs de crash
3. Verificar dependências (PostgreSQL, Redis)
4. Se necessário, reiniciar instâncias
5. Se persistir, fazer rollback

**Comandos Úteis**:
```bash
# Verificar containers
docker ps -a | grep araponga

# Verificar logs
docker logs araponga-api --tail 100

# Verificar health
curl http://localhost:8080/health

# Reiniciar
docker restart araponga-api
```

### 3.2 Banco de Dados Inacessível (P0)

**Sintomas**:
- Erros de conexão no banco
- Timeouts em queries
- Health check de DB falhando

**Ações**:
1. Verificar status do PostgreSQL
2. Verificar conexões ativas (não exceder pool)
3. Verificar espaço em disco
4. Verificar locks/deadlocks
5. Se necessário, reiniciar PostgreSQL (com cuidado!)

**Comandos Úteis**:
```bash
# Verificar conexões
psql -h [host] -U [user] -d araponga -c "SELECT count(*) FROM pg_stat_activity;"

# Verificar locks
psql -h [host] -U [user] -d araponga -c "SELECT * FROM pg_locks WHERE NOT granted;"

# Verificar espaço
psql -h [host] -U [user] -d araponga -c "SELECT pg_size_pretty(pg_database_size('araponga'));"
```

### 3.3 Performance Degradada (P1-P2)

**Sintomas**:
- Latência alta (P95 > 1000ms)
- Timeouts frequentes
- CPU/memória alta

**Ações**:
1. Identificar endpoints lentos (métricas Prometheus)
2. Analisar queries lentas (logs EF Core)
3. Verificar cache hit rate
4. Verificar uso de recursos
5. Escalar se necessário (mais instâncias, mais recursos)

**Comandos Úteis**:
```bash
# Verificar métricas de latência
curl http://localhost:8080/metrics | grep http_request_duration

# Verificar queries lentas (habilitar logging EF Core)
# Ver logs com queries > 1000ms

# Verificar cache
curl http://localhost:8080/metrics | grep cache
```

### 3.4 Erro de Autenticação (P1)

**Sintomas**:
- Usuários não conseguem fazer login
- Tokens JWT inválidos
- 401 Unauthorized em massa

**Ações**:
1. Verificar configuração de JWT (`JWT__SIGNINGKEY`)
2. Verificar expiração de tokens
3. Verificar logs de autenticação
4. Se necessário, invalidar todos os tokens (forçar re-login)

**Comandos Úteis**:
```bash
# Verificar configuração JWT
env | grep JWT__SIGNINGKEY

# Verificar logs de auth
docker logs araponga-api | grep -i "auth\|jwt\|unauthorized"
```

### 3.5 Perda de Dados (P0)

**Sintomas**:
- Dados ausentes após operação
- Inconsistências no banco
- Backups corrompidos

**Ações**:
1. **NÃO FAZER NADA QUE POSSA PIORAR**
2. Documentar exatamente o que foi perdido
3. Verificar backups disponíveis
4. Avaliar se restore é necessário
5. Se restore, planejar cuidadosamente (pode perder dados mais recentes)

**⚠️ IMPORTANTE**: Sempre consultar com time antes de fazer restore em produção.

---

## 4. Comunicação

### 4.1 Durante o Incidente

- **Status Page**: Atualizar status page (se disponível)
- **Canal Interno**: Notificar time via Slack/Teams
- **Usuários**: Se P0/P1, comunicar via email/notificação in-app

### 4.2 Template de Comunicação

```
[SEVERIDADE] Incidente: [Descrição Breve]
Status: [Investigando/Mitigando/Resolvido]
Impacto: [Descrição do impacto]
Ações: [O que está sendo feito]
ETA: [Estimativa de resolução]
```

### 4.3 Pós-Resolução

- **Post-Mortem**: Documentar incidente completo
- **Comunicação Final**: Notificar resolução
- **Ações Preventivas**: Implementar melhorias

---

## 5. Pós-Incidente

### 5.1 Post-Mortem

**Template**:
1. **Resumo**: O que aconteceu?
2. **Timeline**: Quando aconteceu?
3. **Causa Raiz**: Por que aconteceu?
4. **Impacto**: Quem foi afetado?
5. **Ações Corretivas**: O que foi feito?
6. **Ações Preventivas**: O que será feito para evitar?

### 5.2 Ações Preventivas

- Implementar alertas adicionais
- Melhorar monitoramento
- Adicionar testes
- Melhorar documentação
- Treinar time

### 5.3 Métricas de Incidente

- **MTTR** (Mean Time To Resolve): Tempo médio de resolução
- **MTBF** (Mean Time Between Failures): Tempo médio entre falhas
- **Número de incidentes por mês**: Por severidade

---

## 6. Contatos de Emergência

| Função | Contato | Disponibilidade |
|--------|---------|-----------------|
| On-Call Engineer | [definir] | 24/7 |
| Tech Lead | [definir] | Horário comercial |
| DevOps | [definir] | Horário comercial |

---

**Última Atualização**: 2026-01-21
