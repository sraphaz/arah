using Araponga.Application.Interfaces;
using System.Diagnostics;

namespace Araponga.Api.Middleware;

/// <summary>
/// Middleware para logging de requisições HTTP conforme especificação de observabilidade mínima.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly IObservabilityLogger? _observabilityLogger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IObservabilityLogger? observabilityLogger = null)
    {
        _next = next;
        _logger = logger;
        _observabilityLogger = observabilityLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";

        // Sanitize user-controlled values before logging to avoid log forging via control characters.
        var sanitizedMethod = method.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
        var sanitizedPath = path.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            // Use sanitized values to prevent log injection
            _logger.LogInformation(
                "Request: {Method} {Path} {StatusCode} {DurationMs}ms CorrelationId: {CorrelationId}",
                sanitizedMethod, sanitizedPath, statusCode, stopwatch.ElapsedMilliseconds, correlationId);

            _observabilityLogger?.LogRequest(sanitizedMethod, sanitizedPath, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
