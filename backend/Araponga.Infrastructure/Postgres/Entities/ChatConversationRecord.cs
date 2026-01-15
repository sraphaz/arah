using Araponga.Domain.Chat;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class ChatConversationRecord
{
    public Guid Id { get; set; }
    public Guid? TerritoryId { get; set; }
    public ConversationKind Kind { get; set; }
    public ConversationStatus Status { get; set; }
    public string? Name { get; set; }

    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }

    public Guid? LockedByUserId { get; set; }
    public DateTime? LockedAtUtc { get; set; }

    public Guid? DisabledByUserId { get; set; }
    public DateTime? DisabledAtUtc { get; set; }
}

