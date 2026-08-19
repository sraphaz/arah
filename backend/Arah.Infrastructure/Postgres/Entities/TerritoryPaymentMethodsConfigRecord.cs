namespace Arah.Infrastructure.Postgres.Entities;

public sealed class TerritoryPaymentMethodsConfigRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string MethodsCsv { get; set; } = string.Empty;
    public string? PspProvider { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
