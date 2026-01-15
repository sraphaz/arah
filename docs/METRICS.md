# Métricas do Araponga

Este documento descreve todas as métricas coletadas pelo sistema Araponga.

## 📊 Visão Geral

O Araponga utiliza **Prometheus** para coleta de métricas e **OpenTelemetry** para instrumentação. As métricas são expostas no endpoint `/metrics` e podem ser coletadas por Prometheus para visualização em dashboards (Grafana).

---

## 🔧 Configuração

### Prometheus

As métricas são expostas automaticamente no endpoint `/metrics` na porta configurada (padrão: 9090).

**Configuração** (`appsettings.json`):
```json
{
  "Metrics": {
    "Prometheus": {
      "Port": 9090
    }
  }
}
```

### OpenTelemetry

OpenTelemetry está configurado para coletar:
- **Tracing**: HTTP requests, database queries, eventos
- **Metrics**: HTTP automáticas, métricas de negócio, métricas de sistema

---

## 📈 Métricas de Negócio

### Posts Criados
- **Nome**: `araponga.posts.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de posts criados
- **Tags**: `territory_id`

**Exemplo**:
```
araponga.posts.created{territory_id="123e4567-e89b-12d3-a456-426614174000"} 42
```

### Eventos Criados
- **Nome**: `araponga.events.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de eventos criados
- **Tags**: `territory_id`

### Membros Criados
- **Nome**: `araponga.memberships.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de memberships criados
- **Tags**: `territory_id`

### Territórios Criados
- **Nome**: `araponga.territories.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de territórios criados

### Relatórios Criados
- **Nome**: `araponga.reports.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de relatórios de moderação criados
- **Tags**: `territory_id`

### Solicitações de Entrada Criadas
- **Nome**: `araponga.join_requests.created`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de solicitações de entrada criadas
- **Tags**: `territory_id`

---

## 💾 Métricas de Cache

### Cache Hits
- **Nome**: `araponga.cache.hits`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de cache hits

### Cache Misses
- **Nome**: `araponga.cache.misses`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de cache misses

**Cálculo de Hit Rate**:
```
hit_rate = araponga.cache.hits / (araponga.cache.hits + araponga.cache.misses)
```

---

## ⚡ Métricas de Concorrência

### Conflitos de Concorrência
- **Nome**: `araponga.concurrency.conflicts`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de conflitos de concorrência detectados (RowVersion mismatch)

---

## 📨 Métricas de Processamento de Eventos

### Eventos Processados
- **Nome**: `araponga.events.processed`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de eventos processados com sucesso

### Eventos Falhados
- **Nome**: `araponga.events.failed`
- **Tipo**: Counter
- **Unidade**: count
- **Descrição**: Total de eventos que falharam após todas as tentativas (moved to dead letter queue)

### Duração de Processamento de Eventos
- **Nome**: `araponga.events.processing.duration`
- **Tipo**: Histogram
- **Unidade**: ms
- **Descrição**: Duração do processamento de eventos em milissegundos

---

## 🗄️ Métricas de Banco de Dados

### Duração de Queries
- **Nome**: `araponga.database.query.duration`
- **Tipo**: Histogram
- **Unidade**: ms
- **Descrição**: Duração de queries do banco de dados em milissegundos

**Coletado automaticamente via OpenTelemetry EntityFrameworkCore Instrumentation**.

---

## 🌐 Métricas HTTP (Automáticas)

As seguintes métricas são coletadas automaticamente via `prometheus-net.AspNetCore`:

- `http_requests_received_total`: Total de requisições HTTP recebidas
- `http_requests_duration_seconds`: Duração de requisições HTTP
- `http_requests_active`: Requisições HTTP ativas
- `http_request_size_bytes`: Tamanho das requisições HTTP
- `http_response_size_bytes`: Tamanho das respostas HTTP

---

## 📊 Como Usar

### Visualizar Métricas

1. **Endpoint Prometheus**: `http://localhost:9090/metrics`
2. **Grafana**: Configure Prometheus como data source e crie dashboards
3. **Application Insights**: Se configurado, métricas são enviadas automaticamente

### Alertas Recomendados

1. **Alta Taxa de Erros**: `rate(http_requests_received_total{code=~"5.."}[5m]) > 0.05`
2. **Alta Latência**: `histogram_quantile(0.95, http_requests_duration_seconds) > 1`
3. **Cache Hit Rate Baixo**: `rate(araponga.cache.hits[5m]) / (rate(araponga.cache.hits[5m]) + rate(araponga.cache.misses[5m])) < 0.7`
4. **Muitos Conflitos de Concorrência**: `rate(araponga.concurrency.conflicts[5m]) > 10`
5. **Eventos Falhando**: `rate(araponga.events.failed[5m]) > 5`

---

## 🔗 Links Relacionados

- [MONITORING.md](./MONITORING.md) - Dashboards e monitoramento
- [RUNBOOK.md](./RUNBOOK.md) - Runbook de operações
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Troubleshooting comum
