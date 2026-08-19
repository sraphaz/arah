using Arah.Application.Interfaces;
using Arah.Domain.Fiscal;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed class PostgresTerritoryFiscalPackBindingRepository : ITerritoryFiscalPackBindingRepository
{
    private readonly ArahDbContext _dbContext;

    public PostgresTerritoryFiscalPackBindingRepository(ArahDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryFiscalPackBinding?> GetByTerritoryIdAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryFiscalPackBindings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.TerritoryId == territoryId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task AddAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.TerritoryFiscalPackBindings
            .AnyAsync(b => b.TerritoryId == binding.TerritoryId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException(
                $"Fiscal pack binding already exists for territory {binding.TerritoryId}.");
        }

        _dbContext.TerritoryFiscalPackBindings.Add(binding.ToRecord());
    }

    public async Task UpdateAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryFiscalPackBindings
            .FirstOrDefaultAsync(b => b.Id == binding.Id, cancellationToken)
            ?? throw new InvalidOperationException($"TerritoryFiscalPackBinding {binding.Id} not found.");

        record.PackId = binding.PackId;
        record.Status = (int)binding.Status;
        record.ActivatedByUserId = binding.ActivatedByUserId;
        record.ActivatedAtUtc = binding.ActivatedAtUtc;
        record.MunicipalityIbge = binding.MunicipalityIbge;
        record.UpdatedAtUtc = binding.UpdatedAtUtc;
    }
}
