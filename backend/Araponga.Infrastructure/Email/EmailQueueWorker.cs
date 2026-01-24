using Araponga.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Email;

/// <summary>
/// Background worker que processa a fila de emails periodicamente.
/// </summary>
public sealed class EmailQueueWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailQueueWorker> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);
    private readonly int _batchSize = 10;
    private readonly int _maxEmailsPerMinute = 100;
    private DateTime _lastMinuteStart = DateTime.UtcNow;
    private int _emailsSentThisMinute = 0;

    public EmailQueueWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailQueueWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailQueueWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueueBatchAsync(stoppingToken);
                await Task.Delay(_processingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EmailQueueWorker main loop");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("EmailQueueWorker stopped");
    }

    private async Task ProcessQueueBatchAsync(CancellationToken cancellationToken)
    {
        // Reset contador a cada minuto
        if (DateTime.UtcNow - _lastMinuteStart >= TimeSpan.FromMinutes(1))
        {
            _lastMinuteStart = DateTime.UtcNow;
            _emailsSentThisMinute = 0;
        }

        // Rate limiting
        if (_emailsSentThisMinute >= _maxEmailsPerMinute)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<EmailQueueService>();

        var processed = await queueService.ProcessQueueAsync(_batchSize, cancellationToken);
        _emailsSentThisMinute += processed;

        if (processed > 0)
        {
            _logger.LogInformation("Processed {Count} emails from queue", processed);
        }
    }
}
