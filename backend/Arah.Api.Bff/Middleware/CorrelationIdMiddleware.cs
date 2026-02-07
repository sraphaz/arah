using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arah.Bff.Middleware;

/// <summary>
/// Garante um Correlation ID em toda a requisição para rastreio App → BFF → API.
/// Lê X-Correlation-ID do cliente ou gera um novo; repassa na resposta e para a API.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public const string CorrelationIdHeader = "X-Correlation-ID";
    public const string CorrelationIdItemKey = "CorrelationId";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N")[..16];

        context.Request.Headers[CorrelationIdHeader] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items[CorrelationIdItemKey] = correlationId;

        // Sanitize for logging only (correlationId can come from client header — log injection)
        var correlationIdForLog = SanitizeForLog(correlationId);
        using (context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>()
            .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationIdForLog }))
        {
            await _next(context);
        }
    }

    /// <summary>Remove control chars and newlines to prevent log injection from user-provided X-Correlation-ID.</summary>
    private static string SanitizeForLog(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        var sb = new System.Text.StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c >= ' ' && c != '\u007f') sb.Append(c);
        }
        return sb.ToString().Trim();
    }
}
