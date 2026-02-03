using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface ISystemPermissionRepository
{
    Task<SystemPermission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SystemPermission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SystemPermission>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> HasActivePermissionAsync(Guid userId, SystemPermissionType permissionType, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListUserIdsWithPermissionAsync(SystemPermissionType permissionType, CancellationToken cancellationToken);
    Task AddAsync(SystemPermission permission, CancellationToken cancellationToken);
    Task UpdateAsync(SystemPermission permission, CancellationToken cancellationToken);
}
