using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Services;

public sealed class SubscriptionPlanAdminService
{
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ISubscriptionPlanHistoryRepository _historyRepository;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionPlanAdminService(
        ISubscriptionPlanRepository planRepository,
        ISubscriptionPlanHistoryRepository historyRepository,
        ITerritoryRepository territoryRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _historyRepository = historyRepository;
        _territoryRepository = territoryRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria plano global (SystemAdmin).
    /// </summary>
    public async Task<Result<SubscriptionPlan>> CreateGlobalPlanAsync(
        Guid adminUserId,
        string name,
        string? description,
        SubscriptionPlanTier tier,
        decimal? pricePerCycle,
        SubscriptionBillingCycle? billingCycle,
        List<FeatureCapability> capabilities,
        Dictionary<string, object>? limits,
        int? trialDays,
        CancellationToken cancellationToken)
    {
        // Validar integridade
        var validationResult = await ValidatePlanIntegrityAsync(
            tier,
            PlanScope.Global,
            null,
            capabilities,
            pricePerCycle,
            cancellationToken);
        
        if (!validationResult.IsSuccess)
        {
            return Result<SubscriptionPlan>.Failure(validationResult.Error!);
        }

        var plan = new SubscriptionPlan(
            Guid.NewGuid(),
            name,
            description,
            tier,
            PlanScope.Global,
            null,
            pricePerCycle,
            billingCycle,
            capabilities,
            limits,
            tier == SubscriptionPlanTier.FREE,
            trialDays,
            adminUserId);

        await _planRepository.AddAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            adminUserId,
            SubscriptionPlanHistoryChangeType.Created);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Cria plano territorial (Curador).
    /// </summary>
    public async Task<Result<SubscriptionPlan>> CreateTerritoryPlanAsync(
        Guid territoryId,
        Guid curatorUserId,
        string name,
        string? description,
        SubscriptionPlanTier tier,
        decimal? pricePerCycle,
        SubscriptionBillingCycle? billingCycle,
        List<FeatureCapability> capabilities,
        Dictionary<string, object>? limits,
        int? trialDays,
        CancellationToken cancellationToken)
    {
        // Validar que território existe
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory == null)
        {
            return Result<SubscriptionPlan>.Failure($"Territory {territoryId} not found.");
        }

        // Validar integridade
        var validationResult = await ValidatePlanIntegrityAsync(
            tier,
            PlanScope.Territory,
            territoryId,
            capabilities,
            pricePerCycle,
            cancellationToken);
        
        if (!validationResult.IsSuccess)
        {
            return Result<SubscriptionPlan>.Failure(validationResult.Error!);
        }

        var plan = new SubscriptionPlan(
            Guid.NewGuid(),
            name,
            description,
            tier,
            PlanScope.Territory,
            territoryId,
            pricePerCycle,
            billingCycle,
            capabilities,
            limits,
            tier == SubscriptionPlanTier.FREE,
            trialDays,
            curatorUserId);

        await _planRepository.AddAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            curatorUserId,
            SubscriptionPlanHistoryChangeType.Created);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Atualiza plano.
    /// </summary>
    public async Task<Result<SubscriptionPlan>> UpdatePlanAsync(
        Guid planId,
        Guid userId,
        string? name,
        string? description,
        decimal? pricePerCycle,
        SubscriptionBillingCycle? billingCycle,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<SubscriptionPlan>.Failure($"Plan {planId} not found.");
        }

        // Validar preço se mudou
        if (pricePerCycle.HasValue && plan.PricePerCycle != pricePerCycle.Value)
        {
            var validationResult = await ValidatePlanIntegrityAsync(
                plan.Tier,
                plan.Scope,
                plan.TerritoryId,
                plan.Capabilities,
                pricePerCycle,
                cancellationToken);
            
            if (!validationResult.IsSuccess)
            {
                return Result<SubscriptionPlan>.Failure(validationResult.Error!);
            }
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            plan.UpdateName(name);
        }

        if (description != null)
        {
            plan.UpdateDescription(description);
        }

        if (pricePerCycle.HasValue || billingCycle.HasValue)
        {
            plan.UpdatePrice(pricePerCycle, billingCycle);
        }

