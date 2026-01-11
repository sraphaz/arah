namespace Araponga.Domain.Moderation;

public sealed class ModerationReport
{
    public ModerationReport(
        Guid id,
        Guid reporterUserId,
        ReportTargetType targetType,
        Guid targetId,
        string reason,
        string? details,
        DateTime createdAtUtc)
    {
        if (reporterUserId == Guid.Empty)
        {
            throw new ArgumentException("Reporter user ID is required.", nameof(reporterUserId));
        }

        if (targetId == Guid.Empty)
        {
            throw new ArgumentException("Target ID is required.", nameof(targetId));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required.", nameof(reason));
        }

        Id = id;
        ReporterUserId = reporterUserId;
        TargetType = targetType;
        TargetId = targetId;
        Reason = reason.Trim();
        Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid ReporterUserId { get; }
    public ReportTargetType TargetType { get; }
    public Guid TargetId { get; }
    public string Reason { get; }
    public string? Details { get; }
    public DateTime CreatedAtUtc { get; }
}
