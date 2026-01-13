namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class CartRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
