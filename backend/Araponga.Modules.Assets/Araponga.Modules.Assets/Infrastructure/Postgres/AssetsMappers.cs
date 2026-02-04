using Araponga.Modules.Assets.Domain;
using Araponga.Modules.Assets.Infrastructure.Postgres.Entities;

namespace Araponga.Modules.Assets.Infrastructure.Postgres;

public static class AssetsMappers
{
    public static TerritoryAssetRecord ToRecord(this TerritoryAsset asset)
    {
        return new TerritoryAssetRecord
        {
            Id = asset.Id,
            TerritoryId = asset.TerritoryId,
            Type = asset.Type,
            Name = asset.Name,
            Description = asset.Description,
            Status = asset.Status,
            CreatedByUserId = asset.CreatedByUserId,
            CreatedAtUtc = asset.CreatedAtUtc,
            UpdatedByUserId = asset.UpdatedByUserId,
            UpdatedAtUtc = asset.UpdatedAtUtc,
            ArchivedByUserId = asset.ArchivedByUserId,
            ArchivedAtUtc = asset.ArchivedAtUtc,
            ArchiveReason = asset.ArchiveReason
        };
    }

    public static TerritoryAsset ToDomain(this TerritoryAssetRecord record)
    {
        return new TerritoryAsset(
            record.Id,
            record.TerritoryId,
            record.Type,
            record.Name,
            record.Description,
            record.Status,
            record.CreatedByUserId,
            record.CreatedAtUtc,
            record.UpdatedByUserId,
            record.UpdatedAtUtc,
            record.ArchivedByUserId,
            record.ArchivedAtUtc,
            record.ArchiveReason);
    }

    public static AssetGeoAnchorRecord ToRecord(this AssetGeoAnchor anchor)
    {
        return new AssetGeoAnchorRecord
        {
            Id = anchor.Id,
            AssetId = anchor.AssetId,
            Latitude = anchor.Latitude,
            Longitude = anchor.Longitude,
            CreatedAtUtc = anchor.CreatedAtUtc
        };
    }

    public static AssetGeoAnchor ToDomain(this AssetGeoAnchorRecord record)
    {
        return new AssetGeoAnchor(
            record.Id,
            record.AssetId,
            record.Latitude,
            record.Longitude,
            record.CreatedAtUtc);
    }
}
