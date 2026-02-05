using Araponga.Application.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Postgres;

/// <summary>
/// Serviço para coletar métricas de connection pool em tempo real.
/// Usa IServiceScopeFactory para resolver ArapongaDbContext por chamada (evita Singleton dependendo de Scoped).
/// </summary>
public sealed class ConnectionPoolMetricsService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConnectionPoolMetricsService> _logger;

    public ConnectionPoolMetricsService(
        IServiceScopeFactory scopeFactory,
        ILogger<ConnectionPoolMetricsService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o número de conexões ativas consultando pg_stat_activity.
    /// </summary>
    public long GetActiveConnections()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArapongaDbContext>();
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM pg_stat_activity WHERE datname = current_database() AND state = 'active'";
            var result = command.ExecuteScalar();
            
            if (connection.State == System.Data.ConnectionState.Open && connection.State != System.Data.ConnectionState.Executing)
            {
                connection.Close();
            }
            
            return result is long count ? count : (result is int intCount ? intCount : 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get active connections count");
            return 0;
        }
    }

    /// <summary>
    /// Obtém o número de conexões idle consultando pg_stat_activity.
    /// </summary>
    public long GetIdleConnections()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArapongaDbContext>();
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM pg_stat_activity WHERE datname = current_database() AND state = 'idle'";
            var result = command.ExecuteScalar();
            
            if (connection.State == System.Data.ConnectionState.Open && connection.State != System.Data.ConnectionState.Executing)
            {
                connection.Close();
            }
            
            return result is long count ? count : (result is int intCount ? intCount : 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get idle connections count");
            return 0;
        }
    }

    /// <summary>
    /// Configura ObservableGauges para métricas de connection pool.
    /// Deve ser chamado durante a inicialização da aplicação.
    /// </summary>
    public static void ConfigureMetrics(ConnectionPoolMetricsService service)
    {
        ArapongaMetrics.ConfigureConnectionPoolMetrics(
            () => service.GetActiveConnections(),
            () => service.GetIdleConnections());
    }
}
