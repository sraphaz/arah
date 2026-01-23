using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryVotingRepository : IVotingRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryVotingRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(Voting voting, CancellationToken cancellationToken)
    {
        _dataStore.Votings.Add(voting);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Voting voting, CancellationToken cancellationToken)
    {
        var index = _dataStore.Votings.FindIndex(v => v.Id == voting.Id);
        if (index >= 0)
        {
            _dataStore.Votings[index] = voting;
        }
        else
        {
            _dataStore.Votings.Add(voting);
        }

        return Task.CompletedTask;
    }

    public Task<Voting?> GetByIdAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var voting = _dataStore.Votings.FirstOrDefault(v => v.Id == votingId);
        return Task.FromResult(voting);
    }

    public Task<IReadOnlyList<Voting>> ListByTerritoryAsync(
        Guid territoryId,
        VotingStatus? status,
        Guid? createdByUserId,
        CancellationToken cancellationToken)
    {
        var votings = _dataStore.Votings
            .Where(v => v.TerritoryId == territoryId);

        if (status.HasValue)
        {
            votings = votings.Where(v => v.Status == status.Value);
        }

        if (createdByUserId.HasValue)
        {
            votings = votings.Where(v => v.CreatedByUserId == createdByUserId.Value);
        }

        var result = votings
            .OrderByDescending(v => v.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<Voting>>(result);
    }
}
