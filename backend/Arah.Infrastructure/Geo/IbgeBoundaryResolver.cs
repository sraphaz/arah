using System.Net.Http.Headers;
using System.Text.Json;
using Arah.Application.Interfaces.Geo;
using Arah.Domain.Territories;
using Microsoft.Extensions.Logging;

namespace Arah.Infrastructure.Geo;

public sealed class IbgeBoundaryResolver : IIbgeBoundaryResolver
{
    private const string LocalidadesBase = "https://servicodados.ibge.gov.br/api/v1/localidades/";
    private const string MalhasBase = "https://servicodados.ibge.gov.br/api/v3/malhas/municipios/";

    private readonly HttpClient _httpClient;
    private readonly ILogger<IbgeBoundaryResolver> _logger;

    public IbgeBoundaryResolver(HttpClient httpClient, ILogger<IbgeBoundaryResolver> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IbgeMunicipalityBoundary?> ResolveByCityAsync(
        string city,
        string stateCode,
        CancellationToken cancellationToken = default)
    {
        var ibgeCode = await ResolveMunicipalityCodeAsync(city, stateCode, cancellationToken);
        if (ibgeCode is null)
        {
            return null;
        }

        return await FetchBoundaryAsync(ibgeCode.Value, city, stateCode, cancellationToken);
    }

    private async Task<int?> ResolveMunicipalityCodeAsync(
        string city,
        string stateCode,
        CancellationToken cancellationToken)
    {
        var states = await GetJsonAsync<JsonElement>($"{LocalidadesBase}estados", cancellationToken);
        if (states.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        int? stateId = null;
        foreach (var state in states.EnumerateArray())
        {
            if (state.TryGetProperty("sigla", out var sigla) &&
                string.Equals(sigla.GetString(), stateCode, StringComparison.OrdinalIgnoreCase))
            {
                stateId = state.GetProperty("id").GetInt32();
                break;
            }
        }

        if (stateId is null)
        {
            return null;
        }

        var municipalities = await GetJsonAsync<JsonElement>(
            $"{LocalidadesBase}estados/{stateId}/municipios",
            cancellationToken);

        if (municipalities.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var municipality in municipalities.EnumerateArray())
        {
            if (!municipality.TryGetProperty("nome", out var nomeProp))
            {
                continue;
            }

            var nome = nomeProp.GetString();
            if (nome is not null && string.Equals(nome, city, StringComparison.OrdinalIgnoreCase))
            {
                return municipality.GetProperty("id").GetInt32();
            }
        }

        return null;
    }

    private async Task<IbgeMunicipalityBoundary?> FetchBoundaryAsync(
        int ibgeCode,
        string city,
        string stateCode,
        CancellationToken cancellationToken)
    {
        var url = $"{MalhasBase}{ibgeCode}?formato=application/vnd.geo+json&qualidade=intermediaria&resolucao=3";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.geo+json"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("IBGE malhas HTTP {StatusCode} para código {IbgeCode}", response.StatusCode, ibgeCode);
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var root = doc.RootElement;
        JsonElement geometry;
        if (root.TryGetProperty("type", out var rootType) && rootType.GetString() == "FeatureCollection")
        {
            geometry = root.GetProperty("features")[0].GetProperty("geometry");
        }
        else if (root.TryGetProperty("geometry", out var directGeometry))
        {
            geometry = directGeometry;
        }
        else
        {
            return null;
        }

        var ring = ExtractLargestRing(geometry);
        if (ring is null || ring.Count < 3)
        {
            return null;
        }

        var points = new List<TerritoryBoundaryPoint>(ring.Count);
        var sumLat = 0.0;
        var sumLng = 0.0;
        foreach (var coord in ring)
        {
            var lng = coord[0];
            var lat = coord[1];
            points.Add(new TerritoryBoundaryPoint(lat, lng));
            sumLat += lat;
            sumLng += lng;
        }

        var municipalityName = city;
        return new IbgeMunicipalityBoundary(
            ibgeCode,
            municipalityName,
            city,
            stateCode,
            Math.Round(sumLat / points.Count, 6),
            Math.Round(sumLng / points.Count, 6),
            points);
    }

    private static List<double[]>? ExtractLargestRing(JsonElement geometry)
    {
        var type = geometry.GetProperty("type").GetString();
        if (type == "Polygon")
        {
            return ParseRing(geometry.GetProperty("coordinates")[0]);
        }

        if (type == "MultiPolygon")
        {
            List<double[]>? largest = null;
            var max = 0;
            foreach (var polygon in geometry.GetProperty("coordinates").EnumerateArray())
            {
                var ring = ParseRing(polygon[0]);
                if (ring.Count > max)
                {
                    max = ring.Count;
                    largest = ring;
                }
            }

            return largest;
        }

        return null;
    }

    private static List<double[]> ParseRing(JsonElement ringElement)
    {
        var ring = new List<double[]>();
        foreach (var point in ringElement.EnumerateArray())
        {
            ring.Add(new[] { point[0].GetDouble(), point[1].GetDouble() });
        }

        return ring;
    }

    private async Task<T> GetJsonAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
        if (result is null)
        {
            throw new InvalidOperationException($"Resposta JSON vazia: {url}");
        }

        return result;
    }
}
