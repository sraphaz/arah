namespace Araponga.Modules.Assets.Domain;

/// <summary>
/// Representa um recurso valioso do território (naturais, culturais, comunitários, infraestruturais, simbólicos).
/// TerritoryAssets sempre pertencem a um Territory e não são vendáveis.
/// TerritoryAssets não representam mídia (foto, vídeo, documento, link) - mídia deve ser tratada como
/// registro/evidência associada a um TerritoryAsset, Event ou Post.
/// O campo Type deve representar o tipo de recurso territorial (ex.: "natural", "cultural", "community", "infrastructure", "knowledge").
/// </summary>
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
    
    /// <summary>
    /// ID do Territory ao qual este asset pertence. TerritoryAssets sempre pertencem a um Territory.
    /// </summary>
    public Guid TerritoryId { get; }
    
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public AssetStatus Status { get; private set; }
    
    /// <summary>
    /// ID do usuário que criou o asset (auditoria). TerritoryAssets não têm "owner" individual.
    /// </summary>
    public Guid CreatedByUserId { get; }
    
    public DateTime CreatedAtUtc { get; }
    
    /// <summary>
    /// ID do usuário que atualizou o asset (auditoria). Não representa ownership ou stewardship.
    /// </summary>
    public Guid UpdatedByUserId { get; private set; }
    
    public DateTime UpdatedAtUtc { get; private set; }
    
    /// <summary>
    /// ID do usuário que arquivou o asset (auditoria). Não representa ownership ou stewardship.
    /// </summary>
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

    public void Approve(Guid approvedByUserId, DateTime approvedAtUtc)
    {
        Status = AssetStatus.Active;
        UpdatedByUserId = approvedByUserId;
        UpdatedAtUtc = approvedAtUtc;
    }

    public void Reject(Guid rejectedByUserId, DateTime rejectedAtUtc, string? reason)
    {
        Status = AssetStatus.Rejected;
        ArchivedByUserId = rejectedByUserId;
        ArchivedAtUtc = rejectedAtUtc;
        ArchiveReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        UpdatedByUserId = rejectedByUserId;
        UpdatedAtUtc = rejectedAtUtc;
    }
}
