using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IUserDeviceRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryUserDeviceRepository : IUserDeviceRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryUserDeviceRepository(InMemorySharedStore store) => _store = store;

    public Task AddAsync(UserDevice device, CancellationToken cancellationToken)
    {
        _store.UserDevices.Add(device);
        return Task.CompletedTask;
    }

    public Task<UserDevice?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken)
        => Task.FromResult(_store.UserDevices.FirstOrDefault(d => d.Id == deviceId));

    public Task<IReadOnlyList<UserDevice>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<UserDevice>>(_store.UserDevices.Where(d => d.UserId == userId && d.IsActive).OrderByDescending(d => d.LastUsedAtUtc).ToList());

    public Task<UserDevice?> GetByTokenAsync(string deviceToken, CancellationToken cancellationToken)
        => Task.FromResult(_store.UserDevices.FirstOrDefault(d => d.DeviceToken == deviceToken));

    public Task UpdateAsync(UserDevice device, CancellationToken cancellationToken)
    {
        var existing = _store.UserDevices.FirstOrDefault(d => d.Id == device.Id);
        if (existing is not null)
        {
            _store.UserDevices.Remove(existing);
            _store.UserDevices.Add(device);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var d = _store.UserDevices.FirstOrDefault(x => x.Id == deviceId);
        if (d is not null) _store.UserDevices.Remove(d);
        return Task.CompletedTask;
    }
}
