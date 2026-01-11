namespace Araponga.Domain.Geo;

public static class GeoCoordinate
{
    public static bool IsValid(double latitude, double longitude)
    {
        return latitude is >= -90 and <= 90
               && longitude is >= -180 and <= 180;
    }
}
