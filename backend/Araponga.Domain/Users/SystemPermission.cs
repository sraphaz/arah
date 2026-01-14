namespace Araponga.Domain.Users;

/// <summary>
/// Representa uma permissão global do sistema atribuída a um User.
/// Permissões são globais, não territoriais, e podem ser revogadas.
/// </summary>
public sealed class SystemPermission
{
    public SystemPermission(
        Guid id,
        Guid userId,
        SystemPermissionType permissionType,
        DateTime grantedAtUtc,
        Guid? grantedByUserId,
        DateTime? revokedAtUtc,
        Guid? revokedByUserId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        Id = id;
        UserId = userId;
        PermissionType = permissionType;
        GrantedAtUtc = grantedAtUtc;
        GrantedByUserId = grantedByUserId;
        RevokedAtUtc = revokedAtUtc;
        RevokedByUserId = revokedByUserId;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public SystemPermissionType PermissionType { get; }
    public DateTime GrantedAtUtc { get; }
    public Guid? GrantedByUserId { get; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }

    public bool IsActive() => RevokedAtUtc is null;

    public void Revoke(Guid revokedByUserId, DateTime revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
        {
            throw new InvalidOperationException("Permission is already revoked.");
        }

        RevokedAtUtc = revokedAtUtc;
        RevokedByUserId = revokedByUserId;
    }
}
