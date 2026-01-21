namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class StoreRatingRecord
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
