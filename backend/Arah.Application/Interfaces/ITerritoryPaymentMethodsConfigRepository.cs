using Arah.Domain.Fiscal;

namespace Arah.Application.Interfaces;

public interface ITerritoryPaymentMethodsConfigRepository
{
    Task<TerritoryPaymentMethodsConfig?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken);
}
