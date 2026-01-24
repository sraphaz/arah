using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Email;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task<OperationResult> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Email enviado (simulado). To={To} Subject={Subject} IsHtml={IsHtml} BodyLength={BodyLength}",
            to,
            subject,
            isHtml,
            body?.Length ?? 0);

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> SendEmailAsync(
        string to,
        string subject,
        string templateName,
        object templateData,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Email enviado (simulado) com template. To={To} Subject={Subject} Template={Template}",
            to,
            subject,
            templateName);

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Email enviado (simulado) via EmailMessage. To={To} Subject={Subject} IsHtml={IsHtml} BodyLength={BodyLength}",
            message.To,
            message.Subject,
            message.IsHtml,
            message.Body?.Length ?? 0);

        return Task.FromResult(OperationResult.Success());
    }
}
