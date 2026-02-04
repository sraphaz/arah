using Araponga.Application.Interfaces;
using Araponga.Domain.Connections;
using Araponga.Modules.Connections.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Connections.Infrastructure.Postgres;

public sealed class PostgresUserConnectionRepository : IUserConnectionRepository
{
    private readonly ConnectionsDbContext _db;

    public PostgresUserConnectionRepository(ConnectionsDbContext db)
    {
        _db = db;
    }

    public async Task<UserConnection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _db.UserConnections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return record is null ? null : MapToDomain(record);
    }

    public async Task<UserConnection?> GetByUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        var record = await _db.UserConnections
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                (c.RequesterUserId == userId1 && c.TargetUserId == userId2) ||
                (c.RequesterUserId == userId2 && c.TargetUserId == userId1),
                cancellationToken);
        return record is null ? null : MapToDomain(record);
    }

    public async Task<IReadOnlyList<UserConnection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _db.UserConnections
            .AsNoTracking()
            .Where(c => c.TargetUserId == userId && c.Status == (int)ConnectionStatus.Pending)
            .OrderByDescending(c => c.RequestedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<UserConnection>> GetAcceptedConnectionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _db.UserConnections
            .AsNoTracking()
            .Where(c =>
                (c.RequesterUserId == userId || c.TargetUserId == userId) &&
                c.Status == (int)ConnectionStatus.Accepted)
            .ToListAsync(cancellationToken);
        return records.Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<UserConnection>> GetConnectionsAsync(Guid userId, ConnectionStatus? status, CancellationToken cancellationToken)
    {
        var query = _db.UserConnections
            .AsNoTracking()
            .Where(c => c.RequesterUserId == userId || c.TargetUserId == userId);
        if (status.HasValue)
            query = query.Where(c => c.Status == (int)status.Value);
        var records = await query.OrderByDescending(c => c.UpdatedAtUtc).ToListAsync(cancellationToken);
        return records.Select(MapToDomain).ToList();
    }

    public async Task<bool> ExistsAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        return await _db.UserConnections
            .AsNoTracking()
            .AnyAsync(c =>
                (c.RequesterUserId == userId1 && c.TargetUserId == userId2) ||
                (c.RequesterUserId == userId2 && c.TargetUserId == userId1),
                cancellationToken);
    }

    public Task<UserConnection> AddAsync(UserConnection connection, CancellationToken cancellationToken)
    {
        var record = MapToRecord(connection);
        _db.UserConnections.Add(record);
        return Task.FromResult(connection);
    }

    public async Task UpdateAsync(UserConnection connection, CancellationToken cancellationToken)
    {
        var record = await _db.UserConnections.FirstOrDefaultAsync(c => c.Id == connection.Id, cancellationToken);
        if (record is null) return;
        record.Status = (int)connection.Status;
        record.RespondedAtUtc = connection.RespondedAtUtc;
        record.RemovedAtUtc = connection.RemovedAtUtc;
        record.UpdatedAtUtc = connection.UpdatedAtUtc;
    }

    public async Task<int> GetConnectionCountAsync(Guid userId, ConnectionStatus status, CancellationToken cancellationToken)
    {
        return await _db.UserConnections
            .AsNoTracking()
            .CountAsync(c =>
                (c.RequesterUserId == userId || c.TargetUserId == userId) &&
                c.Status == (int)status,
                cancellationToken);
    }

    private static UserConnection MapToDomain(UserConnectionRecord r)
    {
        return UserConnection.FromPersistence(
            r.Id,
            r.RequesterUserId,
            r.TargetUserId,
            (ConnectionStatus)r.Status,
            r.TerritoryId,
            r.RequestedAtUtc,
            r.RespondedAtUtc,
            r.RemovedAtUtc,
            r.CreatedAtUtc,
            r.UpdatedAtUtc);
    }

    private static UserConnectionRecord MapToRecord(UserConnection c)
    {
        return new UserConnectionRecord
        {
            Id = c.Id,
            RequesterUserId = c.RequesterUserId,
            TargetUserId = c.TargetUserId,
            Status = (int)c.Status,
            TerritoryId = c.TerritoryId,
            RequestedAtUtc = c.RequestedAtUtc,
            RespondedAtUtc = c.RespondedAtUtc,
            RemovedAtUtc = c.RemovedAtUtc,
            CreatedAtUtc = c.CreatedAtUtc,
            UpdatedAtUtc = c.UpdatedAtUtc
        };
    }
}
