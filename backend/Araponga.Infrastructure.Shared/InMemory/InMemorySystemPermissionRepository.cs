using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ISystemPermissionRepository usando InMemorySharedStore.</summary>
public sealed class InMemorySystemPermissionRepository : ISystemPermissionRepository
{
    private readonly InMemorySharedStore _store;

    public InMemorySystemPermissionRepository(InMemorySharedStore store) => _store = store;

    public Task<SystemPermission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken)
        => Task.FromResult(_store.SystemPermissions.FirstOrDefault(p => p.Id == permissionId));

    public Task<IReadOnlyList<SystemPermission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<SystemPermission>>(_store.SystemPermissions.Where(p => p.UserId == userId).ToList());

    public Task<IReadOnlyList<SystemPermission>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<SystemPermission>>(_store.SystemPermissions.Where(p => p.UserId == userId && p.IsActive()).ToList());

    public Task<bool> HasActivePermissionAsync(Guid userId, SystemPermissionType permissionType, CancellationToken cancellationToken)
        => Task.FromResult(_store.SystemPermissions.Any(p => p.UserId == userId && p.PermissionType == permissionType && p.IsActive()));

    public Task<IReadOnlyList<Guid>> ListUserIdsWithPermissionAsync(SystemPermissionType permissionType, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Guid>>(_store.SystemPermissions.Where(p => p.PermissionType == permissionType && p.IsActive()).Select(p => p.UserId).Distinct().ToList());

    public Task AddAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        _store.SystemPermissions.Add(permission);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        var existing = _store.SystemPermissions.FirstOrDefault(p => p.Id == permission.Id);
        if (existing is null) _store.SystemPermissions.Add(permission);
        else { var i = _store.SystemPermissions.IndexOf(existing); if (i >= 0) _store.SystemPermissions[i] = permission; }
        return Task.CompletedTask;
    }
}
