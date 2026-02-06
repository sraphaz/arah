using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Araponga.Bff.Services;
using Microsoft.Extensions.Options;

namespace Araponga.Bff.Middleware;

/// <summary>
/// Encaminha requisições /api/v2/journeys/* para a API principal.
/// Aplica cache em respostas GET 2xx quando configurado.
/// Registra em log exceções e respostas 5xx da API para diagnóstico.
/// </summary>
public sealed class JourneyProxyMiddleware
{
    private const string JourneyPathPrefix = "/api/v2/journeys/";
    private readonly RequestDelegate _next;
    private readonly IOptions<BffOptions> _options;
    private readonly ILogger<JourneyProxyMiddleware> _logger;

    public JourneyProxyMiddleware(RequestDelegate next, IOptions<BffOptions> options, ILogger<JourneyProxyMiddleware> logger)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJourneyApiProxy proxy, IJourneyResponseCache cache)
    {
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith(JourneyPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Value.ApiBaseUrl))
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(new { error = "BFF: ApiBaseUrl not configured." }).ConfigureAwait(false);
            return;
        }

        var pathAndQuery = path[JourneyPathPrefix.Length..].TrimStart('/');
        var method = context.Request.Method;

        if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) && _options.Value.EnableCache)
        {
            var cacheKey = BuildCacheKey(pathAndQuery, context.Request.QueryString.ToString(), context.Request.Headers.Authorization.ToString());
            if (cache.TryGet(cacheKey, out var cached) && cached is not null)
            {
                context.Response.Headers["X-Bff-Cache"] = "HIT";
                await WriteCachedResponseAsync(context, cached).ConfigureAwait(false);
                return;
            }
        }

        context.Request.EnableBuffering();
        var forwardUri = JourneyApiProxy.BuildForwardUri(_options.Value.ApiBaseUrl ?? "", pathAndQuery, context.Request.QueryString.ToString());
        var correlationId = context.Items[CorrelationIdMiddleware.CorrelationIdItemKey]?.ToString() ?? "unknown";
        var opts = _options.Value;

        if (opts.LogForwardToApi)
        {
            _logger.LogInformation(
                "BFF Forward to API | Method={Method} Path={Path} Uri={Uri} CorrelationId={CorrelationId}",
                method, pathAndQuery, forwardUri, correlationId);
        }

        var stopwatch = Stopwatch.StartNew();
        HttpResponseMessage response;
        try
        {
            response = await proxy.ForwardAsync(context.Request, pathAndQuery, context.RequestAborted).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "BFF Proxy error | Method={Method} Path={Path} Uri={Uri} DurationMs={DurationMs} CorrelationId={CorrelationId} Message={Message}",
                method, pathAndQuery, forwardUri, stopwatch.ElapsedMilliseconds, correlationId, ex.Message);
            context.Response.StatusCode = 502;
            var apiBase = _options.Value.ApiBaseUrl ?? "http://localhost:8080";
            var isPrematureClose = ex.Message.Contains("prematurely", StringComparison.OrdinalIgnoreCase) ||
                                  ex.Message.Contains("ResponseEnded", StringComparison.OrdinalIgnoreCase);
            var hint = isPrematureClose
                ? "A API fechou a conexão sem responder. Verifique se a API está rodando e os logs do container/processo (ex.: docker logs para araponga-api)."
                : "Ensure the API is running (e.g. docker ps for araponga-api, or open " + apiBase + "/health).";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "BFF could not reach the API.",
                detail = ex.Message,
                apiBaseUrl = apiBase,
                hint
            }).ConfigureAwait(false);
            return;
        }

        using (response)
        {
            stopwatch.Stop();
            var statusCode = (int)response.StatusCode;
            if (opts.LogForwardToApi)
            {
                _logger.LogInformation(
                    "BFF Forward completed | Method={Method} Path={Path} ApiStatusCode={StatusCode} DurationMs={DurationMs} CorrelationId={CorrelationId}",
                    method, pathAndQuery, statusCode, stopwatch.ElapsedMilliseconds, correlationId);
            }

            if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "API returned error | StatusCode={StatusCode} Method={Method} Path={Path} Uri={Uri} CorrelationId={CorrelationId}",
                    statusCode, method, pathAndQuery, forwardUri, correlationId);
            }

            // Log response body for any 4xx/5xx so the console shows the real API error (e.g. error, detail) for diagnosis.
            var bodyPreviewLength = opts.LogApiErrorBodyPreviewLength;
            if (statusCode >= 400 && bodyPreviewLength > 0 && response.Content is not null)
            {
                var bodyPreview = "";
                try
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync(context.RequestAborted).ConfigureAwait(false);
                    bodyPreview = Encoding.UTF8.GetString(bytes);
                    if (bodyPreview.Length > bodyPreviewLength)
                        bodyPreview = bodyPreview[..bodyPreviewLength] + "...";
                    response.Content = new ByteArrayContent(bytes);
                    _logger.LogWarning(
                        "API error body | StatusCode={StatusCode} Path={Path} CorrelationId={CorrelationId} BodyPreview={BodyPreview}",
                        statusCode, pathAndQuery, correlationId, bodyPreview);
                }
                catch { /* ignore */ }
            }

            if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
                context.Response.Headers["X-Bff-Cache"] = "MISS";

            if (cache.ShouldCache(method, pathAndQuery, (int)response.StatusCode) && response.Content is not null)
        {
            var body = await response.Content.ReadAsByteArrayAsync(context.RequestAborted).ConfigureAwait(false);
            var headers = new Dictionary<string, string[]>();
            foreach (var h in response.Headers)
                headers[h.Key] = h.Value.ToArray();
            if (response.Content.Headers.ContentType?.ToString() is { } ct)
                headers["Content-Type"] = new[] { ct };
            var cachedResponse = new CachedJourneyResponse(
                (int)response.StatusCode,
                headers,
                response.Content.Headers.ContentType?.ToString(),
                body);
            var ttl = TimeSpan.FromSeconds(cache.GetTtlSeconds(pathAndQuery));
            cache.Set(BuildCacheKey(pathAndQuery, context.Request.QueryString.ToString(), context.Request.Headers.Authorization.ToString()), cachedResponse, ttl);

            context.Response.StatusCode = cachedResponse.StatusCode;
            foreach (var (key, values) in cachedResponse.Headers)
            {
                if (key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                    continue;
                context.Response.Headers[key] = values;
            }
            await context.Response.Body.WriteAsync(cachedResponse.Body, context.RequestAborted).ConfigureAwait(false);
            return;
        }

        context.Response.StatusCode = (int)response.StatusCode;
        foreach (var header in response.Headers)
        {
            if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                continue;
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        if (response.Content is not null)
        {
            foreach (var header in response.Content.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();
            await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted).ConfigureAwait(false);
        }
        }
    }

    private static async Task WriteCachedResponseAsync(HttpContext context, CachedJourneyResponse cached)
    {
        context.Response.StatusCode = cached.StatusCode;
        foreach (var (key, values) in cached.Headers)
        {
            if (key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                continue;
            context.Response.Headers[key] = values;
        }
        await context.Response.Body.WriteAsync(cached.Body, context.RequestAborted).ConfigureAwait(false);
    }

    private static string BuildCacheKey(string pathAndQuery, string queryString, string authorization)
    {
        var raw = $"{pathAndQuery}|{queryString}|{authorization}";
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
