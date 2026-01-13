using Araponga.Domain.Geo;
using Araponga.Domain.Social;

namespace Araponga.Domain.Events;

public sealed class TerritoryEvent
{
    public TerritoryEvent(
        Guid id,
        Guid territoryId,
        string title,
        string? description,
        DateTime startsAtUtc,
        DateTime? endsAtUtc,
        double latitude,
        double longitude,
        string? locationLabel,
        Guid createdByUserId,
        MembershipRole createdByMembership,
        EventStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Created by user ID is required.", nameof(createdByUserId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            throw new ArgumentException("Invalid latitude/longitude.");
        }

        if (endsAtUtc is not null && endsAtUtc.Value < startsAtUtc)
        {
            throw new ArgumentException("EndsAtUtc must be after StartsAtUtc.", nameof(endsAtUtc));
        }

        Id = id;
        TerritoryId = territoryId;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Latitude = latitude;
        Longitude = longitude;
        LocationLabel = string.IsNullOrWhiteSpace(locationLabel) ? null : locationLabel.Trim();
        CreatedByUserId = createdByUserId;
        CreatedByMembership = createdByMembership;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime? EndsAtUtc { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? LocationLabel { get; private set; }
    public Guid CreatedByUserId { get; }
    public MembershipRole CreatedByMembership { get; }
    public EventStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(
        string title,
        string? description,
        DateTime startsAtUtc,
        DateTime? endsAtUtc,
        double latitude,
        double longitude,
        string? locationLabel,
        DateTime updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            throw new ArgumentException("Invalid latitude/longitude.");
        }

        if (endsAtUtc is not null && endsAtUtc.Value < startsAtUtc)
        {
            throw new ArgumentException("EndsAtUtc must be after StartsAtUtc.", nameof(endsAtUtc));
        }

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Latitude = latitude;
        Longitude = longitude;
        LocationLabel = string.IsNullOrWhiteSpace(locationLabel) ? null : locationLabel.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Cancel(DateTime updatedAtUtc)
    {
        Status = EventStatus.Canceled;
        UpdatedAtUtc = updatedAtUtc;
    }
}
