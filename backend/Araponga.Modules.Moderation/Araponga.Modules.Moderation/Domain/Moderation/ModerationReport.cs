namespace Araponga.Modules.Moderation.Domain.Moderation;

public sealed class ModerationReport
{
    public ModerationReport(Guid id, Guid reporterUserId, Guid territoryId, ReportTargetType targetType, Guid targetId, string reason, string? details, ReportStatus status, DateTime createdAtUtc)
    {
        if (reporterUserId == Guid.Empty) throw new ArgumentException("Reporter user ID is required.", nameof(reporterUserId));
        if (territoryId == Guid.Empty) throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        if (targetId == Guid.Empty) throw new ArgumentException("Target ID is required.", nameof(targetId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required.", nameof(reason));
        Id = id;
        ReporterUserId = reporterUserId;
        TerritoryId = territoryId;
        TargetType = targetType;
        TargetId = targetId;
        Reason = reason.Trim();
        Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim();
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid ReporterUserId { get; }
    public Guid TerritoryId { get; }
    public ReportTargetType TargetType { get; }
    public Guid TargetId { get; }
    public string Reason { get; }
    public string? Details { get; }
    public ReportStatus Status { get; }
    public DateTime CreatedAtUtc { get; }
}
