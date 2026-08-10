namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Corpo d'água / ativo natural curável do território (FASE24.0). Nunca vendável; Territory permanece sem campos hídricos.
/// Slice WA-N1: tipos de ponto apenas (SPRING, WATERFALL, POTABLE_WATER) com <see cref="WaterPointDetails"/>.
/// </summary>
public sealed class NaturalAsset
{
    public NaturalAsset(
        Guid id,
        Guid territoryId,
        string type,
        string name,
        string? description,
        NaturalAssetStatus status,
        WaterPointDetails waterPoint,
        Guid createdByUserId,
        DateTime createdAtUtc,
        Guid updatedByUserId,
        DateTime updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Created-by user ID is required.", nameof(createdByUserId));
        }

        if (updatedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Updated-by user ID is required.", nameof(updatedByUserId));
        }

        if (!NaturalAssetType.TryValidatePointType(type, out var normalizedType, out var typeError))
        {
            throw new ArgumentException(typeError ?? "Invalid type.", nameof(type));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(waterPoint);

        var trimmedName = name.Trim();
        if (trimmedName.Length > 200)
        {
            throw new ArgumentException("Name must be at most 200 characters.", nameof(name));
        }

        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription is { Length: > 1000 })
        {
            throw new ArgumentException("Description must be at most 1000 characters.", nameof(description));
        }

        Id = id;
        TerritoryId = territoryId;
        Type = normalizedType!;
        Name = trimmedName;
        Description = trimmedDescription;
        Status = status;
        WaterPoint = waterPoint;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string Type { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public NaturalAssetStatus Status { get; private set; }
    public WaterPointDetails WaterPoint { get; private set; }
    public Guid CreatedByUserId { get; }
    public DateTime CreatedAtUtc { get; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>NaturalAsset nunca é item de marketplace (AC-WA-6).</summary>
    public static bool IsMarketplaceEligible => false;

    public static NaturalAsset CreatePending(
        Guid territoryId,
        string type,
        string name,
        string? description,
        WaterPointDetails waterPoint,
        Guid createdByUserId,
        DateTime createdAtUtc)
    {
        return new NaturalAsset(
            Guid.NewGuid(),
            territoryId,
            type,
            name,
            description,
            NaturalAssetStatus.Pending,
            waterPoint,
            createdByUserId,
            createdAtUtc,
            createdByUserId,
            createdAtUtc);
    }

    public void Publish(Guid publishedByUserId, DateTime publishedAtUtc)
    {
        if (Status != NaturalAssetStatus.Pending && Status != NaturalAssetStatus.Review)
        {
            throw new InvalidOperationException("Only Pending or Review natural assets can be published.");
        }

        if (publishedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Publisher user ID is required.", nameof(publishedByUserId));
        }

        Status = NaturalAssetStatus.Published;
        UpdatedByUserId = publishedByUserId;
        UpdatedAtUtc = publishedAtUtc;
    }

    /// <summary>
    /// Tentativa de exposição no marketplace — sempre rejeitada.
    /// </summary>
    public bool TryExposeAsMarketplaceItem(out string error)
    {
        error = "Natural assets are never sellable.";
        return false;
    }
}
