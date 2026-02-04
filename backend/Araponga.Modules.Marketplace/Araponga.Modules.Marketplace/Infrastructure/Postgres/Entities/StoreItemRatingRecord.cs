namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class StoreItemRatingRecord
{
    public Guid Id { get; set; }
    public Guid StoreItemId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
