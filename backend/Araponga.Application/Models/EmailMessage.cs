namespace Araponga.Application.Models;

/// <summary>
/// Representa uma mensagem de email.
/// </summary>
public sealed class EmailMessage
{
    /// <summary>
    /// Endereço de email do destinatário.
    /// </summary>
    public required string To { get; init; }

    /// <summary>
    /// Assunto do email.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// Corpo do email (opcional se TemplateName for fornecido).
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Indica se o corpo é HTML.
    /// </summary>
    public bool IsHtml { get; init; } = true;

    /// <summary>
    /// Endereço de email do remetente. Se não especificado, usa o padrão da configuração.
    /// </summary>
    public string? From { get; init; }

    /// <summary>
    /// Nome do remetente. Se não especificado, usa o padrão da configuração.
    /// </summary>
    public string? FromName { get; init; }

    /// <summary>
    /// Endereço de email para resposta.
    /// </summary>
    public string? ReplyTo { get; init; }

    /// <summary>
    /// Nome do template a ser usado (opcional).
    /// </summary>
    public string? TemplateName { get; init; }

    /// <summary>
    /// Dados para preencher o template (opcional).
    /// </summary>
    public object? TemplateData { get; init; }
}
