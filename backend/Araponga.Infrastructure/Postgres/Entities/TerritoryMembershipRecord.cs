using Araponga.Domain.Social;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class TerritoryMembershipRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TerritoryId { get; set; }
    public MembershipRole Role { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
