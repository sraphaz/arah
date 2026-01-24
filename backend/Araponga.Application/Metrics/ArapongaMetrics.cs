using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace Araponga.Application.Metrics;

/// <summary>
/// Centralized metrics for Araponga application using .NET Metrics API.
/// </summary>
public static class ArapongaMetrics
{
    private static readonly Meter Meter = new("Araponga", "1.0.0");

    // Business Metrics
    public static readonly Counter<long> PostsCreated = Meter.CreateCounter<long>(
        "araponga.posts.created",
        "count",
        "Total number of posts created");

    public static readonly Counter<long> EventsCreated = Meter.CreateCounter<long>(
        "araponga.events.created",
        "count",
        "Total number of events created");

    public static readonly Counter<long> MembershipsCreated = Meter.CreateCounter<long>(
        "araponga.memberships.created",
        "count",
        "Total number of memberships created");

    public static readonly Counter<long> TerritoriesCreated = Meter.CreateCounter<long>(
        "araponga.territories.created",
        "count",
        "Total number of territories created");

    public static readonly Counter<long> ReportsCreated = Meter.CreateCounter<long>(
        "araponga.reports.created",
        "count",
        "Total number of moderation reports created");

    public static readonly Counter<long> JoinRequestsCreated = Meter.CreateCounter<long>(
        "araponga.join_requests.created",
        "count",
        "Total number of join requests created");

    // Cache Metrics
    public static readonly Counter<long> CacheHits = Meter.CreateCounter<long>(
        "araponga.cache.hits",
        "count",
        "Total number of cache hits");

    public static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>(
        "araponga.cache.misses",
        "count",
        "Total number of cache misses");

    // Concurrency Metrics
    public static readonly Counter<long> ConcurrencyConflicts = Meter.CreateCounter<long>(
        "araponga.concurrency.conflicts",
        "count",
        "Total number of concurrency conflicts detected");

    // Event Processing Metrics
    public static readonly Counter<long> EventsProcessed = Meter.CreateCounter<long>(
        "araponga.events.processed",
        "count",
        "Total number of events processed");

    public static readonly Counter<long> EventsFailed = Meter.CreateCounter<long>(
        "araponga.events.failed",
        "count",
        "Total number of events that failed processing");

    public static readonly Histogram<double> EventProcessingDuration = Meter.CreateHistogram<double>(
        "araponga.events.processing.duration",
        "ms",
        "Duration of event processing in milliseconds");

    // Database Metrics
    public static readonly Histogram<double> DatabaseQueryDuration = Meter.CreateHistogram<double>(
        "araponga.database.query.duration",
        "ms",
        "Duration of database queries in milliseconds");

    // Connection Pool Metrics
    // Nota: ObservableGauges requerem funções callback. Para connection pooling,
    // recomenda-se monitorar via PostgreSQL diretamente (pg_stat_activity) ou
    // usar ferramentas de monitoramento externas.
    // Counters abaixo podem ser incrementados quando conexões são abertas/fechadas.
    
    public static readonly Counter<long> DatabaseConnectionsOpened = Meter.CreateCounter<long>(
        "araponga.database.connections.opened",
        "count",
        "Total number of database connections opened");

    public static readonly Counter<long> DatabaseConnectionsClosed = Meter.CreateCounter<long>(
        "araponga.database.connections.closed",
        "count",
        "Total number of database connections closed");

    public static readonly Counter<long> DatabaseConnectionPoolExhausted = Meter.CreateCounter<long>(
        "araponga.database.connection_pool.exhausted",
        "count",
        "Number of times the connection pool was exhausted");
    
    // ObservableGauges para métricas em tempo real
    // Requerem funções callback que consultam o banco de dados
    // Nota: Estas métricas consultam pg_stat_activity quando necessário
    // Para melhor performance, considere cachear os valores por alguns segundos
    public static ObservableGauge<long>? DatabaseConnectionsActive { get; private set; }
    public static ObservableGauge<long>? DatabaseConnectionsIdle { get; private set; }
    
    /// <summary>
    /// Configura ObservableGauges para métricas de connection pool em tempo real.
    /// Requer uma função que retorna o número de conexões ativas consultando pg_stat_activity.
    /// </summary>
    public static void ConfigureConnectionPoolMetrics(Func<long> getActiveConnections, Func<long> getIdleConnections)
    {
        DatabaseConnectionsActive = Meter.CreateObservableGauge<long>(
            "araponga.database.connections.active",
            unit: "count",
            description: "Number of active database connections",
            observeValue: getActiveConnections);
        
        DatabaseConnectionsIdle = Meter.CreateObservableGauge<long>(
            "araponga.database.connections.idle",
            unit: "count",
            description: "Number of idle database connections",
            observeValue: getIdleConnections);
    }
    
    // Nota: Para métricas de conexões ativas/idle em tempo real, consulte:
    // - PostgreSQL: SELECT COUNT(*) FROM pg_stat_activity WHERE datname = current_database() AND state = 'active';
    // - Documentação: docs/CONNECTION_POOLING_METRICS.md
}
