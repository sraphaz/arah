using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class TerritoryMembershipRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TerritoryId { get; set; }
    public MembershipRole Role { get; set; }
    public ResidencyVerification ResidencyVerification { get; set; }
    public DateTime? LastGeoVerifiedAtUtc { get; set; }
    public DateTime? LastDocumentVerifiedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
