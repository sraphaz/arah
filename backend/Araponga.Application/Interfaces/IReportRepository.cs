using Araponga.Domain.Moderation;

namespace Araponga.Application.Interfaces;

public interface IReportRepository
{
    Task<ModerationReport?> GetByIdAsync(Guid reportId, CancellationToken cancellationToken);

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
    
    /// <summary>
    /// Lists reports with pagination.
    /// </summary>
    Task<IReadOnlyList<ModerationReport>> ListPagedAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts reports matching criteria.
    /// </summary>
    Task<int> CountAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);

    Task UpdateStatusAsync(Guid reportId, ReportStatus status, CancellationToken cancellationToken);
}
