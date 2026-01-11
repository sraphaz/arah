using System.Globalization;
using Araponga.Domain.Geo;

namespace Araponga.Api;

public static class GeoHeaderReader
{
    public static bool TryGetCoordinates(
        IHeaderDictionary headers,
        out double latitude,
        out double longitude)
    {
        latitude = 0;
        longitude = 0;

        if (!headers.TryGetValue(ApiHeaders.GeoLatitude, out var latHeader) ||
            !headers.TryGetValue(ApiHeaders.GeoLongitude, out var lngHeader))
        {
            return false;
        }

        if (!double.TryParse(latHeader, NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) ||
            !double.TryParse(lngHeader, NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
        {
            return false;
        }

        return GeoCoordinate.IsValid(latitude, longitude);
    }
}
