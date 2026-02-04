namespace Araponga.Application.Interfaces;

/// <summary>
/// Logger de observabilidade para métricas e logs mínimos conforme especificação MVP.
/// </summary>
public interface IObservabilityLogger
{
    /// <summary>
    /// Loga erro de geolocalização com contexto mínimo.
    /// </summary>
    void LogGeolocationError(string operation, string? reason, Guid? userId, Guid? territoryId);

    /// <summary>
    /// Loga métrica de report criado.
    /// </summary>
    void LogReportCreated(string targetType, Guid territoryId);

    /// <summary>
    /// Loga métrica de falha em moderação.
    /// </summary>
    void LogModerationFailure(string operation, string reason, Guid? territoryId);

    /// <summary>
    /// Loga métrica de requisição HTTP.
    /// </summary>
    void LogRequest(string method, string path, int statusCode, long durationMs);
}
