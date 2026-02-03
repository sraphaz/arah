using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para criar o plano FREE padrão no sistema.
/// </summary>
public sealed class SubscriptionPlanSeedService
{
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionPlanSeedService>? _logger;

    public SubscriptionPlanSeedService(
        ISubscriptionPlanRepository planRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubscriptionPlanSeedService>? logger = null)
    {
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Cria o plano FREE padrão global se não existir.
    /// </summary>
    public async Task SeedDefaultFreePlanAsync(CancellationToken cancellationToken)
    {
        // Verificar se já existe plano FREE global
        var existingFreePlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
        if (existingFreePlan != null)
        {
            _logger?.LogInformation("Default FREE plan already exists: {PlanId}", existingFreePlan.Id);
            return;
        }

        // Criar plano FREE padrão
        var freePlan = new SubscriptionPlan(
            Guid.NewGuid(),
            "FREE",
            "Plano gratuito com funcionalidades básicas sempre disponíveis",
            SubscriptionPlanTier.FREE,
            PlanScope.Global,
            null, // TerritoryId = null para plano global
            0m, // Preço zero
            null, // BillingCycle = null para FREE
            new List<FeatureCapability>
            {
                FeatureCapability.FeedBasic,
                FeatureCapability.PostsBasic,
                FeatureCapability.EventsBasic,
                FeatureCapability.MarketplaceBasic,
                FeatureCapability.ChatBasic
            },
            new Dictionary<string, object>
            {
                ["maxPosts"] = 10,
                ["maxEvents"] = 3,
                ["maxMarketplaceItems"] = 5,
                ["maxStorageMB"] = 100
            },
            isDefault: true,
            trialDays: null,
            createdByUserId: Guid.Empty, // Sistema
            stripePriceId: null,
            stripeProductId: null);

        await _planRepository.AddAsync(freePlan, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger?.LogInformation("Default FREE plan created successfully: {PlanId}", freePlan.Id);
    }
}
