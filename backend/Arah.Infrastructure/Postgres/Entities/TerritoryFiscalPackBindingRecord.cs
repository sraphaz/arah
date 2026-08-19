namespace Arah.Infrastructure.Postgres.Entities;

public sealed class TerritoryFiscalPackBindingRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string PackId { get; set; } = string.Empty;
    public int Status { get; set; }
    public Guid? ActivatedByUserId { get; set; }
    public DateTimeOffset? ActivatedAtUtc { get; set; }
    public string? MunicipalityIbge { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
