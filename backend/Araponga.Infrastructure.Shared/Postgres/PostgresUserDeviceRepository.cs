using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IUserDeviceRepository usando SharedDbContext.</summary>
public sealed class PostgresUserDeviceRepository : IUserDeviceRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresUserDeviceRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public Task AddAsync(UserDevice device, CancellationToken cancellationToken)
    {
        _dbContext.UserDevices.Add(device.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<UserDevice?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);
        return record is null ? null : record.ToDomain();
    }

    public async Task<IReadOnlyList<UserDevice>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.UserDevices
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.IsActive)
            .OrderByDescending(d => d.LastUsedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<UserDevice?> GetByTokenAsync(string deviceToken, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken, cancellationToken);
        return record is null ? null : record.ToDomain();
    }

    public async Task UpdateAsync(UserDevice device, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices.FirstOrDefaultAsync(d => d.Id == device.Id, cancellationToken);
        if (record is null) return;
        record.DeviceToken = device.DeviceToken;
        record.DeviceName = device.DeviceName;
        record.LastUsedAtUtc = device.LastUsedAtUtc;
        record.IsActive = device.IsActive;
    }

    public async Task DeleteAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices.FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);
        if (record is not null)
            _dbContext.UserDevices.Remove(record);
    }
}
