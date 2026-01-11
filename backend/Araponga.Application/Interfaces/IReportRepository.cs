using Araponga.Domain.Moderation;

namespace Araponga.Application.Interfaces;

public interface IReportRepository
{
    Task<bool> HasRecentReportAsync(
        Guid reporterUserId,
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken);

    Task AddAsync(ModerationReport report, CancellationToken cancellationToken);

    Task<int> CountDistinctReportersAsync(
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ModerationReport>> ListAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);
}
