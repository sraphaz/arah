using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Modules.Moderation.Domain.Work;
using Xunit;

namespace Araponga.Tests.Modules.Moderation.Domain;

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
    public void WorkItem_MarkRequiresHumanReview_SetsStatus()
    {
        var item = CreateValidWorkItem(WorkItemStatus.Pending);
        item.MarkRequiresHumanReview();

        Assert.Equal(WorkItemStatus.RequiresHumanReview, item.Status);
    }
}
