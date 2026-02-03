using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de ITerritoryCharacterizationRepository usando SharedDbContext.</summary>
public sealed class PostgresTerritoryCharacterizationRepository : ITerritoryCharacterizationRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresTerritoryCharacterizationRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task UpsertAsync(TerritoryCharacterization characterization, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryCharacterizations
            .FirstOrDefaultAsync(c => c.TerritoryId == characterization.TerritoryId, cancellationToken);
        var tagsJson = JsonSerializer.Serialize(characterization.Tags);
        if (record is null)
        {
            _dbContext.TerritoryCharacterizations.Add(characterization.ToRecord());
        }
        else
        {
            record.TagsJson = tagsJson;
            record.UpdatedAtUtc = characterization.UpdatedAtUtc;
        }
    }

    public async Task<TerritoryCharacterization?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryCharacterizations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId, cancellationToken);
        if (record is null) return null;
        return record.ToDomain();
    }
}
