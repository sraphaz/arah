using System.Net.Http.Headers;
using Araponga.Bff.Journeys;
using Microsoft.Extensions.Options;

namespace Araponga.Bff.Services;

/// <summary>
/// Encaminha requisições de jornada para a API principal (Araponga.Api).
/// O BFF é uma aplicação separada que repassa as chamadas para a API.
/// </summary>
public interface IJourneyApiProxy
{
    /// <summary>
    /// Encaminha a requisição HTTP atual para o path de jornada na API e retorna a resposta.
    /// </summary>
    /// <param name="request">Requisição HTTP atual.</param>
    /// <param name="pathAndQuery">Path relativo (ex: feed/territory-feed) + QueryString já incluso no request.</param>
    Task<HttpResponseMessage> ForwardAsync(
        HttpRequest request,
        string pathAndQuery,
        CancellationToken cancellationToken = default);
}

public sealed class JourneyApiProxy : IJourneyApiProxy
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<BffOptions> _options;

    public JourneyApiProxy(HttpClient httpClient, IOptions<BffOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    /// <summary>Constrói a URI de destino na API (para testes e diagnóstico).</summary>
    public static string BuildForwardUri(string baseUrl, string pathAndQuery, string? queryString = null)
    {
        var path = pathAndQuery.TrimStart('/');
        var (apiPathBase, relativePath) = ResolveApiPath(path);
        var qs = queryString ?? "";
        if (qs.Length > 0 && !qs.StartsWith('?'))
            qs = "?" + qs;
        return string.IsNullOrEmpty(relativePath)
            ? $"{baseUrl.TrimEnd('/')}/{apiPathBase}{qs}"
            : $"{baseUrl.TrimEnd('/')}/{apiPathBase}/{relativePath}{qs}";
    }

    public async Task<HttpResponseMessage> ForwardAsync(
        HttpRequest request,
        string pathAndQuery,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.Value.ApiBaseUrl?.TrimEnd('/') ?? "";
        var uri = BuildForwardUri(baseUrl, pathAndQuery, request.QueryString.ToString());
        var requestMessage = new HttpRequestMessage(MapMethod(request.Method), uri);

        // Corpo: buffer até 2MB; acima disso ou chunked (sem Content-Length) encaminha em stream para não regredir uploads.
        const int maxBufferedBodyBytes = 2 * 1024 * 1024;
        byte[]? bodyBytes = null;
        var bodyStreamSet = false; // true quando já definimos StreamContent(request.Body) para chunked/large
        var contentLength = request.ContentLength;
        var methodHasBody = string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase) == false;

        if (contentLength is > 0 and <= maxBufferedBodyBytes && request.Body.CanRead)
        {
            if (request.Body.CanSeek)
                request.Body.Position = 0;
            var buf = new byte[contentLength.Value];
            var offset = 0;
            while (offset < buf.Length)
            {
                var read = await request.Body.ReadAsync(buf.AsMemory(offset, buf.Length - offset), cancellationToken).ConfigureAwait(false);
                if (read == 0) break;
                offset += read;
            }
            bodyBytes = offset > 0 ? (offset == buf.Length ? buf : buf.AsSpan(0, offset).ToArray()) : null;
        }
        else if (request.Body.CanRead && methodHasBody && (contentLength is null or 0 or > maxBufferedBodyBytes))
        {
            // Chunked ou corpo > 2MB: encaminhar stream (evita corpo vazio na API)
            bodyStreamSet = true;
            if (request.Body.CanSeek)
                request.Body.Position = 0;
            requestMessage.Content = new StreamContent(request.Body);
            if (request.ContentType is not null)
                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
            if (contentLength is > 0)
                requestMessage.Content.Headers.ContentLength = contentLength;
        }

        foreach (var header in request.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                continue;
            // Não copiar Content-Length/Content-Type; serão definidos pelo ByteArrayContent/StreamContent
            if (header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                continue;
            if (requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                continue;
            requestMessage.Content ??= new StreamContent(Stream.Null);
            requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        // Só não sobrescrever Content quando já definimos o stream do body (chunked/large). Caso contrário,
        // mesmo que o loop de headers tenha feito Content = StreamContent(Stream.Null), usamos o body bufferizado.
        if (bodyBytes is { Length: > 0 } && !bodyStreamSet)
        {
            requestMessage.Content = new ByteArrayContent(bodyBytes);
            if (request.ContentType is not null)
                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
        }

        return await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Resolve path relativo ao BFF (ex: feed/territory-feed, auth/login) para (path base na API, path relativo).</summary>
    private static (string apiPathBase, string relativePath) ResolveApiPath(string pathAndQuery)
    {
        var path = pathAndQuery.TrimStart('/');
        if (string.IsNullOrEmpty(path))
            return (BffJourneyRegistry.GetApiPathBase(""), "");

        var slash = path.IndexOf('/');
        var journeyName = slash < 0 ? path : path[..slash];
        var relativePath = slash < 0 ? "" : path[(slash + 1)..].TrimEnd('/');
        var apiPathBase = BffJourneyRegistry.GetApiPathBase(journeyName);
        return (apiPathBase, relativePath);
    }

    private static HttpMethod MapMethod(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            _ => new HttpMethod(method)
        };
    }
}

public sealed class BffOptions
{
    public const string SectionName = "Bff";
    public string? ApiBaseUrl { get; set; }

    /// <summary>Habilita cache de respostas GET (jornadas).</summary>
    public bool EnableCache { get; set; } = true;

    /// <summary>TTL padrão do cache em segundos (apenas para GET 2xx).</summary>
    public int CacheTtlSeconds { get; set; } = 60;

    /// <summary>TTL por prefixo de path (ex: "feed" => 30, "onboarding" => 300). Sobrescreve CacheTtlSeconds.</summary>
    public Dictionary<string, int>? CacheTtlByPath { get; set; }

    // Monitoramento / logging
    /// <summary>Logar requisições recebidas no BFF (entrada).</summary>
    public bool LogIncomingRequest { get; set; } = true;

    /// <summary>Logar conclusão da requisição (status, duração, saída).</summary>
    public bool LogOutgoingResponse { get; set; } = true;

    /// <summary>Logar chamadas à API (forward URI, status, duração).</summary>
    public bool LogForwardToApi { get; set; } = true;

    /// <summary>Incluir preview do body em respostas 4xx/5xx da API (máx caracteres; 0 = desligado).</summary>
    public int LogApiErrorBodyPreviewLength { get; set; } = 500;

    /// <summary>Logar corpo da requisição recebida (máx caracteres; 0 = desligado). Habilitar em Development para diagnóstico.</summary>
    public int LogRequestBodyMaxLength { get; set; } = 0;
}
