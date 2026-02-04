namespace Araponga.Modules.Feed.Infrastructure.Postgres.Entities;

public sealed class PostAssetRecord
{
    public Guid PostId { get; set; }
    public Guid AssetId { get; set; }
}
