using Arah.Application.Interfaces;
using Arah.Application.Services;
using Arah.Domain.Financial;
using Arah.Domain.Territories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Arah.Infrastructure.Hosting;

/// <summary>
/// Seed de FeeSplitRule padrão por território ativo (FASE55 v0).
/// </summary>
public sealed class FeeSplitRuleBootstrapHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<FeeSplitRuleBootstrapHostedService> _logger;

    public FeeSplitRuleBootstrapHostedService(
        IServiceProvider services,
        ILogger<FeeSplitRuleBootstrapHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var territories = scope.ServiceProvider.GetRequiredService<ITerritoryRepository>();
        var rules = scope.ServiceProvider.GetRequiredService<IFeeSplitRuleRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var active = await territories.ListByStatusAsync(TerritoryStatus.Active, cancellationToken);
        var seeded = 0;

        foreach (var territory in active)
        {
            var existing = await rules.GetActiveAsync(
                territory.Id,
                TransactionQuoteService.DefaultRevenueType,
                DateTimeOffset.UtcNow,
                cancellationToken);

            if (existing is not null)
            {
                continue;
            }

            await rules.AddAsync(
                new FeeSplitRule(
                    Guid.NewGuid(),
                    territory.Id,
                    TransactionQuoteService.DefaultRevenueType,
                    implementerSharePercent: 40m,
                    territoryFundSharePercent: 30m,
                    platformSharePercent: 30m,
                    effectiveFromUtc: DateTimeOffset.UtcNow.AddYears(-1)),
                cancellationToken);
            seeded++;
        }

        if (seeded > 0)
        {
            await unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Seeded {Count} default FeeSplitRule(s)", seeded);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
