using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresCheckoutRepository : ICheckoutRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresCheckoutRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        _dbContext.Checkouts.Add(checkout.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        _dbContext.Checkouts.Update(checkout.ToRecord());
        return Task.CompletedTask;
    }
}
