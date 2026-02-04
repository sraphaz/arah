using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Modules.Moderation.Domain.Work;

/// <summary>
/// Item de trabalho genérico para suportar automação + fallback humano (fila).
/// </summary>
public sealed class WorkItem
{
    public const int MaxSubjectTypeLength = 50;
    public const int MaxNotesLength = 2_000;
    public const int MaxPayloadLength = 20_000;

    public WorkItem(
        Guid id,
        WorkItemType type,
        WorkItemStatus status,
        Guid? territoryId,
        Guid createdByUserId,
        DateTime createdAtUtc,
        SystemPermissionType? requiredSystemPermission,
        MembershipCapabilityType? requiredCapability,
        string subjectType,
        Guid subjectId,
        string? payloadJson,
        WorkItemOutcome outcome,
        DateTime? completedAtUtc,
        Guid? completedByUserId,
        string? completionNotes)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID is required.", nameof(id));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(subjectType)) throw new ArgumentException("SubjectType is required.", nameof(subjectType));
        var normalizedSubjectType = subjectType.Trim().ToUpperInvariant();
        if (normalizedSubjectType.Length > MaxSubjectTypeLength) throw new ArgumentException($"SubjectType must not exceed {MaxSubjectTypeLength} characters.", nameof(subjectType));
        if (subjectId == Guid.Empty) throw new ArgumentException("SubjectId is required.", nameof(subjectId));
        var normalizedPayload = string.IsNullOrWhiteSpace(payloadJson) ? null : payloadJson.Trim();
        if (normalizedPayload is not null && normalizedPayload.Length > MaxPayloadLength) throw new ArgumentException($"Payload must not exceed {MaxPayloadLength} characters.", nameof(payloadJson));
        var normalizedNotes = string.IsNullOrWhiteSpace(completionNotes) ? null : completionNotes.Trim();
        if (normalizedNotes is not null && normalizedNotes.Length > MaxNotesLength) throw new ArgumentException($"Completion notes must not exceed {MaxNotesLength} characters.", nameof(completionNotes));
        if (requiredCapability.HasValue && territoryId is null) throw new ArgumentException("TerritoryId is required when RequiredCapability is set.", nameof(territoryId));

        Id = id;
        Type = type;
        Status = status;
        TerritoryId = territoryId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
        RequiredSystemPermission = requiredSystemPermission;
        RequiredCapability = requiredCapability;
        SubjectType = normalizedSubjectType;
        SubjectId = subjectId;
        PayloadJson = normalizedPayload;
        Outcome = outcome;
        CompletedAtUtc = completedAtUtc;
        CompletedByUserId = completedByUserId;
        CompletionNotes = normalizedNotes;
    }

    public Guid Id { get; }
    public WorkItemType Type { get; }
    public WorkItemStatus Status { get; private set; }
    public Guid? TerritoryId { get; }
    public Guid CreatedByUserId { get; }
    public DateTime CreatedAtUtc { get; }
    public SystemPermissionType? RequiredSystemPermission { get; }
    public MembershipCapabilityType? RequiredCapability { get; }
    public string SubjectType { get; }
    public Guid SubjectId { get; }
    public string? PayloadJson { get; }
    public WorkItemOutcome Outcome { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public Guid? CompletedByUserId { get; private set; }
    public string? CompletionNotes { get; private set; }

    public void MarkRequiresHumanReview()
    {
        if (Status == WorkItemStatus.Completed || Status == WorkItemStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status of a completed/cancelled work item.");
        Status = WorkItemStatus.RequiresHumanReview;
    }

    public void Complete(WorkItemOutcome outcome, Guid completedByUserId, string? notes, DateTime completedAtUtc)
    {
        if (completedByUserId == Guid.Empty) throw new ArgumentException("CompletedByUserId is required.", nameof(completedByUserId));
        if (outcome == WorkItemOutcome.None) throw new ArgumentException("Outcome is required.", nameof(outcome));
        if (Status == WorkItemStatus.Completed) throw new InvalidOperationException("Work item is already completed.");
        if (Status == WorkItemStatus.Cancelled) throw new InvalidOperationException("Work item is cancelled.");
        var normalizedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (normalizedNotes is not null && normalizedNotes.Length > MaxNotesLength) throw new ArgumentException($"Completion notes must not exceed {MaxNotesLength} characters.", nameof(notes));
        Status = WorkItemStatus.Completed;
        Outcome = outcome;
        CompletedAtUtc = completedAtUtc;
        CompletedByUserId = completedByUserId;
        CompletionNotes = normalizedNotes;
    }

    public void Cancel(Guid cancelledByUserId, string? notes, DateTime cancelledAtUtc)
    {
        if (cancelledByUserId == Guid.Empty) throw new ArgumentException("CancelledByUserId is required.", nameof(cancelledByUserId));
        if (Status == WorkItemStatus.Completed) throw new InvalidOperationException("Work item is already completed.");
        if (Status == WorkItemStatus.Cancelled) throw new InvalidOperationException("Work item is already cancelled.");
        var normalizedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (normalizedNotes is not null && normalizedNotes.Length > MaxNotesLength) throw new ArgumentException($"Cancellation notes must not exceed {MaxNotesLength} characters.", nameof(notes));
        Status = WorkItemStatus.Cancelled;
        Outcome = WorkItemOutcome.NoAction;
        CompletedAtUtc = cancelledAtUtc;
        CompletedByUserId = cancelledByUserId;
        CompletionNotes = normalizedNotes;
    }
}
