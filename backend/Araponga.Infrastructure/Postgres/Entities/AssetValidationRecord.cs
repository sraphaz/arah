namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class AssetValidationRecord
{
    public Guid AssetId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
