using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Background;

/// <summary>
/// Background worker que processa payouts automaticamente baseado nas configurações de cada território.
/// </summary>
public sealed class PayoutProcessingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PayoutProcessingWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromMinutes(5); // Verifica a cada 5 minutos

    public PayoutProcessingWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<PayoutProcessingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PayoutProcessingWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPayoutsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payouts");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }

        _logger.LogInformation("PayoutProcessingWorker stopped.");
    }

    private async Task ProcessPayoutsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var payoutConfigRepository = scope.ServiceProvider.GetRequiredService<ITerritoryPayoutConfigRepository>();
        var territoryRepository = scope.ServiceProvider.GetRequiredService<ITerritoryRepository>();
        var payoutService = scope.ServiceProvider.GetRequiredService<SellerPayoutService>();

        // Buscar todos os territórios
        var territories = await territoryRepository.ListAsync(cancellationToken);

        foreach (var territory in territories)
        {
            try
            {
                var config = await payoutConfigRepository.GetActiveAsync(territory.Id, cancellationToken);
                if (config is null || !config.IsActive || !config.AutoPayoutEnabled)
                {
                    continue; // Território sem configuração ativa ou payout automático desabilitado
                }

                // Verificar se é hora de processar baseado na frequência
                if (!ShouldProcessNow(config))
                {
                    continue;
                }

                // Processar payouts para este território
                // Para processamento automático, usar Guid.Empty como userId (sistema)
                var result = await payoutService.ProcessPendingPayoutsAsync(
                    territory.Id,
                    Guid.Empty, // Sistema (não um usuário específico)
                    cancellationToken);

                if (result.IsSuccess && result.Value > 0)
                {
                    _logger.LogInformation(
                        "Processed {Count} payouts for territory {TerritoryId}",
                        result.Value,
                        territory.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error processing payouts for territory {TerritoryId}",
                    territory.Id);
            }
        }
    }

    /// <summary>
    /// Verifica se é hora de processar payouts baseado na frequência configurada.
    /// </summary>
    private bool ShouldProcessNow(TerritoryPayoutConfig config)
    {
        var now = DateTime.UtcNow;

        return config.Frequency switch
        {
            PayoutFrequency.Daily => true, // Processa sempre que o worker roda (já tem intervalo de 5 minutos)
            PayoutFrequency.Weekly => ShouldProcessWeekly(config, now),
            PayoutFrequency.Monthly => ShouldProcessMonthly(config, now),
            PayoutFrequency.Manual => false, // Manual nunca processa automaticamente
            _ => false
        };
    }

    private static bool ShouldProcessWeekly(TerritoryPayoutConfig config, DateTime now)
    {
        // Processa na segunda-feira de manhã (00:00 UTC)
        // Simplificação: processa toda segunda-feira
        return now.DayOfWeek == DayOfWeek.Monday && now.Hour == 0 && now.Minute < 10;
    }

    private static bool ShouldProcessMonthly(TerritoryPayoutConfig config, DateTime now)
    {
        // Processa no dia 1 de cada mês às 00:00 UTC
        return now.Day == 1 && now.Hour == 0 && now.Minute < 10;
    }
}
