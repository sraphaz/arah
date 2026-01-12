namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class OutboxMessageRecord
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTime? ProcessAfterUtc { get; set; }
}
