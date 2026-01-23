namespace Araponga.Application.Interfaces;

<<<<<<< HEAD
public interface IEmailSender
{
    Task SendEmailAsync(
=======
/// <summary>
/// Interface para envio de emails.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envia um email com conteúdo direto.
    /// </summary>
    /// <param name="to">Endereço de email do destinatário.</param>
    /// <param name="subject">Assunto do email.</param>
    /// <param name="body">Corpo do email.</param>
    /// <param name="isHtml">Indica se o corpo é HTML.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Common.OperationResult> SendEmailAsync(
>>>>>>> origin/feature/fase14-governanca-votacao
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken);
<<<<<<< HEAD
=======

    /// <summary>
    /// Envia um email usando um template.
    /// </summary>
    /// <param name="to">Endereço de email do destinatário.</param>
    /// <param name="subject">Assunto do email.</param>
    /// <param name="templateName">Nome do template a ser usado.</param>
    /// <param name="templateData">Dados para preencher o template.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Common.OperationResult> SendEmailAsync(
        string to,
        string subject,
        string templateName,
        object templateData,
        CancellationToken cancellationToken);

    /// <summary>
    /// Envia um email usando um objeto EmailMessage.
    /// </summary>
    /// <param name="message">Mensagem de email a ser enviada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Common.OperationResult> SendEmailAsync(
        Models.EmailMessage message,
        CancellationToken cancellationToken);
>>>>>>> origin/feature/fase14-governanca-votacao
}
