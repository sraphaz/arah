using Araponga.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.InMemory;

/// <summary>
/// Implementação InMemory do logger de observabilidade usando ILogger padrão do .NET.
/// </summary>
public sealed class InMemoryObservabilityLogger : IObservabilityLogger
{
    private readonly ILogger<InMemoryObservabilityLogger> _logger;

    public InMemoryObservabilityLogger(ILogger<InMemoryObservabilityLogger> logger)
    {
        _logger = logger;
    }

    public void LogGeolocationError(string operation, string? reason, Guid? userId, Guid? territoryId)
    {
        _logger.LogWarning(
            "Geolocation error in {Operation}. Reason: {Reason}. UserId: {UserId}, TerritoryId: {TerritoryId}",
            operation,
            reason ?? "Unknown",
            userId,
            territoryId);
    }

    public void LogReportCreated(string targetType, Guid territoryId)
    {
        _logger.LogInformation(
            "Report created. TargetType: {TargetType}, TerritoryId: {TerritoryId}",
            targetType,
            territoryId);
    }

    public void LogModerationFailure(string operation, string reason, Guid? territoryId)
    {
        _logger.LogError(
            "Moderation failure in {Operation}. Reason: {Reason}. TerritoryId: {TerritoryId}",
            operation,
            reason,
            territoryId);
    }

    public void LogRequest(string method, string path, int statusCode, long durationMs)
    {
        var level = statusCode >= 500 ? LogLevel.Error
            : statusCode >= 400 ? LogLevel.Warning
            : LogLevel.Information;

        _logger.Log(
            level,
            "HTTP {Method} {Path} returned {StatusCode} in {DurationMs}ms",
            method,
            path,
            statusCode,
            durationMs);
    }
}
