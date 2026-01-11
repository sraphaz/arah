namespace Araponga.Domain.Map;

public sealed class PostGeoAnchor
{
    public PostGeoAnchor(
        Guid id,
        Guid postId,
        double latitude,
        double longitude,
        string type,
        DateTime createdAtUtc)
    {
        if (postId == Guid.Empty)
        {
            throw new ArgumentException("Post ID is required.", nameof(postId));
        }

        if (!Geo.GeoCoordinate.IsValid(latitude, longitude))
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude/longitude is invalid.");
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Anchor type is required.", nameof(type));
        }

        Id = id;
        PostId = postId;
        Latitude = latitude;
        Longitude = longitude;
        Type = type.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid PostId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public string Type { get; }
    public DateTime CreatedAtUtc { get; }
}
