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
}
