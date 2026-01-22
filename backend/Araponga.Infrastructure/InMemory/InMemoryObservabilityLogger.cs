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
        // Sanitize user-controlled values to prevent log injection
        var sanitizedOperation = SanitizeForLogging(operation);
        var sanitizedReason = SanitizeForLogging(reason ?? "Unknown");
        
        _logger.LogWarning(
            "Geolocation error in {Operation}. Reason: {Reason}. UserId: {UserId}, TerritoryId: {TerritoryId}",
            sanitizedOperation,
            sanitizedReason,
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
        // Sanitize user-controlled values to prevent log injection
        var sanitizedOperation = SanitizeForLogging(operation);
        var sanitizedReason = SanitizeForLogging(reason);
        
        _logger.LogError(
            "Moderation failure in {Operation}. Reason: {Reason}. TerritoryId: {TerritoryId}",
            sanitizedOperation,
            sanitizedReason,
            territoryId);
    }

    public void LogRequest(string method, string path, int statusCode, long durationMs)
    {
        // Sanitize user-controlled values to prevent log injection
        var sanitizedMethod = SanitizeForLogging(method);
        var sanitizedPath = SanitizeForLogging(path);
        
        var level = statusCode >= 500 ? LogLevel.Error
            : statusCode >= 400 ? LogLevel.Warning
            : LogLevel.Information;

        _logger.Log(
            level,
            "HTTP {Method} {Path} returned {StatusCode} in {DurationMs}ms",
            sanitizedMethod,
            sanitizedPath,
            statusCode,
            durationMs);
    }

    /// <summary>
    /// Sanitizes user-controlled input to prevent log injection attacks.
    /// Removes control characters (newlines, carriage returns, etc.) that could be used to forge log entries.
    /// </summary>
    private static string SanitizeForLogging(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Remove control characters that could be used for log injection
        return input
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace("\t", " ", StringComparison.Ordinal)
            .Trim();
    }
}
