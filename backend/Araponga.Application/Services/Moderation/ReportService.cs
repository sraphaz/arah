using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Metrics;
using Araponga.Application.Events;
using Araponga.Domain.Media;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Application.Services;

public sealed class ReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObservabilityLogger? _observabilityLogger;

    public ReportService(
        IReportRepository reportRepository,
        IFeedRepository feedRepository,
        IUserRepository userRepository,
        ISanctionRepository sanctionRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        IAuditLogger auditLogger,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        IObservabilityLogger? observabilityLogger = null)
    {
        _reportRepository = reportRepository;
        _feedRepository = feedRepository;
        _userRepository = userRepository;
        _sanctionRepository = sanctionRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _auditLogger = auditLogger;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _observabilityLogger = observabilityLogger;
    }

    public async Task<(bool created, string? error, ModerationReport? report)> ReportPostAsync(
        Guid reporterUserId,
        Guid postId,
        string reason,
        string? details,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return (false, "Reason is required.", null);
        }

        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null)
        {
            return (false, "Post not found.", null);
        }

        var sinceUtc = DateTime.UtcNow.Subtract(Constants.Moderation.DuplicateWindow);
        var alreadyReported = await _reportRepository.HasRecentReportAsync(
            reporterUserId,
            ReportTargetType.Post,
            postId,
            sinceUtc,
            cancellationToken);

        if (alreadyReported)
        {
            return (false, null, null);
        }

        var report = new ModerationReport(
            Guid.NewGuid(),
            reporterUserId,
            post.TerritoryId,
            ReportTargetType.Post,
            postId,
            reason,
            details,
            ReportStatus.Open,
            DateTime.UtcNow);

        await _reportRepository.AddAsync(report, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("report.post", reporterUserId, post.TerritoryId, postId, DateTime.UtcNow),
            cancellationToken);

        _observabilityLogger?.LogReportCreated("POST", post.TerritoryId);

        await EvaluatePostThresholdAsync(report, post, cancellationToken);

        await _eventBus.PublishAsync(
            new ReportCreatedEvent(report.Id, report.TerritoryId, report.ReporterUserId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null, report);
    }

    public async Task<(bool created, string? error, ModerationReport? report)> ReportUserAsync(
        Guid reporterUserId,
        Guid territoryId,
        Guid reportedUserId,
        string reason,
        string? details,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return (false, "Reason is required.", null);
        }

        var user = await _userRepository.GetByIdAsync(reportedUserId, cancellationToken);
        if (user is null)
        {
            return (false, "User not found.", null);
        }

        if (territoryId == Guid.Empty)
        {
            return (false, "Territory ID is required.", null);
        }

        var sinceUtc = DateTime.UtcNow.Subtract(Constants.Moderation.DuplicateWindow);
        var alreadyReported = await _reportRepository.HasRecentReportAsync(
            reporterUserId,
            ReportTargetType.User,
            reportedUserId,
            sinceUtc,
            cancellationToken);

        if (alreadyReported)
        {
            return (false, null, null);
        }

        var report = new ModerationReport(
            Guid.NewGuid(),
            reporterUserId,
            territoryId,
            ReportTargetType.User,
            reportedUserId,
            reason,
            details,
            ReportStatus.Open,
            DateTime.UtcNow);

        await _reportRepository.AddAsync(report, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("report.user", reporterUserId, territoryId, reportedUserId, DateTime.UtcNow),
            cancellationToken);

        _observabilityLogger?.LogReportCreated("USER", territoryId);

        await EvaluateUserThresholdAsync(report, cancellationToken);

        await _eventBus.PublishAsync(
            new ReportCreatedEvent(report.Id, report.TerritoryId, report.ReporterUserId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null, report);
    }

    public Task<IReadOnlyList<ModerationReport>> ListAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        return _reportRepository.ListAsync(territoryId, targetType, status, fromUtc, toUtc, cancellationToken);
    }

    public async Task<PagedResult<ModerationReport>> ListPagedAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.ListAsync(territoryId, targetType, status, fromUtc, toUtc, cancellationToken);
        var ordered = reports.OrderByDescending(r => r.CreatedAtUtc).ToList();
        return ordered.ToPagedResult(pagination);
    }

    private async Task EvaluatePostThresholdAsync(
        ModerationReport report,
        Domain.Feed.CommunityPost post,
        CancellationToken cancellationToken)
    {
        var sinceUtc = DateTime.UtcNow.Subtract(Constants.Moderation.ThresholdWindow);
        var reportCount = await _reportRepository.CountDistinctReportersAsync(
            ReportTargetType.Post,
            report.TargetId,
            sinceUtc,
            cancellationToken);

        if (reportCount < Constants.Moderation.ReportThreshold)
        {
            return;
        }

        if (post.Status == Domain.Feed.PostStatus.Hidden)
        {
            return;
        }

        await _feedRepository.UpdateStatusAsync(post.Id, Domain.Feed.PostStatus.Hidden, cancellationToken);

        // Deletar mídias associadas ao post quando ocultado
        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.Post, post.Id, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("moderation.threshold.post", report.ReporterUserId, report.TerritoryId, post.Id, DateTime.UtcNow),
            cancellationToken);

        _observabilityLogger?.LogModerationFailure("EvaluatePostThreshold", $"Post {post.Id} hidden automatically", report.TerritoryId);
    }

    private async Task EvaluateUserThresholdAsync(
        ModerationReport report,
        CancellationToken cancellationToken)
    {
        var sinceUtc = DateTime.UtcNow.Subtract(Constants.Moderation.ThresholdWindow);
        var reportCount = await _reportRepository.CountDistinctReportersAsync(
            ReportTargetType.User,
            report.TargetId,
            sinceUtc,
            cancellationToken);

        if (reportCount < Constants.Moderation.ReportThreshold)
        {
            return;
        }

        var alreadyRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            report.TargetId,
            report.TerritoryId,
            SanctionType.PostingRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (alreadyRestricted)
        {
            return;
        }

        var sanction = new Sanction(
            Guid.NewGuid(),
            report.TerritoryId,
            SanctionScope.Territory,
            SanctionTargetType.User,
            report.TargetId,
            SanctionType.PostingRestriction,
            $"Automatic threshold reached for {report.TargetType}.",
            SanctionStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow.Add(Constants.Moderation.SanctionDuration),
            DateTime.UtcNow);

        await _sanctionRepository.AddAsync(sanction, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("moderation.threshold.user", report.ReporterUserId, report.TerritoryId, report.TargetId, DateTime.UtcNow),
            cancellationToken);

        _observabilityLogger?.LogModerationFailure("EvaluateUserThreshold", $"User {report.TargetId} sanctioned automatically", report.TerritoryId);
    }
}
