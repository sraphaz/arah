using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Feed;
using Araponga.Domain.Moderation;
using Araponga.Domain.Work;

namespace Araponga.Application.Services;

/// <summary>
/// Aplica decisão humana para casos de moderação derivados de reports (WorkItem SubjectType=REPORT).
/// </summary>
public sealed class ModerationCaseService
{
    private static readonly TimeSpan DefaultSanctionDuration = TimeSpan.FromDays(7);

    private readonly IWorkItemRepository _workItemRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public ModerationCaseService(
        IWorkItemRepository workItemRepository,
        IReportRepository reportRepository,
        IFeedRepository feedRepository,
        ISanctionRepository sanctionRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _workItemRepository = workItemRepository;
        _reportRepository = reportRepository;
        _feedRepository = feedRepository;
        _sanctionRepository = sanctionRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// outcome:
    /// - Approved: report procedente -> aplica ação (post hidden / sanção usuário) e marca report Actioned
    /// - Rejected: report improcedente -> marca report Reviewed
    /// - NoAction: revisado sem ação -> marca report Reviewed
    /// </summary>
    public async Task<OperationResult> DecideAsync(
        Guid workItemId,
        Guid actorUserId,
        WorkItemOutcome outcome,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (outcome == WorkItemOutcome.None)
        {
            return OperationResult.Failure("Outcome is required.");
        }

        var item = await _workItemRepository.GetByIdAsync(workItemId, cancellationToken);
        if (item is null)
        {
            return OperationResult.Failure("Work item not found.");
        }

        if (item.Type != WorkItemType.ModerationCase || item.SubjectType != "REPORT")
        {
            return OperationResult.Failure("Work item is not a moderation case.");
        }

        if (item.TerritoryId is null)
        {
            return OperationResult.Failure("TerritoryId is required for moderation cases.");
        }

        var report = await _reportRepository.GetByIdAsync(item.SubjectId, cancellationToken);
        if (report is null)
        {
            return OperationResult.Failure("Report not found.");
        }

        if (report.TerritoryId != item.TerritoryId.Value)
        {
            return OperationResult.Failure("Report territory mismatch.");
        }

        var now = DateTime.UtcNow;

        if (outcome == WorkItemOutcome.Approved)
        {
            // Aplica ação conforme alvo do report
            if (report.TargetType == ReportTargetType.Post)
            {
                var post = await _feedRepository.GetPostAsync(report.TargetId, cancellationToken);
                if (post is not null && post.TerritoryId == report.TerritoryId && post.Status != PostStatus.Hidden)
                {
                    await _feedRepository.UpdateStatusAsync(post.Id, PostStatus.Hidden, cancellationToken);
                }

                await _auditLogger.LogAsync(
                    new AuditEntry("moderation.manual.post_hidden", actorUserId, report.TerritoryId, report.TargetId, now),
                    cancellationToken);
            }
            else if (report.TargetType == ReportTargetType.User)
            {
                var alreadyRestricted = await _sanctionRepository.HasActiveSanctionAsync(
                    report.TargetId,
                    report.TerritoryId,
                    SanctionType.PostingRestriction,
                    now,
                    cancellationToken);

                if (!alreadyRestricted)
                {
                    var sanction = new Sanction(
                        Guid.NewGuid(),
                        report.TerritoryId,
                        SanctionScope.Territory,
                        SanctionTargetType.User,
                        report.TargetId,
                        SanctionType.PostingRestriction,
                        "Manual moderation action from work item.",
                        SanctionStatus.Active,
                        now,
                        now.Add(DefaultSanctionDuration),
                        now);

                    await _sanctionRepository.AddAsync(sanction, cancellationToken);
                }

                await _auditLogger.LogAsync(
                    new AuditEntry("moderation.manual.user_sanctioned", actorUserId, report.TerritoryId, report.TargetId, now),
                    cancellationToken);
            }

            await _reportRepository.UpdateStatusAsync(report.Id, ReportStatus.Actioned, cancellationToken);
        }
        else
        {
            await _reportRepository.UpdateStatusAsync(report.Id, ReportStatus.Reviewed, cancellationToken);
        }

        item.Complete(outcome, actorUserId, notes, now);
        await _workItemRepository.UpdateAsync(item, cancellationToken);
        await _auditLogger.LogAsync(new AuditEntry("work_item.completed", actorUserId, report.TerritoryId, item.Id, now), cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }
}

