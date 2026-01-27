using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresSubscriptionPlanRepository : ISubscriptionPlanRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresSubscriptionPlanRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SubscriptionPlan>> GetGlobalPlansAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Global)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<SubscriptionPlan>> GetTerritoryPlansAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Territory && p.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<SubscriptionPlan>> GetPlansForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        // Retorna planos territoriais primeiro, depois globais
        var territoryRecords = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Territory && p.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);

        var globalRecords = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Global)
            .ToListAsync(cancellationToken);

        var allRecords = territoryRecords.Concat(globalRecords).ToList();
        return allRecords.Select(r => r.ToDomain()).ToList();
    }

    public async Task<SubscriptionPlan?> GetDefaultPlanAsync(CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Global && p.IsDefault)
            .FirstOrDefaultAsync(cancellationToken);
        return record?.ToDomain();
    }

    public async Task<SubscriptionPlan?> GetDefaultPlanForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        // Primeiro tenta encontrar plano territorial default
        var territoryRecord = await _dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.Scope == (int)PlanScope.Territory && p.TerritoryId == territoryId && p.IsDefault)
            .FirstOrDefaultAsync(cancellationToken);

        if (territoryRecord != null)
        {
            return territoryRecord.ToDomain();
        }

        // Se não encontrar, retorna o global default
        return await GetDefaultPlanAsync(cancellationToken);
    }

    public Task AddAsync(SubscriptionPlan plan, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionPlans.Add(plan.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(SubscriptionPlan plan, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == plan.Id, cancellationToken);

        if (record == null)
        {
            throw new InvalidOperationException($"Subscription plan {plan.Id} not found.");
        }

        // Atualizar propriedades
        record.Name = plan.Name;
        record.Description = plan.Description;
        record.Tier = (int)plan.Tier;
        record.Scope = (int)plan.Scope;
        record.TerritoryId = plan.TerritoryId;
        record.PricePerCycle = plan.PricePerCycle;
        record.BillingCycle = plan.BillingCycle.HasValue ? (int)plan.BillingCycle.Value : null;
        record.CapabilitiesJson = System.Text.Json.JsonSerializer.Serialize(plan.Capabilities);
        record.LimitsJson = plan.Limits != null ? System.Text.Json.JsonSerializer.Serialize(plan.Limits) : null;
        record.IsDefault = plan.IsDefault;
        record.TrialDays = plan.TrialDays;
        record.IsActive = plan.IsActive;
        record.StripePriceId = plan.StripePriceId;
        record.StripeProductId = plan.StripeProductId;
        record.UpdatedAtUtc = plan.UpdatedAtUtc;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.SubscriptionPlans
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}
