using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ICouponRepository
{
    Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<Coupon>> ListAsync(bool? isActive, CancellationToken cancellationToken);
    Task AddAsync(Coupon coupon, CancellationToken cancellationToken);
    Task UpdateAsync(Coupon coupon, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
}
