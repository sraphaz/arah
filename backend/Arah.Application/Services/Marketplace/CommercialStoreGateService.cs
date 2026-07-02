using Arah.Application.Interfaces;
using Arah.Domain.Subscriptions;

namespace Arah.Application.Services;

/// <summary>
/// Gate FASE55 — comércio só vende com plano comercial (MarketplaceAdvanced).
/// </summary>
public sealed class CommercialStoreGateService
{
    private readonly SubscriptionCapabilityService _capabilities;

    public CommercialStoreGateService(SubscriptionCapabilityService capabilities)
    {
        _capabilities = capabilities;
    }

    public async Task<bool> CanEnableCommerceAsync(
        Guid storeOwnerUserId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        return await _capabilities.CheckCapabilityAsync(
            storeOwnerUserId,
            territoryId,
            FeatureCapability.MarketplaceAdvanced,
            cancellationToken);
    }

    public async Task<string?> GetCommerceBlockReasonAsync(
        Guid storeOwnerUserId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        if (await CanEnableCommerceAsync(storeOwnerUserId, territoryId, cancellationToken))
        {
            return null;
        }

        return "Active commercial subscription (MarketplaceAdvanced) is required to sell with payments.";
    }
}
