using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Evidence;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Domain.Work;

namespace Araponga.Application.Services;

/// <summary>
/// Orquestra fluxos de verificação (identidade global e residência territorial) usando Work Items.
/// Nesta fase, não há OCR/IA: toda submissão vai direto para RequiresHumanReview.
/// </summary>
public sealed class VerificationQueueService
{
    private readonly IUserRepository _userRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IDocumentEvidenceRepository _documentEvidenceRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public VerificationQueueService(
        IUserRepository userRepository,
        ITerritoryMembershipRepository membershipRepository,
        IWorkItemRepository workItemRepository,
        IDocumentEvidenceRepository documentEvidenceRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _workItemRepository = workItemRepository;
        _documentEvidenceRepository = documentEvidenceRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<WorkItem>> SubmitIdentityDocumentAsync(
        Guid userId,
        Guid evidenceId,
        CancellationToken cancellationToken)
    {
        if (evidenceId == Guid.Empty)
        {
            return OperationResult<WorkItem>.Failure("evidenceId is required.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return OperationResult<WorkItem>.Failure("User not found.");
        }

        var evidence = await _documentEvidenceRepository.GetByIdAsync(evidenceId, cancellationToken);
        if (evidence is null)
        {
            return OperationResult<WorkItem>.Failure("Evidence not found.");
        }

        if (evidence.UserId != userId || evidence.Kind != DocumentEvidenceKind.Identity || evidence.TerritoryId is not null)
        {
            return OperationResult<WorkItem>.Failure("Invalid evidence for identity verification.");
        }

        // Marca identidade como pendente
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Pending, null);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var now = DateTime.UtcNow;
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.IdentityVerification,
            WorkItemStatus.RequiresHumanReview,
            territoryId: null,
            createdByUserId: userId,
            createdAtUtc: now,
            requiredSystemPermission: SystemPermissionType.SystemAdmin,
            requiredCapability: null,
            subjectType: "USER",
            subjectId: userId,
            payloadJson: $$"""{"evidenceId":"{{evidenceId}}"}""",
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await _workItemRepository.AddAsync(item, cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("verification.identity.submitted", userId, Guid.Empty, item.Id, now),
            cancellationToken);
        await _auditLogger.LogAsync(
            new AuditEntry("work_item.created", userId, Guid.Empty, item.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<WorkItem>.Success(item);
    }

    public async Task<OperationResult<WorkItem>> SubmitResidencyDocumentAsync(
        Guid userId,
        Guid territoryId,
        Guid evidenceId,
        CancellationToken cancellationToken)
    {
        if (territoryId == Guid.Empty)
        {
            return OperationResult<WorkItem>.Failure("territoryId is required.");
        }

        if (evidenceId == Guid.Empty)
        {
            return OperationResult<WorkItem>.Failure("evidenceId is required.");
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        if (membership is null || membership.Role != MembershipRole.Resident)
        {
            return OperationResult<WorkItem>.Failure("Membership not found or user is not a Resident in this territory.");
        }

        var evidence = await _documentEvidenceRepository.GetByIdAsync(evidenceId, cancellationToken);
        if (evidence is null)
        {
            return OperationResult<WorkItem>.Failure("Evidence not found.");
        }

        if (evidence.UserId != userId ||
            evidence.Kind != DocumentEvidenceKind.Residency ||
            evidence.TerritoryId != territoryId)
        {
            return OperationResult<WorkItem>.Failure("Invalid evidence for residency verification.");
        }

        var now = DateTime.UtcNow;
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ResidencyVerification,
            WorkItemStatus.RequiresHumanReview,
            territoryId: territoryId,
            createdByUserId: userId,
            createdAtUtc: now,
            requiredSystemPermission: null,
            requiredCapability: MembershipCapabilityType.Curator,
            subjectType: "MEMBERSHIP",
            subjectId: membership.Id,
            payloadJson: $$"""{"evidenceId":"{{evidenceId}}","userId":"{{userId}}","territoryId":"{{territoryId}}"}""",
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await _workItemRepository.AddAsync(item, cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("verification.residency.submitted", userId, territoryId, item.Id, now),
            cancellationToken);
        await _auditLogger.LogAsync(
            new AuditEntry("work_item.created", userId, territoryId, item.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<WorkItem>.Success(item);
    }

    public async Task<OperationResult> DecideIdentityAsync(
        Guid workItemId,
        Guid adminUserId,
        WorkItemOutcome outcome,
        string? notes,
        CancellationToken cancellationToken)
    {
        var item = await _workItemRepository.GetByIdAsync(workItemId, cancellationToken);
        if (item is null)
        {
            return OperationResult.Failure("Work item not found.");
        }

        if (item.Type != WorkItemType.IdentityVerification || item.SubjectType != "USER")
        {
            return OperationResult.Failure("Work item is not an identity verification.");
        }

        var user = await _userRepository.GetByIdAsync(item.SubjectId, cancellationToken);
        if (user is null)
        {
            return OperationResult.Failure("User not found.");
        }

        var now = DateTime.UtcNow;
        if (outcome == WorkItemOutcome.Approved)
        {
            user.UpdateIdentityVerification(UserIdentityVerificationStatus.Verified, now);
            await _auditLogger.LogAsync(new AuditEntry("verification.identity.approved", adminUserId, Guid.Empty, user.Id, now), cancellationToken);
        }
        else if (outcome == WorkItemOutcome.Rejected)
        {
            user.UpdateIdentityVerification(UserIdentityVerificationStatus.Rejected, now);
            await _auditLogger.LogAsync(new AuditEntry("verification.identity.rejected", adminUserId, Guid.Empty, user.Id, now), cancellationToken);
        }
        else
        {
            return OperationResult.Failure("Invalid outcome for identity verification.");
        }

        item.Complete(outcome, adminUserId, notes, now);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _workItemRepository.UpdateAsync(item, cancellationToken);

        await _auditLogger.LogAsync(new AuditEntry("work_item.completed", adminUserId, Guid.Empty, item.Id, now), cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DecideResidencyAsync(
        Guid workItemId,
        Guid curatorUserId,
        WorkItemOutcome outcome,
        string? notes,
        CancellationToken cancellationToken)
    {
        var item = await _workItemRepository.GetByIdAsync(workItemId, cancellationToken);
        if (item is null)
        {
            return OperationResult.Failure("Work item not found.");
        }

        if (item.Type != WorkItemType.ResidencyVerification || item.SubjectType != "MEMBERSHIP")
        {
            return OperationResult.Failure("Work item is not a residency verification.");
        }

        var membership = await _membershipRepository.GetByIdAsync(item.SubjectId, cancellationToken);
        if (membership is null)
        {
            return OperationResult.Failure("Membership not found.");
        }

        var now = DateTime.UtcNow;
        if (outcome == WorkItemOutcome.Approved)
        {
            membership.AddDocumentVerification(now);
            await _auditLogger.LogAsync(new AuditEntry("verification.residency.approved", curatorUserId, membership.TerritoryId, membership.Id, now), cancellationToken);
        }
        else if (outcome == WorkItemOutcome.Rejected)
        {
            await _auditLogger.LogAsync(new AuditEntry("verification.residency.rejected", curatorUserId, membership.TerritoryId, membership.Id, now), cancellationToken);
        }
        else
        {
            return OperationResult.Failure("Invalid outcome for residency verification.");
        }

        item.Complete(outcome, curatorUserId, notes, now);
        await _membershipRepository.UpdateAsync(membership, cancellationToken);
        await _workItemRepository.UpdateAsync(item, cancellationToken);

        await _auditLogger.LogAsync(new AuditEntry("work_item.completed", curatorUserId, membership.TerritoryId, item.Id, now), cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }
}

