using Araponga.Application.Models;
using Araponga.Domain.Email;

namespace Araponga.Application.Services;

/// <summary>
/// Mapeia tipos de notificação para templates de email e determina quando enviar emails.
/// </summary>
public sealed class EmailNotificationMapper
{
    /// <summary>
    /// Determina se uma notificação deve gerar um email.
    /// </summary>
    public static bool ShouldSendEmail(string notificationKind)
    {
        return notificationKind switch
        {
            "post.created" => false, // Apenas in-app
            "comment.created" => false, // Apenas in-app
            "event.created" => true, // Email se evento importante
            "event.reminder" => true, // Lembrete de evento
            "marketplace.order.confirmed" => true, // Pedido confirmado
            "marketplace.inquiry" => false, // Apenas in-app
            "alert.critical" => true, // Alertas críticos sempre por email
            "alert.created" => false, // Alertas normais apenas in-app
            "report.created" => false, // Apenas in-app
            "membership.request" => false, // Apenas in-app
            _ => false // Por padrão, não enviar email
        };
    }

    /// <summary>
    /// Obtém o nome do template de email para um tipo de notificação.
    /// </summary>
    public static string? GetEmailTemplate(string notificationKind)
    {
        return notificationKind switch
        {
            "event.created" => "event-reminder",
            "event.reminder" => "event-reminder",
            "marketplace.order.confirmed" => "marketplace-order",
            "alert.critical" => "alert-critical",
            _ => null
        };
    }

    /// <summary>
    /// Obtém a prioridade do email para um tipo de notificação.
    /// </summary>
    public static EmailQueuePriority GetEmailPriority(string notificationKind)
    {
        return notificationKind switch
        {
            "alert.critical" => EmailQueuePriority.Critical,
            "marketplace.order.confirmed" => EmailQueuePriority.High,
            "event.reminder" => EmailQueuePriority.Normal,
            "event.created" => EmailQueuePriority.Normal,
            _ => EmailQueuePriority.Normal
        };
    }

    /// <summary>
    /// Cria dados de template para um tipo de notificação.
    /// </summary>
    public static object? CreateTemplateData(string notificationKind, NotificationDispatchPayload payload, string baseUrl)
    {
        if (payload.Data == null)
            return null;

        return notificationKind switch
        {
            "event.created" or "event.reminder" => new EventReminderEmailTemplateData
            {
                UserName = "Usuário", // Será substituído pelo nome real do usuário
                BaseUrl = baseUrl,
                EventName = payload.Title,
                EventDate = DateTime.TryParse(payload.Data.GetValueOrDefault("eventDate"), out var date) ? date : DateTime.UtcNow,
                Location = payload.Data.GetValueOrDefault("location"),
                EventLink = payload.Data.ContainsKey("eventId") ? $"{baseUrl}/events/{payload.Data["eventId"]}" : null
            },
            "marketplace.order.confirmed" => new MarketplaceOrderEmailTemplateData
            {
                UserName = "Usuário",
                BaseUrl = baseUrl,
                OrderId = Guid.TryParse(payload.Data.GetValueOrDefault("orderId"), out var orderId) ? orderId : Guid.Empty,
                Items = new List<OrderItem>(), // Será preenchido com dados reais
                Total = decimal.TryParse(payload.Data.GetValueOrDefault("total"), out var total) ? total : 0,
                SellerName = payload.Data.GetValueOrDefault("sellerName") ?? "Vendedor",
                OrderLink = payload.Data.ContainsKey("orderId") ? $"{baseUrl}/orders/{payload.Data["orderId"]}" : null
            },
            "alert.critical" => new CriticalAlertEmailTemplateData
            {
                UserName = "Usuário",
                BaseUrl = baseUrl,
                AlertType = payload.Data.GetValueOrDefault("alertType") ?? "Crítico",
                AlertTitle = payload.Title,
                AlertDescription = payload.Body ?? string.Empty,
                RecommendedActions = payload.Data.ContainsKey("recommendedActions")
                    ? payload.Data["recommendedActions"].Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : null,
                MoreInfoLink = payload.Data.ContainsKey("alertId") ? $"{baseUrl}/alerts/{payload.Data["alertId"]}" : null
            },
            _ => null
        };
    }
}
