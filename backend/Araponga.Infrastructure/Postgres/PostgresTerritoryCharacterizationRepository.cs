using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryCharacterizationRepository : ITerritoryCharacterizationRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryCharacterizationRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(TerritoryCharacterization characterization, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryCharacterizations
            .FirstOrDefaultAsync(c => c.TerritoryId == characterization.TerritoryId, cancellationToken);

        var tagsJson = JsonSerializer.Serialize(characterization.Tags);

        if (record is null)
        {
            _dbContext.TerritoryCharacterizations.Add(new TerritoryCharacterizationRecord
            {
                TerritoryId = characterization.TerritoryId,
                TagsJson = tagsJson,
                UpdatedAtUtc = characterization.UpdatedAtUtc
            });
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

        if (record is null)
        {
            return null;
        }

        var tags = JsonSerializer.Deserialize<List<string>>(record.TagsJson) ?? new List<string>();
        return new TerritoryCharacterization(
            record.TerritoryId,
            tags,
            record.UpdatedAtUtc);
    }
}
