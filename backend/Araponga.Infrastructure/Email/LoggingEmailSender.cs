using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Email;

/// <summary>
/// Logging email sender for development/testing.
/// All sensitive data (email addresses) is intentionally excluded from logs.
/// Security fixes applied on 2026-01-24.
/// </summary>
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
            "Email enviado (simulado). Subject={Subject} IsHtml={IsHtml} BodyLength={BodyLength}",
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
            "Email enviado (simulado) com template. Subject={Subject} Template={Template}",
            subject,
            templateName);

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Email enviado (simulado) via EmailMessage. Subject={Subject} IsHtml={IsHtml} BodyLength={BodyLength}",
            message.Subject,
            message.IsHtml,
            message.Body?.Length ?? 0);

        return Task.FromResult(OperationResult.Success());
    }
}
