using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserTerritoryRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TerritoryId { get; set; }
    public MembershipStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
