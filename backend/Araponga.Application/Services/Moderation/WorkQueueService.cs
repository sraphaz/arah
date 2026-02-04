using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Work;

namespace Araponga.Application.Services;

public sealed class WorkQueueService
{
    private readonly IWorkItemRepository _repository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public WorkQueueService(
        IWorkItemRepository repository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<WorkItem>> ListAsync(
        WorkItemType? type,
        WorkItemStatus? status,
        Guid? territoryId,
        CancellationToken cancellationToken)
        => _repository.ListAsync(type, status, territoryId, cancellationToken);

    public Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _repository.GetByIdAsync(id, cancellationToken);

    public async Task<OperationResult<WorkItem>> EnqueueAsync(
        WorkItemType type,
        Guid? territoryId,
        Guid createdByUserId,
        SystemPermissionType? requiredSystemPermission,
        MembershipCapabilityType? requiredCapability,
        string subjectType,
        Guid subjectId,
        string? payloadJson,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var item = new WorkItem(
                Guid.NewGuid(),
                type,
                WorkItemStatus.Pending,
                territoryId,
                createdByUserId,
                now,
                requiredSystemPermission,
                requiredCapability,
                subjectType,
                subjectId,
                payloadJson,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null);

            await _repository.AddAsync(item, cancellationToken);

            await _auditLogger.LogAsync(
                new AuditEntry("work_item.created", createdByUserId, territoryId ?? Guid.Empty, item.Id, now),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult<WorkItem>.Success(item);
        }
        catch (Exception ex)
        {
            return OperationResult<WorkItem>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult> CompleteAsync(
        Guid workItemId,
        Guid actorUserId,
        WorkItemOutcome outcome,
        string? notes,
        CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(workItemId, cancellationToken);
        if (item is null)
        {
            return OperationResult.Failure("Work item not found.");
        }

        try
        {
            var now = DateTime.UtcNow;
            item.Complete(outcome, actorUserId, notes, now);

            await _repository.UpdateAsync(item, cancellationToken);

            await _auditLogger.LogAsync(
                new AuditEntry("work_item.completed", actorUserId, item.TerritoryId ?? Guid.Empty, item.Id, now),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex.Message);
        }
    }
}

