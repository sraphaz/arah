using Araponga.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Background;

/// <summary>
/// Background worker que processa renovações automáticas de assinaturas.
/// </summary>
public sealed class SubscriptionRenewalWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubscriptionRenewalWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromHours(6); // Verifica a cada 6 horas

    public SubscriptionRenewalWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SubscriptionRenewalWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubscriptionRenewalWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRenewalsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing subscription renewals");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }

        _logger.LogInformation("SubscriptionRenewalWorker stopped.");
    }

    private async Task ProcessRenewalsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var renewalService = scope.ServiceProvider.GetRequiredService<SubscriptionRenewalService>();

        await renewalService.ProcessRenewalsAsync(cancellationToken);
    }
}
