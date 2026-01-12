namespace Araponga.Domain.Assets;

public sealed class TerritoryAsset
{
    public TerritoryAsset(
        Guid id,
        Guid territoryId,
        string type,
        string name,
        string? description,
        AssetStatus status,
        Guid createdByUserId,
        DateTime createdAtUtc,
        Guid updatedByUserId,
        DateTime updatedAtUtc,
        Guid? archivedByUserId,
        DateTime? archivedAtUtc,
        string? archiveReason)
    {
        Id = id;
        TerritoryId = territoryId;
        Type = type;
        Name = name;
        Description = description;
        Status = status;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = updatedAtUtc;
        ArchivedByUserId = archivedByUserId;
        ArchivedAtUtc = archivedAtUtc;
        ArchiveReason = archiveReason;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public AssetStatus Status { get; private set; }
    public Guid CreatedByUserId { get; }
    public DateTime CreatedAtUtc { get; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public Guid? ArchivedByUserId { get; private set; }
    public DateTime? ArchivedAtUtc { get; private set; }
    public string? ArchiveReason { get; private set; }

    public void UpdateDetails(
        string type,
        string name,
        string? description,
        Guid updatedByUserId,
        DateTime updatedAtUtc)
    {
        Type = type;
        Name = name;
        Description = description;
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Archive(
        Guid archivedByUserId,
        DateTime archivedAtUtc,
        string? reason,
        Guid updatedByUserId,
        DateTime updatedAtUtc)
    {
        Status = AssetStatus.Archived;
        ArchivedByUserId = archivedByUserId;
        ArchivedAtUtc = archivedAtUtc;
        ArchiveReason = reason;
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = updatedAtUtc;
    }
}
