using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken);
    Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken);
}
