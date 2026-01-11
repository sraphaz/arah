using Araponga.Domain.Moderation;

namespace Araponga.Application.Interfaces;

public interface IUserBlockRepository
{
    Task<bool> ExistsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken);
    Task AddAsync(UserBlock block, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetBlockedUserIdsAsync(Guid blockerUserId, CancellationToken cancellationToken);
}
