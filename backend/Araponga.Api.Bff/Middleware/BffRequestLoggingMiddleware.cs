using System.Diagnostics;
using System.Text;
using Araponga.Bff.Services;
using Microsoft.Extensions.Options;

namespace Araponga.Bff.Middleware;

/// <summary>
/// Logging estruturado de todas as requisições no BFF: entrada (método, path, query, body opcional)
/// e saída (status, duração, correlation id). Integra com App → BFF → API para monitoramento.
/// </summary>
public sealed class BffRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BffRequestLoggingMiddleware> _logger;
    private readonly IOptions<BffOptions> _options;

    public BffRequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<BffRequestLoggingMiddleware> logger,
        IOptions<BffOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var opts = _options.Value;
        if (!opts.LogIncomingRequest && !opts.LogOutgoingResponse)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        context.Request.EnableBuffering();
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var query = context.Request.QueryString.Value ?? "";
        var correlationId = context.Items[CorrelationIdMiddleware.CorrelationIdItemKey]?.ToString() ?? "unknown";
        var contentLength = context.Request.ContentLength ?? 0;

        // Sanitizar para evitar log injection (method, path, query e correlationId podem vir do cliente)
        var sanitizedMethod = SanitizeForLog(method);
        var sanitizedPath = SanitizeForLog(path);
        var sanitizedQuery = query.Length > 200 ? SanitizeForLog(query[..200]) + "..." : SanitizeForLog(query);
        var sanitizedCorrelationId = SanitizeForLog(correlationId);

        if (opts.LogIncomingRequest)
        {
            _logger.LogInformation(
                "BFF Request started | Method={Method} Path={Path} Query={Query} ContentLength={ContentLength} CorrelationId={CorrelationId}",
                sanitizedMethod, sanitizedPath, sanitizedQuery, contentLength, sanitizedCorrelationId);
        }

        if (opts.LogRequestBodyMaxLength > 0 && contentLength > 0 && contentLength < 50_000)
        {
            try
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var bodyPreview = await reader.ReadToEndAsync(context.RequestAborted).ConfigureAwait(false);
                context.Request.Body.Position = 0;
                var max = opts.LogRequestBodyMaxLength;
                var toLog = bodyPreview.Length > max ? bodyPreview[..max] + "..." : bodyPreview;
                if (!string.IsNullOrWhiteSpace(toLog))
                    _logger.LogInformation("BFF Request body | CorrelationId={CorrelationId} Body={Body}", sanitizedCorrelationId, SanitizeForLog(toLog));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "BFF Request body read failed | CorrelationId={CorrelationId}", sanitizedCorrelationId);
                context.Request.Body.Position = 0;
            }
        }

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            if (opts.LogOutgoingResponse)
            {
                var statusCode = context.Response.StatusCode;
                _logger.LogInformation(
                    "BFF Request completed | Method={Method} Path={Path} StatusCode={StatusCode} DurationMs={DurationMs} CorrelationId={CorrelationId}",
                    sanitizedMethod, sanitizedPath, statusCode, stopwatch.ElapsedMilliseconds, sanitizedCorrelationId);
            }
        }
    }

    /// <summary>Remove caracteres de controle e newlines para evitar log injection.</summary>
    private static string SanitizeForLog(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c >= ' ' && c != '\u007f') sb.Append(c);
        }
        return sb.ToString().Trim();
    }
}
