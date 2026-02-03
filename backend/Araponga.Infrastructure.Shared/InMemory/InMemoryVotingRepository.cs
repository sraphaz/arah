using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IVotingRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryVotingRepository : IVotingRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryVotingRepository(InMemorySharedStore store) => _store = store;

    public Task AddAsync(Voting voting, CancellationToken cancellationToken)
    {
        _store.Votings.Add(voting);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Voting voting, CancellationToken cancellationToken)
    {
        var i = _store.Votings.FindIndex(v => v.Id == voting.Id);
        if (i >= 0) _store.Votings[i] = voting;
        else _store.Votings.Add(voting);
        return Task.CompletedTask;
    }

    public Task<Voting?> GetByIdAsync(Guid votingId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Votings.FirstOrDefault(v => v.Id == votingId));

    public Task<IReadOnlyList<Voting>> ListByTerritoryAsync(Guid territoryId, VotingStatus? status, Guid? createdByUserId, CancellationToken cancellationToken)
    {
        var q = _store.Votings.Where(v => v.TerritoryId == territoryId);
        if (status.HasValue) q = q.Where(v => v.Status == status.Value);
        if (createdByUserId.HasValue) q = q.Where(v => v.CreatedByUserId == createdByUserId.Value);
        return Task.FromResult<IReadOnlyList<Voting>>(q.OrderByDescending(v => v.CreatedAtUtc).ToList());
    }
}
