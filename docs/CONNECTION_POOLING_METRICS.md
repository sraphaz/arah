# Métricas de Connection Pooling - Araponga

**Última Atualização**: 2025-01-23  
**Status**: ✅ Configurado

---

## 📋 Resumo

Este documento descreve a configuração de connection pooling e como monitorar métricas de conexões do PostgreSQL no Araponga.

---

## ⚙️ Configuração Atual

### Connection String

A connection string está configurada em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=araponga;Username=araponga;Password=araponga;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;Command Timeout=30"
  }
}
```

**Parâmetros**:
- `Pooling=true` - Habilita connection pooling
- `Minimum Pool Size=5` - Mantém 5 conexões sempre abertas
- `Maximum Pool Size=100` - Máximo de 100 conexões simultâneas
- `Command Timeout=30` - Timeout de 30 segundos para comandos

---

## 📊 Métricas Disponíveis

### Métricas do .NET Metrics API

As seguintes métricas foram adicionadas ao `ArapongaMetrics`:

```csharp
// Connection Pool Metrics
public static readonly ObservableGauge<int> DatabaseConnectionsActive
public static readonly ObservableGauge<int> DatabaseConnectionsIdle
public static readonly ObservableGauge<int> DatabaseConnectionsTotal
public static readonly Counter<long> DatabaseConnectionsOpened
public static readonly Counter<long> DatabaseConnectionsClosed
public static readonly Counter<long> DatabaseConnectionPoolExhausted
```

### Métricas do PostgreSQL

Para monitorar conexões diretamente no PostgreSQL:

```sql
-- Ver conexões ativas por banco
SELECT 
    datname,
    COUNT(*) FILTER (WHERE state = 'active') as active,
    COUNT(*) FILTER (WHERE state = 'idle') as idle,
    COUNT(*) as total
FROM pg_stat_activity
WHERE datname = current_database()
GROUP BY datname;

-- Ver conexões por aplicação
SELECT 
    application_name,
    state,
    COUNT(*) as count
FROM pg_stat_activity
WHERE datname = current_database()
GROUP BY application_name, state;

-- Ver tamanho do pool
SELECT 
    setting as max_connections
FROM pg_settings
WHERE name = 'max_connections';
```

---

## 🔍 Monitoramento

### Prometheus

As métricas são expostas via Prometheus em `/metrics` (se configurado).

### Health Checks

O Araponga inclui health checks que verificam a disponibilidade do banco de dados:

```csharp
// GET /health/db
// Retorna status da conexão com o banco
```

### Logs

Conexões são logadas automaticamente pelo Npgsql quando há problemas:
- Pool exhaustion
- Timeouts
- Erros de conexão

---

## ⚠️ Alertas Recomendados

Configure alertas para:

1. **Pool Exhaustion**: Quando `DatabaseConnectionPoolExhausted` > 0
2. **Alto Uso**: Quando `DatabaseConnectionsActive` > 80% de `Maximum Pool Size`
3. **Conexões Idle Altas**: Quando `DatabaseConnectionsIdle` > 50 por muito tempo

---

## 🔧 Ajustes de Performance

### Aumentar Pool Size

Se houver muitos erros de pool exhaustion:

```json
{
  "ConnectionStrings": {
    "Postgres": "...;Maximum Pool Size=200;"
  }
}
```

### Reduzir Pool Size

Se houver muitas conexões idle:

```json
{
  "ConnectionStrings": {
    "Postgres": "...;Minimum Pool Size=2;Maximum Pool Size=50;"
  }
}
```

### Connection Lifetime

Para forçar renovação periódica de conexões:

```json
{
  "ConnectionStrings": {
    "Postgres": "...;Connection Lifetime=300;"
  }
}
```

---

## 📚 Referências

- [Npgsql Connection Pooling](https://www.npgsql.org/doc/connection-string-parameters.html#pooling)
- [PostgreSQL Connection Settings](https://www.postgresql.org/docs/current/runtime-config-connection.html)
- [.NET Metrics API](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics)

---

**Nota**: Métricas de connection pooling são coletadas automaticamente quando o sistema está em execução. Para métricas detalhadas, consulte os logs do PostgreSQL ou use ferramentas de monitoramento como Prometheus/Grafana.
