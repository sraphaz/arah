using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class SystemPermissionRepositoryTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task SystemPermissionRepository_AddAndGetById()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemorySystemPermissionRepository(sharedStore);

        var permission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await repository.AddAsync(permission, CancellationToken.None);

        var found = await repository.GetByIdAsync(permission.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(permission.Id, found!.Id);
        Assert.Equal(SystemPermissionType.SystemAdmin, found.PermissionType);
    }

    [Fact]
    public async Task SystemPermissionRepository_GetActiveByUserId()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemorySystemPermissionRepository(sharedStore);

        var activePermission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        var revokedPermission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SupportAgent,
            DateTime.UtcNow,
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        await repository.AddAsync(activePermission, CancellationToken.None);
        await repository.AddAsync(revokedPermission, CancellationToken.None);

        var active = await repository.GetActiveByUserIdAsync(UserId, CancellationToken.None);
        Assert.Single(active);
        Assert.Equal(activePermission.Id, active[0].Id);
    }

    [Fact]
    public async Task SystemPermissionRepository_HasPermission_ReturnsTrue_WhenActive()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemorySystemPermissionRepository(sharedStore);

        var permission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await repository.AddAsync(permission, CancellationToken.None);

        var hasPermission = await repository.HasActivePermissionAsync(UserId, SystemPermissionType.SystemAdmin, CancellationToken.None);
        Assert.True(hasPermission);
    }

    [Fact]
    public async Task SystemPermissionRepository_HasPermission_ReturnsFalse_WhenRevoked()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemorySystemPermissionRepository(sharedStore);

        var permission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        await repository.AddAsync(permission, CancellationToken.None);

        var hasPermission = await repository.HasActivePermissionAsync(UserId, SystemPermissionType.SystemAdmin, CancellationToken.None);
        Assert.False(hasPermission);
    }
}
