namespace Araponga.Infrastructure.Postgres.Entities;

/// <summary>
/// Registro Postgres para configuração de storage de mídias (Local, S3, AzureBlob).
/// Settings armazenado em jsonb.
/// </summary>
public sealed class MediaStorageConfigRecord
{
    public Guid Id { get; set; }
    public int Provider { get; set; }
    public string SettingsJson { get; set; } = "{}";
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
