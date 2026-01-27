using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryCouponRepository : ICouponRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryCouponRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var coupon = _dataStore.Coupons.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(coupon);
    }

    public Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var coupon = _dataStore.Coupons.FirstOrDefault(c =>
            string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(coupon);
    }

    public Task<IReadOnlyList<Coupon>> ListAsync(bool? isActive, CancellationToken cancellationToken)
    {
        var coupons = _dataStore.Coupons.AsEnumerable();

        if (isActive.HasValue)
        {
            coupons = coupons.Where(c => c.IsActive == isActive.Value);
        }

        return Task.FromResult<IReadOnlyList<Coupon>>(coupons.ToList());
    }

    public Task AddAsync(Coupon coupon, CancellationToken cancellationToken)
    {
        _dataStore.Coupons.Add(coupon);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Coupon coupon, CancellationToken cancellationToken)
    {
        // In-memory: a referência já está na lista, então não precisa fazer nada
        // Mas vamos garantir que existe
        var existing = _dataStore.Coupons.FirstOrDefault(c => c.Id == coupon.Id);
        if (existing is null)
        {
            _dataStore.Coupons.Add(coupon);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.Coupons.Any(c => c.Id == id);
        return Task.FromResult(exists);
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
    {
        var exists = _dataStore.Coupons.Any(c =>
            string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }
}
