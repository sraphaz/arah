using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IVoteRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryVoteRepository : IVoteRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryVoteRepository(InMemorySharedStore store) => _store = store;

    public Task AddAsync(Vote vote, CancellationToken cancellationToken)
    {
        _store.Votes.Add(vote);
        return Task.CompletedTask;
    }

    public Task<bool> HasVotedAsync(Guid votingId, Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Votes.Any(v => v.VotingId == votingId && v.UserId == userId));

    public Task<IReadOnlyList<Vote>> ListByVotingIdAsync(Guid votingId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Vote>>(_store.Votes.Where(v => v.VotingId == votingId).OrderBy(v => v.CreatedAtUtc).ToList());

    public Task<Dictionary<string, int>> CountByOptionAsync(Guid votingId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Votes.Where(v => v.VotingId == votingId).GroupBy(v => v.SelectedOption).ToDictionary(g => g.Key, g => g.Count()));

    public Task<IReadOnlyList<Vote>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Vote>>(_store.Votes.Where(v => v.UserId == userId).OrderByDescending(v => v.CreatedAtUtc).ToList());
}
