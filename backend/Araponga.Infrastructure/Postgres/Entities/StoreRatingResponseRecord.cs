namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class StoreRatingResponseRecord
{
    public Guid Id { get; set; }
    public Guid RatingId { get; set; }
    public Guid StoreId { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
