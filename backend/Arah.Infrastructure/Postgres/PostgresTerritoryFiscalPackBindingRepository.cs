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

    public Task AddAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryFiscalPackBindings.Add(binding.ToRecord());
        return Task.CompletedTask;
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
