using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByProviderAsync(string provider, string externalId, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListUserIdsByRoleAsync(UserRole role, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}
