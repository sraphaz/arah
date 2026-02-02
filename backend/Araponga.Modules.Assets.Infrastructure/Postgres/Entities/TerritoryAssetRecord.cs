using Araponga.Domain.Assets;

namespace Araponga.Modules.Assets.Infrastructure.Postgres.Entities;

public sealed class TerritoryAssetRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssetStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid UpdatedByUserId { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public Guid? ArchivedByUserId { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
    public string? ArchiveReason { get; set; }
}
