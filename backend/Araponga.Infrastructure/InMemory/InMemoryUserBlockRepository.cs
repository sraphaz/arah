using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserBlockRepository : IUserBlockRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserBlockRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> ExistsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken)
    {
        var exists = _dataStore.UserBlocks.Any(block =>
            block.BlockerUserId == blockerUserId &&
            block.BlockedUserId == blockedUserId);

        return Task.FromResult(exists);
    }

    public Task AddAsync(UserBlock block, CancellationToken cancellationToken)
    {
        _dataStore.UserBlocks.Add(block);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken)
    {
        var block = _dataStore.UserBlocks.FirstOrDefault(b =>
            b.BlockerUserId == blockerUserId &&
            b.BlockedUserId == blockedUserId);

        if (block is not null)
        {
            _dataStore.UserBlocks.Remove(block);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Guid>> GetBlockedUserIdsAsync(
        Guid blockerUserId,
        CancellationToken cancellationToken)
    {
        var blocked = _dataStore.UserBlocks
            .Where(block => block.BlockerUserId == blockerUserId)
            .Select(block => block.BlockedUserId)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<Guid>>(blocked);
    }
}
