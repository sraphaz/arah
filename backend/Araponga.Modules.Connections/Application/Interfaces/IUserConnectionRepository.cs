using Araponga.Domain.Connections;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório de conexões entre usuários.
/// </summary>
public interface IUserConnectionRepository
{
    Task<UserConnection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserConnection?> GetByUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetAcceptedConnectionsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetConnectionsAsync(Guid userId, ConnectionStatus? status, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<UserConnection> AddAsync(UserConnection connection, CancellationToken cancellationToken);
    Task UpdateAsync(UserConnection connection, CancellationToken cancellationToken);
    Task<int> GetConnectionCountAsync(Guid userId, ConnectionStatus status, CancellationToken cancellationToken);
}
