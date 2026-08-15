using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Domain.Financial;
using Arah.Domain.Subscriptions;
using Arah.Modules.Marketplace.Application.Interfaces;

namespace Arah.Application.Services;

/// <summary>
/// Alias merchant → subscription + leitura de consumo (FASE55 v0).
/// </summary>
public sealed class MerchantCommercialService
{
    private readonly IStoreRepository _stores;
    private readonly SubscriptionService _subscriptions;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IConsumptionMeterRepository _meters;

    public MerchantCommercialService(
        IStoreRepository stores,
        SubscriptionService subscriptions,
        ISubscriptionRepository subscriptionRepository,
        IConsumptionMeterRepository meters)
    {
        _stores = stores;
        _subscriptions = subscriptions;
        _subscriptionRepository = subscriptionRepository;
        _meters = meters;
    }

    public async Task<Result<Subscription>> CreateSubscriptionForMerchantAsync(
        Guid merchantId,
        Guid requestingUserId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken)
    {
        var store = await _stores.GetByIdAsync(merchantId, cancellationToken);
        if (store is null)
        {
            return Result<Subscription>.Failure("Merchant not found.");
        }

        if (store.OwnerUserId != requestingUserId)
        {
            return Result<Subscription>.Failure("Forbidden.");
        }

        return await _subscriptions.CreateSubscriptionAsync(
            requestingUserId,
            store.TerritoryId,
            planId,
            couponCode,
            cancellationToken);
    }

    public async Task<Result<IReadOnlyList<ConsumptionMeter>>> GetConsumptionAsync(
        Guid merchantId,
        Guid requestingUserId,
        CancellationToken cancellationToken)
    {
        var store = await _stores.GetByIdAsync(merchantId, cancellationToken);
        if (store is null)
        {
            return Result<IReadOnlyList<ConsumptionMeter>>.Failure("Merchant not found.");
        }

        if (store.OwnerUserId != requestingUserId)
        {
            return Result<IReadOnlyList<ConsumptionMeter>>.Failure("Forbidden.");
        }

        var subscription = await _subscriptionRepository.GetByUserIdAsync(
            store.OwnerUserId,
            store.TerritoryId,
            cancellationToken);

        if (subscription is null)
        {
            subscription = await _subscriptions.GetOrCreateUserSubscriptionAsync(
                store.OwnerUserId,
                store.TerritoryId,
                cancellationToken);
        }

        var existing = await _meters.ListBySubscriptionAsync(subscription.Id, cancellationToken);
        if (existing.Count > 0)
        {
            return Result<IReadOnlyList<ConsumptionMeter>>.Success(existing);
        }

        var defaults = ConsumptionMeter.ZeroedDefaults(subscription.Id);
        await _meters.AddRangeAsync(defaults, cancellationToken);
        return Result<IReadOnlyList<ConsumptionMeter>>.Success(defaults);
    }
}
