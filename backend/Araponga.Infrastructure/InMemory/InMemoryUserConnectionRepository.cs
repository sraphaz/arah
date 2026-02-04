using Araponga.Application.Interfaces;
using Araponga.Domain.Connections;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserConnectionRepository : IUserConnectionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserConnectionRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<UserConnection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var c = _dataStore.UserConnections.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(c);
    }

    public Task<UserConnection?> GetByUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        var c = _dataStore.UserConnections.FirstOrDefault(x =>
            (x.RequesterUserId == userId1 && x.TargetUserId == userId2) ||
            (x.RequesterUserId == userId2 && x.TargetUserId == userId1));
        return Task.FromResult(c);
    }

    public Task<IReadOnlyList<UserConnection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var list = _dataStore.UserConnections
            .Where(x => x.TargetUserId == userId && x.Status == ConnectionStatus.Pending)
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<UserConnection>>(list);
    }

    public Task<IReadOnlyList<UserConnection>> GetAcceptedConnectionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var list = _dataStore.UserConnections
            .Where(x => (x.RequesterUserId == userId || x.TargetUserId == userId) && x.Status == ConnectionStatus.Accepted)
            .ToList();
        return Task.FromResult<IReadOnlyList<UserConnection>>(list);
    }

    public Task<IReadOnlyList<UserConnection>> GetConnectionsAsync(Guid userId, ConnectionStatus? status, CancellationToken cancellationToken)
    {
        var query = _dataStore.UserConnections.Where(x => x.RequesterUserId == userId || x.TargetUserId == userId);
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        var list = query.OrderByDescending(x => x.UpdatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<UserConnection>>(list);
    }

    public Task<bool> ExistsAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        var exists = _dataStore.UserConnections.Any(x =>
            (x.RequesterUserId == userId1 && x.TargetUserId == userId2) ||
            (x.RequesterUserId == userId2 && x.TargetUserId == userId1));
        return Task.FromResult(exists);
    }

    public Task<UserConnection> AddAsync(UserConnection connection, CancellationToken cancellationToken)
    {
        _dataStore.UserConnections.Add(connection);
        return Task.FromResult(connection);
    }

    public Task UpdateAsync(UserConnection connection, CancellationToken cancellationToken)
    {
        var idx = _dataStore.UserConnections.FindIndex(x => x.Id == connection.Id);
        if (idx >= 0)
            _dataStore.UserConnections[idx] = connection;
        return Task.CompletedTask;
    }

    public Task<int> GetConnectionCountAsync(Guid userId, ConnectionStatus status, CancellationToken cancellationToken)
    {
        var count = _dataStore.UserConnections.Count(x =>
            (x.RequesterUserId == userId || x.TargetUserId == userId) && x.Status == status);
        return Task.FromResult(count);
    }
}
