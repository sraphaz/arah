using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySubscriptionPaymentRepository : ISubscriptionPaymentRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySubscriptionPaymentRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SubscriptionPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var payment = _dataStore.SubscriptionPayments.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(payment);
    }

    public Task<IReadOnlyList<SubscriptionPayment>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var payments = _dataStore.SubscriptionPayments
            .Where(p => p.SubscriptionId == subscriptionId)
            .OrderByDescending(p => p.PeriodStart)
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPayment>>(payments);
    }

    public Task<IReadOnlyList<SubscriptionPayment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Primeiro precisamos encontrar as subscriptions do usuário
        var subscriptionIds = _dataStore.Subscriptions
            .Where(s => s.UserId == userId)
            .Select(s => s.Id)
            .ToHashSet();

        var payments = _dataStore.SubscriptionPayments
            .Where(p => subscriptionIds.Contains(p.SubscriptionId))
            .OrderByDescending(p => p.PeriodStart)
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPayment>>(payments);
    }

    public Task<SubscriptionPayment?> GetByStripeInvoiceIdAsync(string stripeInvoiceId, CancellationToken cancellationToken)
    {
        var payment = _dataStore.SubscriptionPayments.FirstOrDefault(p => p.StripeInvoiceId == stripeInvoiceId);
        return Task.FromResult(payment);
    }

    public Task<SubscriptionPayment?> GetBySubscriptionAndPeriodAsync(Guid subscriptionId, DateTime periodStart, DateTime periodEnd, CancellationToken cancellationToken)
    {
        var payment = _dataStore.SubscriptionPayments.FirstOrDefault(p =>
            p.SubscriptionId == subscriptionId &&
            p.PeriodStart == periodStart &&
            p.PeriodEnd == periodEnd);
        return Task.FromResult(payment);
    }

    public Task AddAsync(SubscriptionPayment payment, CancellationToken cancellationToken)
    {
        _dataStore.SubscriptionPayments.Add(payment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SubscriptionPayment payment, CancellationToken cancellationToken)
    {
        // In-memory: a referência já está na lista, então não precisa fazer nada
        // Mas vamos garantir que existe
        var existing = _dataStore.SubscriptionPayments.FirstOrDefault(p => p.Id == payment.Id);
        if (existing is null)
        {
            _dataStore.SubscriptionPayments.Add(payment);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.SubscriptionPayments.Any(p => p.Id == id);
        return Task.FromResult(exists);
    }
}
