using System.Text.Json;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Email;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar a fila de envio de emails.
/// </summary>
public sealed class EmailQueueService
{
    private readonly IEmailQueueRepository _queueRepository;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService? _templateService;
    private readonly ILogger<EmailQueueService> _logger;

    public EmailQueueService(
        IEmailQueueRepository queueRepository,
        IEmailSender emailSender,
        ILogger<EmailQueueService> logger,
        IEmailTemplateService? templateService = null)
    {
        _queueRepository = queueRepository;
        _emailSender = emailSender;
        _logger = logger;
        _templateService = templateService;
    }

    /// <summary>
    /// Adiciona um email à fila para envio assíncrono.
    /// </summary>
    public async Task<OperationResult> EnqueueEmailAsync(
        EmailMessage message,
        EmailQueuePriority priority = EmailQueuePriority.Normal,
        DateTime? scheduledFor = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            var queueItem = new EmailQueueItem(
                Guid.NewGuid(),
                message.To,
                message.Subject,
                message.Body ?? string.Empty, // Body é opcional quando TemplateName é fornecido
                message.IsHtml,
                message.TemplateName,
                message.TemplateData != null ? JsonSerializer.Serialize(message.TemplateData) : null,
                priority,
                scheduledFor);

            await _queueRepository.AddAsync(queueItem, cancellationToken);

            _logger.LogInformation(
                "Email queued. Id: {EmailId}, To: {To}, Priority: {Priority}",
                queueItem.Id,
                message.To,
                priority);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue email. To: {To}", message.To);
            return OperationResult.Failure($"Failed to enqueue email: {ex.Message}");
        }
    }

    /// <summary>
    /// Processa itens pendentes da fila.
    /// </summary>
    public async Task<int> ProcessQueueAsync(int batchSize = 10, CancellationToken cancellationToken = default)
    {
        var items = await _queueRepository.GetPendingItemsAsync(batchSize, cancellationToken);
        var processed = 0;

        foreach (var item in items)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                item.MarkAsProcessing();
                await _queueRepository.UpdateAsync(item, cancellationToken);

                var result = await SendQueuedEmailAsync(item, cancellationToken);

                if (result.IsSuccess)
                {
                    item.MarkAsCompleted();
                    processed++;
                }
                else
                {
                    var nextRetry = CalculateNextRetry(item.Attempts);
                    item.MarkAsFailed(result.Error ?? "Unknown error", nextRetry);
                }

                await _queueRepository.UpdateAsync(item, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email queue item. Id: {EmailId}", item.Id);
                var nextRetry = CalculateNextRetry(item.Attempts);
                item.MarkAsFailed(ex.Message, nextRetry);
                await _queueRepository.UpdateAsync(item, cancellationToken);
            }
        }

        return processed;
    }

    private async Task<OperationResult> SendQueuedEmailAsync(
        EmailQueueItem item,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(item.TemplateName) && item.TemplateDataJson != null && _templateService != null)
            {
                var templateData = JsonSerializer.Deserialize<object>(item.TemplateDataJson);
                if (templateData != null)
                {
                    return await _emailSender.SendEmailAsync(
                        item.To,
                        item.Subject,
                        item.TemplateName,
                        templateData,
                        cancellationToken);
                }
            }

            return await _emailSender.SendEmailAsync(
                item.To,
                item.Subject,
                item.Body,
                item.IsHtml,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send queued email. Id: {EmailId}", item.Id);
            return OperationResult.Failure(ex.Message);
        }
    }

    private static DateTime? CalculateNextRetry(int attempts)
    {
        return attempts switch
        {
            0 => DateTime.UtcNow.AddMinutes(5),
            1 => DateTime.UtcNow.AddMinutes(15),
            2 => DateTime.UtcNow.AddHours(1),
            _ => null // Dead letter após 3 tentativas
        };
    }
}
