namespace Araponga.Api;

/// <summary>
/// HTTP header names used by the API.
/// </summary>
public static class ApiHeaders
{
    /// <summary>
    /// Header name for session identification.
    /// </summary>
    public const string SessionId = "X-Session-Id";

    /// <summary>
    /// Header name for authorization.
    /// </summary>
    public const string Authorization = "Authorization";

    /// <summary>
    /// Header name for geo latitude.
    /// </summary>
    public const string GeoLatitude = "X-Geo-Latitude";

    /// <summary>
    /// Header name for geo longitude.
    /// </summary>
    public const string GeoLongitude = "X-Geo-Longitude";
}
