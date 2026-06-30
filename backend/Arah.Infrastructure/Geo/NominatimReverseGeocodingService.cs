using System.Text.Json;
using Arah.Application.Interfaces.Geo;
using Microsoft.Extensions.Logging;

namespace Arah.Infrastructure.Geo;

public sealed class NominatimReverseGeocodingService : IReverseGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NominatimReverseGeocodingService> _logger;

    public NominatimReverseGeocodingService(HttpClient httpClient, ILogger<NominatimReverseGeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ReverseGeocodeResult?> ReverseAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var url =
            $"reverse?lat={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&lon={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            "&format=json&addressdetails=1&accept-language=pt-BR";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Nominatim reverse HTTP {StatusCode}", response.StatusCode);
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (!doc.RootElement.TryGetProperty("address", out var address))
        {
            return null;
        }

        var city = FirstNonEmpty(
            GetString(address, "city"),
            GetString(address, "town"),
            GetString(address, "municipality"),
            GetString(address, "village"),
            GetString(address, "county"));

        var stateCode = ParseStateCode(address);
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(stateCode))
        {
            return null;
        }

        return new ReverseGeocodeResult(city.Trim(), stateCode);
    }

    private static string? ParseStateCode(JsonElement address)
    {
        if (address.TryGetProperty("ISO3166-2", out var iso) &&
            iso.GetString() is { } isoValue &&
            isoValue.StartsWith("BR-", StringComparison.OrdinalIgnoreCase) &&
            isoValue.Length == 5)
        {
            return isoValue[3..].ToUpperInvariant();
        }

        return null;
    }

    private static string? GetString(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) ? value.GetString() : null;

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
