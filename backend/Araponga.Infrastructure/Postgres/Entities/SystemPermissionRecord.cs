using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class SystemPermissionRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SystemPermissionType PermissionType { get; set; }
    public DateTime GrantedAtUtc { get; set; }
    public Guid? GrantedByUserId { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public Guid? RevokedByUserId { get; set; }
}
