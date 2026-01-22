using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserDeviceRepository : IUserDeviceRepository
{
    private readonly List<UserDevice> _devices = new();

    public Task AddAsync(UserDevice device, CancellationToken cancellationToken)
    {
        _devices.Add(device);
        return Task.CompletedTask;
    }

    public Task<UserDevice?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        return Task.FromResult<UserDevice?>(device);
    }

    public Task<IReadOnlyList<UserDevice>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var devices = _devices
            .Where(d => d.UserId == userId && d.IsActive)
            .OrderByDescending(d => d.LastUsedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<UserDevice>>(devices);
    }

    public Task<UserDevice?> GetByTokenAsync(string deviceToken, CancellationToken cancellationToken)
    {
        var device = _devices.FirstOrDefault(d => d.DeviceToken == deviceToken);
        return Task.FromResult<UserDevice?>(device);
    }

    public Task UpdateAsync(UserDevice device, CancellationToken cancellationToken)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == device.Id);
        if (existing is not null)
        {
            _devices.Remove(existing);
            _devices.Add(device);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (device is not null)
        {
            _devices.Remove(device);
        }
        return Task.CompletedTask;
    }
}
