using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class SystemPermissionTests
{
    [Fact]
    public void SystemPermission_RequiresId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new SystemPermission(
                Guid.Empty,
                Guid.NewGuid(),
                SystemPermissionType.SystemAdmin,
                DateTime.UtcNow,
                null,
                null,
                null));

        Assert.Contains("ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SystemPermission_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new SystemPermission(
                Guid.NewGuid(),
                Guid.Empty,
                SystemPermissionType.SystemAdmin,
                DateTime.UtcNow,
                null,
                null,
                null));

        Assert.Contains("User ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SystemPermission_IsActive_WhenNotRevoked()
    {
        var permission = new SystemPermission(
            Guid.NewGuid(),
            Guid.NewGuid(),
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        Assert.True(permission.IsActive());
    }

    [Fact]
    public void SystemPermission_IsNotActive_WhenRevoked()
    {
        var permission = new SystemPermission(
            Guid.NewGuid(),
            Guid.NewGuid(),
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        Assert.False(permission.IsActive());
    }

    [Fact]
    public void SystemPermission_Revoke_SetsRevokedAt()
    {
        var permission = new SystemPermission(
            Guid.NewGuid(),
            Guid.NewGuid(),
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        var revokedBy = Guid.NewGuid();
        var revokedAt = DateTime.UtcNow;
        permission.Revoke(revokedBy, revokedAt);

        Assert.False(permission.IsActive());
        Assert.NotNull(permission.RevokedAtUtc);
        Assert.Equal(revokedBy, permission.RevokedByUserId);
    }

    [Fact]
    public void SystemPermission_Revoke_Throws_WhenAlreadyRevoked()
    {
        var permission = new SystemPermission(
            Guid.NewGuid(),
            Guid.NewGuid(),
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        var exception = Assert.Throws<InvalidOperationException>(() =>
            permission.Revoke(Guid.NewGuid(), DateTime.UtcNow));

        Assert.Contains("already revoked", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
