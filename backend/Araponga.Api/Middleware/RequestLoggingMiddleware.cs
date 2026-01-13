using Araponga.Application.Interfaces;
using System.Diagnostics;

namespace Araponga.Api.Middleware;

/// <summary>
/// Middleware para logging de requisições HTTP conforme especificação de observabilidade mínima.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IObservabilityLogger? _observabilityLogger;

    public RequestLoggingMiddleware(RequestDelegate next, IObservabilityLogger? observabilityLogger = null)
    {
        _next = next;
        _observabilityLogger = observabilityLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            _observabilityLogger?.LogRequest(method, path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
