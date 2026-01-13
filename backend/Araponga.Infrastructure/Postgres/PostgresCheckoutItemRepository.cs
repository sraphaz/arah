using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresCheckoutItemRepository : ICheckoutItemRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresCheckoutItemRepository(ArapongaDbContext dbContext)
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
