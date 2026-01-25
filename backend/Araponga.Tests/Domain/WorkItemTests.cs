using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Domain.Work;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class WorkItemTests
{
    private WorkItem CreateValidWorkItem(
        WorkItemStatus status = WorkItemStatus.Pending,
        WorkItemOutcome outcome = WorkItemOutcome.None,
        Guid? territoryId = null,
        string? payloadJson = null,
        string? completionNotes = null)
    {
        return new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            status,
            territoryId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            "REPORT",
            Guid.NewGuid(),
            payloadJson,
            outcome,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes);
    }

    [Fact]
    public void WorkItem_RequiresTerritoryId_WhenRequiredCapabilitySet()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ResidencyVerification,
                WorkItemStatus.Pending,
                territoryId: null,
                createdByUserId: Guid.NewGuid(),
                createdAtUtc: DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: MembershipCapabilityType.Curator,
                subjectType: "MEMBERSHIP",
                subjectId: Guid.NewGuid(),
                payloadJson: null,
                outcome: WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("TerritoryId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithEmptyId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.Empty,
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                "REPORT",
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("ID is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithEmptyCreatedByUserId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.Empty,
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                "REPORT",
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("CreatedByUserId is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithNullSubjectType_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                subjectType: null!,
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("SubjectType is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithEmptySubjectType_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                subjectType: "   ",
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("SubjectType is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithSubjectTypeTooLong_ThrowsArgumentException()
    {
        var longSubjectType = new string('A', WorkItem.MaxSubjectTypeLength + 1);
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                subjectType: longSubjectType,
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("must not exceed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithEmptySubjectId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                "REPORT",
                Guid.Empty,
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("SubjectId is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithPayloadTooLong_ThrowsArgumentException()
    {
        var longPayload = new string('A', WorkItem.MaxPayloadLength + 1);
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                "REPORT",
                Guid.NewGuid(),
                payloadJson: longPayload,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: null));

        Assert.Contains("Payload must not exceed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_WithCompletionNotesTooLong_ThrowsArgumentException()
    {
        var longNotes = new string('A', WorkItem.MaxNotesLength + 1);
        var ex = Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                Guid.NewGuid(),
                WorkItemType.ModerationCase,
                WorkItemStatus.Pending,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                requiredSystemPermission: null,
                requiredCapability: null,
                "REPORT",
                Guid.NewGuid(),
                payloadJson: null,
                WorkItemOutcome.None,
                completedAtUtc: null,
                completedByUserId: null,
                completionNotes: longNotes));

        Assert.Contains("Completion notes must not exceed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_NormalizesSubjectType_ToUpperCase()
    {
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.Pending,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            subjectType: "  report  ",
            Guid.NewGuid(),
            payloadJson: null,
            WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        Assert.Equal("REPORT", item.SubjectType);
    }

    [Fact]
    public void WorkItem_NormalizesPayload_TrimsWhitespace()
    {
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.Pending,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            "REPORT",
            Guid.NewGuid(),
            payloadJson: "  {\"key\":\"value\"}  ",
            WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        Assert.Equal("{\"key\":\"value\"}", item.PayloadJson);
    }

    [Fact]
    public void WorkItem_NormalizesCompletionNotes_TrimsWhitespace()
    {
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.Completed,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            "REPORT",
            Guid.NewGuid(),
            payloadJson: null,
            WorkItemOutcome.Approved,
            completedAtUtc: DateTime.UtcNow,
            completedByUserId: Guid.NewGuid(),
            completionNotes: "  notes  ");

        Assert.Equal("notes", item.CompletionNotes);
    }

    [Fact]
    public void WorkItem_Complete_SetsCompletionFields()
    {
        var item = CreateValidWorkItem(WorkItemStatus.RequiresHumanReview);
        var actor = Guid.NewGuid();
        var now = DateTime.UtcNow;
        item.Complete(WorkItemOutcome.Approved, actor, "ok", now);

        Assert.Equal(WorkItemStatus.Completed, item.Status);
        Assert.Equal(actor, item.CompletedByUserId);
        Assert.Equal(now, item.CompletedAtUtc);
        Assert.Equal("ok", item.CompletionNotes);
        Assert.Equal(WorkItemOutcome.Approved, item.Outcome);
    }

    [Fact]
    public void WorkItem_Complete_WithEmptyCompletedByUserId_ThrowsArgumentException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var ex = Assert.Throws<ArgumentException>(() =>
            item.Complete(WorkItemOutcome.Approved, Guid.Empty, "notes", DateTime.UtcNow));

        Assert.Contains("CompletedByUserId is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_WithNoneOutcome_ThrowsArgumentException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var ex = Assert.Throws<ArgumentException>(() =>
            item.Complete(WorkItemOutcome.None, Guid.NewGuid(), "notes", DateTime.UtcNow));

        Assert.Contains("Outcome is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_WhenAlreadyCompleted_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Completed, WorkItemOutcome.Approved);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.Complete(WorkItemOutcome.Rejected, Guid.NewGuid(), "notes", DateTime.UtcNow));

        Assert.Contains("already completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_WhenCancelled_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Cancelled);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.Complete(WorkItemOutcome.Approved, Guid.NewGuid(), "notes", DateTime.UtcNow));

        Assert.Contains("cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_WithNotesTooLong_ThrowsArgumentException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var longNotes = new string('A', WorkItem.MaxNotesLength + 1);
        var ex = Assert.Throws<ArgumentException>(() =>
            item.Complete(WorkItemOutcome.Approved, Guid.NewGuid(), longNotes, DateTime.UtcNow));

        Assert.Contains("must not exceed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_NormalizesNotes_TrimsWhitespace()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        item.Complete(WorkItemOutcome.Approved, Guid.NewGuid(), "  notes  ", DateTime.UtcNow);

        Assert.Equal("notes", item.CompletionNotes);
    }

    [Fact]
    public void WorkItem_Complete_WithNullNotes_SetsNull()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        item.Complete(WorkItemOutcome.Approved, Guid.NewGuid(), null, DateTime.UtcNow);

        Assert.Null(item.CompletionNotes);
    }

    [Fact]
    public void WorkItem_Cancel_SetsCancellationFields()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var actor = Guid.NewGuid();
        var now = DateTime.UtcNow;
        item.Cancel(actor, "cancelled", now);

        Assert.Equal(WorkItemStatus.Cancelled, item.Status);
        Assert.Equal(actor, item.CompletedByUserId);
        Assert.Equal(now, item.CompletedAtUtc);
        Assert.Equal("cancelled", item.CompletionNotes);
        Assert.Equal(WorkItemOutcome.NoAction, item.Outcome);
    }

    [Fact]
    public void WorkItem_Cancel_WithEmptyCancelledByUserId_ThrowsArgumentException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var ex = Assert.Throws<ArgumentException>(() =>
            item.Cancel(Guid.Empty, "notes", DateTime.UtcNow));

        Assert.Contains("CancelledByUserId is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Cancel_WhenAlreadyCompleted_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Completed, WorkItemOutcome.Approved);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.Cancel(Guid.NewGuid(), "notes", DateTime.UtcNow));

        Assert.Contains("already completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Cancel_WhenAlreadyCancelled_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Cancelled);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.Cancel(Guid.NewGuid(), "notes", DateTime.UtcNow));

        Assert.Contains("already cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Cancel_WithNotesTooLong_ThrowsArgumentException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        var longNotes = new string('A', WorkItem.MaxNotesLength + 1);
        var ex = Assert.Throws<ArgumentException>(() =>
            item.Cancel(Guid.NewGuid(), longNotes, DateTime.UtcNow));

        Assert.Contains("must not exceed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Cancel_NormalizesNotes_TrimsWhitespace()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        item.Cancel(Guid.NewGuid(), "  notes  ", DateTime.UtcNow);

        Assert.Equal("notes", item.CompletionNotes);
    }

    [Fact]
    public void WorkItem_MarkRequiresHumanReview_SetsStatus()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        item.MarkRequiresHumanReview();

        Assert.Equal(WorkItemStatus.RequiresHumanReview, item.Status);
    }

    [Fact]
    public void WorkItem_MarkRequiresHumanReview_WhenCompleted_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Completed, WorkItemOutcome.Approved);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.MarkRequiresHumanReview());

        Assert.Contains("completed/cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_MarkRequiresHumanReview_WhenCancelled_ThrowsInvalidOperationException()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Cancelled);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            item.MarkRequiresHumanReview());

        Assert.Contains("completed/cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorkItem_Complete_WithAllOutcomes_Works()
    {
        var outcomes = new[] { WorkItemOutcome.Approved, WorkItemOutcome.Rejected, WorkItemOutcome.NoAction };
        foreach (var outcome in outcomes)
        {
            var item = CreateValidWorkItem(WorkItemStatus.Pending);
            item.Complete(outcome, Guid.NewGuid(), "notes", DateTime.UtcNow);
            Assert.Equal(outcome, item.Outcome);
            Assert.Equal(WorkItemStatus.Completed, item.Status);
        }
    }
}

