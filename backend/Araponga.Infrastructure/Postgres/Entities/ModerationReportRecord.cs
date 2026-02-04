using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class ModerationReportRecord
{
    public Guid Id { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid TerritoryId { get; set; }
    public ReportTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
