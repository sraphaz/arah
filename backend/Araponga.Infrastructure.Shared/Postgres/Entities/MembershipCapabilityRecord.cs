using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class MembershipCapabilityRecord
{
    public Guid Id { get; set; }
    public Guid MembershipId { get; set; }
    public MembershipCapabilityType CapabilityType { get; set; }
    public DateTime GrantedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public Guid? GrantedByUserId { get; set; }
    public Guid? GrantedByMembershipId { get; set; }
    public string? Reason { get; set; }
}
