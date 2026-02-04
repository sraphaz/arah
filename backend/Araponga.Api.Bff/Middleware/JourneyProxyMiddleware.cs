using System.Security.Cryptography;
using System.Text;
using Araponga.Bff.Services;
using Microsoft.Extensions.Options;

namespace Araponga.Bff.Middleware;

/// <summary>
/// Encaminha requisições /api/v2/journeys/* para a API principal.
/// Aplica cache em respostas GET 2xx quando configurado.
/// </summary>
public sealed class JourneyProxyMiddleware
{
    private const string JourneyPathPrefix = "/api/v2/journeys/";
    private readonly RequestDelegate _next;
    private readonly IOptions<BffOptions> _options;

    public JourneyProxyMiddleware(RequestDelegate next, IOptions<BffOptions> options)
    {
        _next = next;
        _options = options;
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
        using var response = await proxy.ForwardAsync(context.Request, pathAndQuery, context.RequestAborted).ConfigureAwait(false);

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
