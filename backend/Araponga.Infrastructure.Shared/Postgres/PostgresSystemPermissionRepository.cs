using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de ISystemPermissionRepository usando SharedDbContext.</summary>
public sealed class PostgresSystemPermissionRepository : ISystemPermissionRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresSystemPermissionRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task<SystemPermission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SystemPermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == permissionId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SystemPermission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SystemPermissions
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<SystemPermission>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SystemPermissions
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<bool> HasActivePermissionAsync(Guid userId, SystemPermissionType permissionType, CancellationToken cancellationToken)
    {
        return await _dbContext.SystemPermissions
            .AsNoTracking()
            .AnyAsync(p => p.UserId == userId && p.PermissionType == permissionType && p.RevokedAtUtc == null, cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> ListUserIdsWithPermissionAsync(SystemPermissionType permissionType, CancellationToken cancellationToken)
    {
        return await _dbContext.SystemPermissions
            .AsNoTracking()
            .Where(p => p.PermissionType == permissionType && p.RevokedAtUtc == null)
            .Select(p => p.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        _dbContext.SystemPermissions.Add(permission.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(SystemPermission permission, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SystemPermissions.FirstOrDefaultAsync(p => p.Id == permission.Id, cancellationToken);
        if (record is null)
            _dbContext.SystemPermissions.Add(permission.ToRecord());
        else
            _dbContext.Entry(record).CurrentValues.SetValues(permission.ToRecord());
    }
}
