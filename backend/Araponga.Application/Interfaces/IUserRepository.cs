using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByProviderAsync(string provider, string externalId, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListUserIdsByRoleAsync(UserRole role, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets multiple users by their IDs in a single batch operation.
    /// </summary>
    Task<IReadOnlyList<User>> ListByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken);
}
