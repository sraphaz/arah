using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryVoteRepository : IVoteRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryVoteRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(Vote vote, CancellationToken cancellationToken)
    {
        _dataStore.Votes.Add(vote);
        return Task.CompletedTask;
    }

    public Task<bool> HasVotedAsync(Guid votingId, Guid userId, CancellationToken cancellationToken)
    {
        var hasVoted = _dataStore.Votes.Any(v => v.VotingId == votingId && v.UserId == userId);
        return Task.FromResult(hasVoted);
    }

    public Task<IReadOnlyList<Vote>> ListByVotingIdAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var votes = _dataStore.Votes
            .Where(v => v.VotingId == votingId)
            .OrderBy(v => v.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<Vote>>(votes);
    }

    public Task<Dictionary<string, int>> CountByOptionAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var counts = _dataStore.Votes
            .Where(v => v.VotingId == votingId)
            .GroupBy(v => v.SelectedOption)
            .ToDictionary(g => g.Key, g => g.Count());

        return Task.FromResult(counts);
    }

    public Task<IReadOnlyList<Vote>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var votes = _dataStore.Votes
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<Vote>>(votes);
    }
}