        await _planRepository.UpdateAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            userId,
            SubscriptionPlanHistoryChangeType.Updated);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Atualiza capacidades do plano.
    /// </summary>
    public async Task<Result<SubscriptionPlan>> UpdatePlanCapabilitiesAsync(
        Guid planId,
        Guid userId,
        List<FeatureCapability> capabilities,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<SubscriptionPlan>.Failure($"Plan {planId} not found.");
        }

        // Validar integridade
        var validationResult = await ValidatePlanIntegrityAsync(
            plan.Tier,
            plan.Scope,
            plan.TerritoryId,
            capabilities,
            plan.PricePerCycle,
            cancellationToken);
        
        if (!validationResult.IsSuccess)
        {
            return Result<SubscriptionPlan>.Failure(validationResult.Error!);
        }

        plan.UpdateCapabilities(capabilities);
        await _planRepository.UpdateAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            userId,
            SubscriptionPlanHistoryChangeType.CapabilitiesChanged);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Ativa plano.
    /// </summary>
    public async Task<Result<SubscriptionPlan>> ActivatePlanAsync(
        Guid planId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<SubscriptionPlan>.Failure($"Plan {planId} not found.");
        }

        plan.Activate();
        await _planRepository.UpdateAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            userId,
            SubscriptionPlanHistoryChangeType.Activated);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Desativa plano.
    /// </summary>
    public async Task<Result<SubscriptionPlan>> DeactivatePlanAsync(
        Guid planId,
        Guid userId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<SubscriptionPlan>.Failure($"Plan {planId} not found.");
        }

        // Verificar se há assinaturas ativas para este plano
        var activeSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.ACTIVE,
            cancellationToken);
        
        var hasActiveSubscriptions = activeSubscriptions.Any(s => s.PlanId == planId);
        if (hasActiveSubscriptions)
        {
            return Result<SubscriptionPlan>.Failure("Cannot deactivate plan with active subscriptions.");
        }

        try
        {
            plan.Deactivate();
        }
        catch (InvalidOperationException ex)
        {
            return Result<SubscriptionPlan>.Failure(ex.Message);
        }

        await _planRepository.UpdateAsync(plan, cancellationToken);

        // Registrar histórico
        var history = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            plan.Id,
            userId,
            SubscriptionPlanHistoryChangeType.Deactivated,
            changeReason: reason);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<SubscriptionPlan>.Success(plan);
    }

    /// <summary>
    /// Obtém plano por ID.
    /// </summary>
    public async Task<SubscriptionPlan?> GetPlanByIdAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetByIdAsync(planId, cancellationToken);
    }

    /// <summary>
    /// Obtém planos globais.
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionPlan>> GetGlobalPlansAsync(
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetGlobalPlansAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém planos para território.
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionPlan>> GetPlansForTerritoryAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetPlansForTerritoryAsync(territoryId, cancellationToken);
    }

    /// <summary>
    /// Obtém histórico de mudanças do plano.
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionPlanHistory>> GetPlanHistoryAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return await _historyRepository.GetByPlanIdAsync(planId, cancellationToken);
    }

    /// <summary>
    /// Valida integridade do plano (garante funcionalidades básicas no FREE).
    /// </summary>
    private Task<OperationResult> ValidatePlanIntegrityAsync(
        SubscriptionPlanTier tier,
        PlanScope scope,
        Guid? territoryId,
        List<FeatureCapability> capabilities,
        decimal? pricePerCycle,
        CancellationToken cancellationToken)
    {
        // Validações para FREE
        if (tier == SubscriptionPlanTier.FREE)
        {
            // FREE deve ter preço = 0
            if (pricePerCycle.HasValue && pricePerCycle.Value != 0)
            {
                return Task.FromResult(OperationResult.Failure("FREE plan must have price = 0."));
            }

            // FREE deve ter todas as funcionalidades básicas
            var basicCapabilities = new[]
            {
                FeatureCapability.FeedBasic,
                FeatureCapability.PostsBasic,
                FeatureCapability.EventsBasic,
                FeatureCapability.MarketplaceBasic,
                FeatureCapability.ChatBasic
            };

            foreach (var basicCap in basicCapabilities)
            {
                if (!capabilities.Contains(basicCap))
                {
                    return Task.FromResult(OperationResult.Failure($"FREE plan must include {basicCap} capability."));
                }
            }

            // FREE global não pode ser desativado (mas isso é validado no método Deactivate)
        }

        // Validações para planos pagos
        if (tier != SubscriptionPlanTier.FREE)
        {
            if (!pricePerCycle.HasValue || pricePerCycle.Value <= 0)
            {
                return Task.FromResult(OperationResult.Failure("Paid plans must have price > 0."));
            }
        }

        return Task.FromResult(OperationResult.Success());
    }
}
