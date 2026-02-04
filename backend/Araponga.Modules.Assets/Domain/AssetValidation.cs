namespace Araponga.Modules.Assets.Domain;

public sealed class AssetValidation
{
    public AssetValidation(Guid assetId, Guid userId, DateTime createdAtUtc)
    {
        AssetId = assetId;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid AssetId { get; }
    public Guid UserId { get; }
    public DateTime CreatedAtUtc { get; }
}
