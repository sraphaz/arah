using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Work;

namespace Araponga.Application.Events;

/// <summary>
/// Enfileira um WorkItem de moderação quando um report é criado.
/// Nesta fase, direciona para Curator (que já recebe notificações de report).
/// </summary>
public sealed class ReportCreatedWorkItemHandler : IEventHandler<ReportCreatedEvent>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IAuditLogger _auditLogger;

    public ReportCreatedWorkItemHandler(
        IWorkItemRepository workItemRepository,
        IAuditLogger auditLogger)
    {
        _workItemRepository = workItemRepository;
        _auditLogger = auditLogger;
    }

    public async Task HandleAsync(ReportCreatedEvent appEvent, CancellationToken cancellationToken)
    {
        var now = appEvent.OccurredAtUtc;
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.RequiresHumanReview,
            territoryId: appEvent.TerritoryId,
            createdByUserId: appEvent.ReporterUserId,
            createdAtUtc: now,
            requiredSystemPermission: null,
            requiredCapability: MembershipCapabilityType.Curator,
            subjectType: "REPORT",
            subjectId: appEvent.ReportId,
            payloadJson: $$"""{"reportId":"{{appEvent.ReportId}}","territoryId":"{{appEvent.TerritoryId}}"}""",
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await _workItemRepository.AddAsync(item, cancellationToken);
        await _auditLogger.LogAsync(new AuditEntry("work_item.created", appEvent.ReporterUserId, appEvent.TerritoryId, item.Id, now), cancellationToken);
    }
}

