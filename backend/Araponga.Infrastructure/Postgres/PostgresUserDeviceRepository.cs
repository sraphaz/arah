using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

internal static class UserDeviceMapperExtensions
{
    public static UserDevice ToDomain(this UserDeviceRecord record)
    {
        var device = new UserDevice(
            record.Id,
            record.UserId,
            record.DeviceToken,
            record.Platform,
            record.DeviceName,
            record.RegisteredAtUtc);

        // Atualizar LastUsedAtUtc e IsActive usando reflection
        var lastUsedProp = typeof(UserDevice).GetProperty("LastUsedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (lastUsedProp?.SetMethod != null && record.LastUsedAtUtc.HasValue)
        {
            lastUsedProp.SetValue(device, record.LastUsedAtUtc.Value);
        }

        if (!record.IsActive)
        {
            device.MarkAsInactive();
        }

        return device;
    }
}

public sealed class PostgresUserDeviceRepository : IUserDeviceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresUserDeviceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(UserDevice device, CancellationToken cancellationToken)
    {
        _dbContext.UserDevices.Add(new UserDeviceRecord
        {
            Id = device.Id,
            UserId = device.UserId,
            DeviceToken = device.DeviceToken,
            Platform = device.Platform,
            DeviceName = device.DeviceName,
            RegisteredAtUtc = device.RegisteredAtUtc,
            LastUsedAtUtc = device.LastUsedAtUtc,
            IsActive = device.IsActive
        });
        return Task.CompletedTask;
    }

    public async Task<UserDevice?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);

        return record is null ? null : UserDeviceMapperExtensions.ToDomain(record);
    }

    public async Task<IReadOnlyList<UserDevice>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.UserDevices
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.IsActive)
            .OrderByDescending(d => d.LastUsedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => UserDeviceMapperExtensions.ToDomain(r)).ToList();
    }

    public async Task<UserDevice?> GetByTokenAsync(string deviceToken, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken, cancellationToken);

        return record is null ? null : UserDeviceMapperExtensions.ToDomain(record);
    }

    public async Task UpdateAsync(UserDevice device, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .FirstOrDefaultAsync(d => d.Id == device.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.DeviceToken = device.DeviceToken;
        record.DeviceName = device.DeviceName;
        record.LastUsedAtUtc = device.LastUsedAtUtc;
        record.IsActive = device.IsActive;
    }

    public async Task DeleteAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserDevices
            .FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);

        if (record is not null)
        {
            _dbContext.UserDevices.Remove(record);
        }
    }
}
