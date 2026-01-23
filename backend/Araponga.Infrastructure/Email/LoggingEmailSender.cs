using Araponga.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Email;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(
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

        return Task.CompletedTask;
    }
}
