namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para renderização de templates de email.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Renderiza um template de email com os dados fornecidos.
    /// </summary>
    /// <param name="templateName">Nome do template (ex: "welcome", "password-reset").</param>
    /// <param name="data">Dados para preencher o template.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>HTML renderizado do template.</returns>
    Task<string> RenderTemplateAsync(
        string templateName,
        object data,
        CancellationToken cancellationToken);
}
