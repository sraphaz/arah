using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresCheckoutItemRepository : ICheckoutItemRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresCheckoutItemRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddRangeAsync(IReadOnlyList<CheckoutItem> items, CancellationToken cancellationToken)
    {
        var records = items.Select(item => item.ToRecord()).ToList();
        _dbContext.CheckoutItems.AddRange(records);
        return Task.CompletedTask;
    }
}
