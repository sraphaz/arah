using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Modules.Moderation.Domain.Work;

namespace Araponga.Modules.Moderation.Infrastructure.Postgres.Entities;

public sealed class WorkItemRecord
{
    public Guid Id { get; set; }
    public WorkItemType Type { get; set; }
    public WorkItemStatus Status { get; set; }
    public Guid? TerritoryId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public SystemPermissionType? RequiredSystemPermission { get; set; }
    public MembershipCapabilityType? RequiredCapability { get; set; }
    public string SubjectType { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string? PayloadJson { get; set; }
    public WorkItemOutcome Outcome { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Guid? CompletedByUserId { get; set; }
    public string? CompletionNotes { get; set; }
}
