# Monitoramento do Araponga

Este documento descreve dashboards e alertas configurados para monitoramento do Araponga.

## 📊 Dashboards

### Dashboard de Performance

**Objetivo**: Monitorar performance da API e latência de requisições.

**Métricas Principais**:
- Request rate (req/s)
- Error rate (%)
- Latência (P50, P95, P99)
- Throughput (bytes/s)

**Queries Prometheus**:
```promql
# Request Rate
rate(http_requests_received_total[5m])

# Error Rate
rate(http_requests_received_total{code=~"5.."}[5m]) / rate(http_requests_received_total[5m])

# Latência P95
histogram_quantile(0.95, rate(http_requests_duration_seconds_bucket[5m]))

# Throughput
rate(http_response_size_bytes[5m])
```

---

### Dashboard de Negócio

**Objetivo**: Monitorar métricas de negócio e atividade dos usuários.

**Métricas Principais**:
- Posts criados por hora
- Eventos criados por hora
- Membros criados por hora
- Relatórios criados por hora
- Solicitações de entrada por hora

**Queries Prometheus**:
```promql
# Posts criados por hora
rate(araponga.posts.created[1h]) * 3600

# Eventos criados por hora
rate(araponga.events.created[1h]) * 3600

# Membros criados por hora
rate(araponga.memberships.created[1h]) * 3600
```

---

### Dashboard de Sistema

**Objetivo**: Monitorar saúde do sistema e recursos.

**Métricas Principais**:
- CPU usage
- Memory usage
- Database connections
- Cache hit rate
- Conflitos de concorrência
- Eventos processados/falhados

**Queries Prometheus**:
```promql
# Cache Hit Rate
rate(araponga.cache.hits[5m]) / (rate(araponga.cache.hits[5m]) + rate(araponga.cache.misses[5m]))

# Conflitos de Concorrência
rate(araponga.concurrency.conflicts[5m])

# Eventos Processados vs Falhados
rate(araponga.events.processed[5m])
rate(araponga.events.failed[5m])
```

---

## 🚨 Alertas

### Alertas Críticos

#### Alta Taxa de Erros HTTP
- **Nome**: `HighErrorRate`
- **Condição**: `rate(http_requests_received_total{code=~"5.."}[5m]) / rate(http_requests_received_total[5m]) > 0.05`
- **Severidade**: Critical
- **Ação**: Investigar logs e health checks

#### Alta Latência
- **Nome**: `HighLatency`
- **Condição**: `histogram_quantile(0.95, rate(http_requests_duration_seconds_bucket[5m])) > 1`
- **Severidade**: Warning
- **Ação**: Verificar queries lentas e cache

#### Banco de Dados Indisponível
- **Nome**: `DatabaseDown`
- **Condição**: `up{job="araponga"} == 0` ou health check falhando
- **Severidade**: Critical
- **Ação**: Verificar conexão e status do banco

---

### Alertas de Negócio

#### Muitos Eventos Falhando
- **Nome**: `HighEventFailureRate`
- **Condição**: `rate(araponga.events.failed[5m]) > 5`
- **Severidade**: Warning
- **Ação**: Verificar dead letter queue e logs de eventos

#### Cache Hit Rate Baixo
- **Nome**: `LowCacheHitRate`
- **Condição**: `rate(araponga.cache.hits[5m]) / (rate(araponga.cache.hits[5m]) + rate(araponga.cache.misses[5m])) < 0.7`
- **Severidade**: Warning
- **Ação**: Revisar estratégia de cache e TTLs

---

### Alertas de Sistema

#### Muitos Conflitos de Concorrência
- **Nome**: `HighConcurrencyConflicts`
- **Condição**: `rate(araponga.concurrency.conflicts[5m]) > 10`
- **Severidade**: Warning
- **Ação**: Investigar operações concorrentes e otimizar

#### Alta Utilização de Memória
- **Nome**: `HighMemoryUsage`
- **Condição**: `process_resident_memory_bytes / 1024 / 1024 / 1024 > 2` (2GB)
- **Severidade**: Warning
- **Ação**: Verificar memory leaks e otimizar

---

## 📈 Exemplos de Dashboards Grafana

### Dashboard Completo

**Painéis**:
1. Request Rate (gráfico de linha)
2. Error Rate (gráfico de linha)
3. Latência P95 (gráfico de linha)
4. Cache Hit Rate (gráfico de linha)
5. Posts/Eventos Criados (gráfico de linha)
6. Eventos Processados vs Falhados (gráfico de barras)
7. Conflitos de Concorrência (gráfico de linha)
8. Top 10 Endpoints por Latência (tabela)

---

## 🔧 Configuração Grafana

### Data Source Prometheus

```yaml
apiVersion: 1
datasources:
  - name: Prometheus
    type: prometheus
    access: proxy
    url: http://prometheus:9090
    isDefault: true
```

### Importar Dashboard

1. Acesse Grafana → Dashboards → Import
2. Use o JSON do dashboard (criar template futuro)
3. Configure alertas conforme necessário

---

## 🔗 Links Relacionados

- [METRICS.md](./METRICS.md) - Lista completa de métricas
- [RUNBOOK.md](./RUNBOOK.md) - Runbook de operações
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Troubleshooting comum
