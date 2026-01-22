namespace Araponga.Application.Models;

/// <summary>
/// Dados base para templates de email.
/// </summary>
public abstract class EmailTemplateData
{
    /// <summary>
    /// Nome do usuário destinatário.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// URL base da plataforma.
    /// </summary>
    public required string BaseUrl { get; init; }
}

/// <summary>
/// Dados para template de email de boas-vindas.
/// </summary>
public sealed class WelcomeEmailTemplateData : EmailTemplateData
{
    /// <summary>
    /// Link para ativação da conta (opcional).
    /// </summary>
    public string? ActivationLink { get; init; }
}

/// <summary>
/// Dados para template de email de recuperação de senha.
/// </summary>
public sealed class PasswordResetEmailTemplateData : EmailTemplateData
{
    /// <summary>
    /// Link para reset de senha.
    /// </summary>
    public required string ResetLink { get; init; }

    /// <summary>
    /// Tempo de expiração do link (em minutos).
    /// </summary>
    public required int ExpirationMinutes { get; init; }
}

/// <summary>
/// Dados para template de email de lembrete de evento.
/// </summary>
public sealed class EventReminderEmailTemplateData : EmailTemplateData
{
    /// <summary>
    /// Nome do evento.
    /// </summary>
    public required string EventName { get; init; }

    /// <summary>
    /// Data e hora do evento.
    /// </summary>
    public required DateTime EventDate { get; init; }

    /// <summary>
    /// Localização do evento.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Link para o evento.
    /// </summary>
    public string? EventLink { get; init; }
}

/// <summary>
/// Dados para template de email de pedido confirmado no marketplace.
/// </summary>
public sealed class MarketplaceOrderEmailTemplateData : EmailTemplateData
{
    /// <summary>
    /// ID do pedido.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// Itens do pedido.
    /// </summary>
    public required IReadOnlyList<OrderItem> Items { get; init; }

    /// <summary>
    /// Total do pedido.
    /// </summary>
    public required decimal Total { get; init; }

    /// <summary>
    /// Nome do vendedor.
    /// </summary>
    public required string SellerName { get; init; }

    /// <summary>
    /// Link para o pedido.
    /// </summary>
    public string? OrderLink { get; init; }
}

/// <summary>
/// Item de pedido para template de email.
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// Nome do item.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public required int Quantity { get; init; }

    /// <summary>
    /// Preço unitário.
    /// </summary>
    public required decimal UnitPrice { get; init; }

    /// <summary>
    /// Preço total do item.
    /// </summary>
    public decimal TotalPrice => Quantity * UnitPrice;
}

/// <summary>
/// Dados para template de email de alerta crítico.
/// </summary>
public sealed class CriticalAlertEmailTemplateData : EmailTemplateData
{
    /// <summary>
    /// Tipo de alerta.
    /// </summary>
    public required string AlertType { get; init; }

    /// <summary>
    /// Título do alerta.
    /// </summary>
    public required string AlertTitle { get; init; }

    /// <summary>
    /// Descrição do alerta.
    /// </summary>
    public required string AlertDescription { get; init; }

    /// <summary>
    /// Ações recomendadas.
    /// </summary>
    public IReadOnlyList<string>? RecommendedActions { get; init; }

    /// <summary>
    /// Link para mais informações.
    /// </summary>
    public string? MoreInfoLink { get; init; }
}
