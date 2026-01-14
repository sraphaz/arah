using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySystemPermissionRepository : ISystemPermissionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySystemPermissionRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SystemPermission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken)
    {
        var permission = _dataStore.SystemPermissions.FirstOrDefault(p => p.Id == permissionId);
        return Task.FromResult(permission);
    }

    public Task<IReadOnlyList<SystemPermission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var permissions = _dataStore.SystemPermissions
            .Where(p => p.UserId == userId)
            .ToList();
        return Task.FromResult<IReadOnlyList<SystemPermission>>(permissions);
    }

    public Task<IReadOnlyList<SystemPermission>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var permissions = _dataStore.SystemPermissions
            .Where(p => p.UserId == userId && p.IsActive())
            .ToList();
        return Task.FromResult<IReadOnlyList<SystemPermission>>(permissions);
    }

    public Task<bool> HasActivePermissionAsync(Guid userId, SystemPermissionType permissionType, CancellationToken cancellationToken)
    {
        var hasPermission = _dataStore.SystemPermissions
            .Any(p => p.UserId == userId &&
                      p.PermissionType == permissionType &&
                      p.IsActive());
        return Task.FromResult(hasPermission);
    }

    public Task<IReadOnlyList<Guid>> ListUserIdsWithPermissionAsync(SystemPermissionType permissionType, CancellationToken cancellationToken)
    {
        var userIds = _dataStore.SystemPermissions
            .Where(p => p.PermissionType == permissionType && p.IsActive())
            .Select(p => p.UserId)
            .Distinct()
            .ToList();
        return Task.FromResult<IReadOnlyList<Guid>>(userIds);
    }

    public Task AddAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        _dataStore.SystemPermissions.Add(permission);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(permission.Id, cancellationToken);
        if (existing is null)
        {
            await AddAsync(permission, cancellationToken);
        }
        else
        {
            var index = _dataStore.SystemPermissions.IndexOf(existing);
            if (index >= 0)
            {
                _dataStore.SystemPermissions[index] = permission;
            }
        }
    }
}
