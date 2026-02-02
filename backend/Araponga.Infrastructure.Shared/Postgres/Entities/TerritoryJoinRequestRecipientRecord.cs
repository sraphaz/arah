namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class TerritoryJoinRequestRecipientRecord
{
    public Guid JoinRequestId { get; set; }
    public Guid RecipientUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
}
