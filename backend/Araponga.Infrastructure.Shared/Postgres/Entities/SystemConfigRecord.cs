using Araponga.Domain.Configuration;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class SystemConfigRecord
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public SystemConfigCategory Category { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
