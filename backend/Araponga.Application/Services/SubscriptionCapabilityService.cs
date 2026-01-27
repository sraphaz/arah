using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Services;

public sealed class SubscriptionCapabilityService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPlanRepository _planRepository;

    public SubscriptionCapabilityService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPlanRepository planRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
    }

    /// <summary>
    /// Verifica se o usuário tem uma capacidade específica.
    /// </summary>
    public async Task<bool> CheckCapabilityAsync(
        Guid userId,
        Guid? territoryId,
        FeatureCapability capability,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        
        // Se não tem assinatura, considera como FREE
        if (subscription == null)
        {
            // Buscar plano FREE padrão
            SubscriptionPlan? defaultPlan;
            if (territoryId.HasValue)
            {
                defaultPlan = await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
            }
            else
            {
                defaultPlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
            }

            if (defaultPlan == null)
            {
                return false; // Sem plano FREE, não tem acesso
            }

            return defaultPlan.Capabilities.Contains(capability);
        }

        // Buscar plano da assinatura
        var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
        if (plan == null)
        {
            return false;
        }

        // Verificar se plano está ativo
        if (!plan.IsActive)
        {
            return false;
        }

        // Verificar se assinatura está ativa
        if (subscription.Status != SubscriptionStatus.ACTIVE && subscription.Status != SubscriptionStatus.TRIALING)
        {
            return false;
        }

        return plan.Capabilities.Contains(capability);
    }

    /// <summary>
    /// Obtém todas as capacidades do usuário.
    /// </summary>
    public async Task<IReadOnlyList<FeatureCapability>> GetUserCapabilitiesAsync(
        Guid userId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        
        // Se não tem assinatura, considera como FREE
        if (subscription == null)
        {
            SubscriptionPlan? defaultPlan;
            if (territoryId.HasValue)
            {
                defaultPlan = await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
            }
            else
            {
                defaultPlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
            }

            return defaultPlan?.Capabilities ?? new List<FeatureCapability>();
        }

        // Buscar plano da assinatura
        var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            return new List<FeatureCapability>();
        }

        // Verificar se assinatura está ativa
        if (subscription.Status != SubscriptionStatus.ACTIVE && subscription.Status != SubscriptionStatus.TRIALING)
        {
            return new List<FeatureCapability>();
        }

        return plan.Capabilities;
    }

    /// <summary>
    /// Verifica se o usuário pode usar uma quantidade específica de um limite.
    /// </summary>
    public async Task<bool> CheckLimitAsync(
        Guid userId,
        Guid? territoryId,
        string limitType,
        int requestedAmount,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        
        // Se não tem assinatura, considera como FREE
        if (subscription == null)
        {
            SubscriptionPlan? defaultPlan;
            if (territoryId.HasValue)
            {
                defaultPlan = await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
            }
            else
            {
                defaultPlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
            }

            if (defaultPlan?.Limits == null || !defaultPlan.Limits.ContainsKey(limitType))
            {
                return false; // Sem limite definido, não permite
            }

            var limitValue = defaultPlan.Limits[limitType];
            if (limitValue is int intLimit)
            {
                return requestedAmount <= intLimit;
            }

            return false;
        }

        // Buscar plano da assinatura
        var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            return false;
        }

        // Verificar se assinatura está ativa
        if (subscription.Status != SubscriptionStatus.ACTIVE && subscription.Status != SubscriptionStatus.TRIALING)
        {
            return false;
        }

        if (plan.Limits == null || !plan.Limits.ContainsKey(limitType))
        {
            return false; // Sem limite definido, não permite
        }

        var limit = plan.Limits[limitType];
        if (limit is int intLimitValue)
        {
            return requestedAmount <= intLimitValue;
        }

        return false;
    }

    /// <summary>
    /// Obtém todos os limites do usuário.
    /// </summary>
    public async Task<Dictionary<string, object>> GetUserLimitsAsync(
        Guid userId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        
        // Se não tem assinatura, considera como FREE
        if (subscription == null)
        {
            SubscriptionPlan? defaultPlan;
            if (territoryId.HasValue)
            {
                defaultPlan = await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
            }
            else
            {
                defaultPlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
            }

            return defaultPlan?.Limits ?? new Dictionary<string, object>();
        }

        // Buscar plano da assinatura
        var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            return new Dictionary<string, object>();
        }

        // Verificar se assinatura está ativa
        if (subscription.Status != SubscriptionStatus.ACTIVE && subscription.Status != SubscriptionStatus.TRIALING)
        {
            return new Dictionary<string, object>();
        }

        return plan.Limits ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Resolve o plano do usuário (territorial ou global).
    /// </summary>
    public async Task<SubscriptionPlan?> ResolveUserPlanAsync(
        Guid userId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        
        if (subscription == null)
        {
            // Retorna plano FREE padrão
            if (territoryId.HasValue)
            {
                return await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
            }
            else
            {
                return await _planRepository.GetDefaultPlanAsync(cancellationToken);
            }
        }

        return await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
    }
}
