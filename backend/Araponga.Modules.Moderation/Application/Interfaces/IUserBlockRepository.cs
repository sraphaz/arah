using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Modules.Moderation.Application.Interfaces;

public interface IUserBlockRepository
{
    Task<bool> ExistsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken);
    Task AddAsync(UserBlock block, CancellationToken cancellationToken);
    Task RemoveAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetBlockedUserIdsAsync(Guid blockerUserId, CancellationToken cancellationToken);
}
