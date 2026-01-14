using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Domain.Work;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class WorkItemTests
{
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
    public void WorkItem_Complete_SetsCompletionFields()
    {
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.RequiresHumanReview,
            territoryId: Guid.NewGuid(),
            createdByUserId: Guid.NewGuid(),
            createdAtUtc: DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: MembershipCapabilityType.Moderator,
            subjectType: "REPORT",
            subjectId: Guid.NewGuid(),
            payloadJson: null,
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        var actor = Guid.NewGuid();
        var now = DateTime.UtcNow;
        item.Complete(WorkItemOutcome.Approved, actor, "ok", now);

        Assert.Equal(WorkItemStatus.Completed, item.Status);
        Assert.Equal(actor, item.CompletedByUserId);
        Assert.Equal(now, item.CompletedAtUtc);
        Assert.Equal("ok", item.CompletionNotes);
    }
}

