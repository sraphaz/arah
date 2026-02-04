using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Modules.Moderation.Application.Interfaces;

public interface IReportRepository
{
    Task<ModerationReport?> GetByIdAsync(Guid reportId, CancellationToken cancellationToken);
    Task<bool> HasRecentReportAsync(Guid reporterUserId, ReportTargetType targetType, Guid targetId, DateTime sinceUtc, CancellationToken cancellationToken);
    Task AddAsync(ModerationReport report, CancellationToken cancellationToken);
    Task<int> CountDistinctReportersAsync(ReportTargetType targetType, Guid targetId, DateTime sinceUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<ModerationReport>> ListAsync(Guid territoryId, ReportTargetType? targetType, ReportStatus? status, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<ModerationReport>> ListPagedAsync(Guid territoryId, ReportTargetType? targetType, ReportStatus? status, DateTime? fromUtc, DateTime? toUtc, int skip, int take, CancellationToken cancellationToken);
    Task<int> CountAsync(Guid territoryId, ReportTargetType? targetType, ReportStatus? status, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid reportId, ReportStatus status, CancellationToken cancellationToken);
}
