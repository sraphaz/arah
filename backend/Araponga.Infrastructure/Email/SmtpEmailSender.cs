using System.Net;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Araponga.Infrastructure.Email;

/// <summary>
/// Implementação de IEmailSender usando SMTP via MailKit.
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailConfiguration _config;
    private readonly ILogger<SmtpEmailSender> _logger;
    private readonly IEmailTemplateService? _templateService;

    public SmtpEmailSender(
        IOptions<EmailConfiguration> config,
        ILogger<SmtpEmailSender> logger,
        IEmailTemplateService? templateService = null)
    {
        _config = config.Value;
        _logger = logger;
        _templateService = templateService;

        if (!_config.IsValid())
        {
            _logger.LogWarning(
                "Email configuration is invalid. Email sending may fail. Host: {Host}, Port: {Port}, FromAddress: {FromAddress}",
                _config.Host,
                _config.Port,
                _config.FromAddress);
        }
    }

    public async Task<OperationResult> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<OperationResult> SendEmailAsync(
        string to,
        string subject,
        string templateName,
        object templateData,
        CancellationToken cancellationToken)
    {
        if (_templateService == null)
        {
            _logger.LogError(
                "EmailTemplateService is not available. Template: {TemplateName}, To: {To}",
                templateName,
                to);
            return OperationResult.Failure(
                "Template-based email sending requires EmailTemplateService to be registered.");
        }

        try
        {
            var body = await _templateService.RenderTemplateAsync(templateName, templateData, cancellationToken);
            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return await SendEmailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to render email template. Template: {TemplateName}, To: {To}",
                templateName,
                to);
            return OperationResult.Failure($"Failed to render email template: {ex.Message}");
        }
    }

    public async Task<OperationResult> SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(message.To))
        {
            return OperationResult.Failure("Email recipient (To) is required.");
        }

        if (string.IsNullOrWhiteSpace(message.Subject))
        {
            return OperationResult.Failure("Email subject is required.");
        }

        if (string.IsNullOrWhiteSpace(message.Body))
        {
            return OperationResult.Failure("Email body is required.");
        }

        if (!_config.IsValid())
        {
            _logger.LogError(
                "Cannot send email: Email configuration is invalid. Host: {Host}, Port: {Port}",
                _config.Host,
                _config.Port);
            return OperationResult.Failure("Email configuration is invalid.");
        }

        try
        {
            var mimeMessage = CreateMimeMessage(message);
            await SendMimeMessageAsync(mimeMessage, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully. To: {To}, Subject: {Subject}",
                message.To,
                message.Subject);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email. To: {To}, Subject: {Subject}",
                message.To,
                message.Subject);

            return OperationResult.Failure($"Failed to send email: {ex.Message}");
        }
    }

    private MimeMessage CreateMimeMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();

        // From
        var fromAddress = message.From ?? _config.FromAddress;
        var fromName = message.FromName ?? _config.FromName;
        mimeMessage.From.Add(new MailboxAddress(fromName, fromAddress));

        // To
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));

        // Reply-To (se especificado)
        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
        {
            mimeMessage.ReplyTo.Add(MailboxAddress.Parse(message.ReplyTo));
        }

        // Subject
        mimeMessage.Subject = message.Subject;

        // Body
        var bodyBuilder = new BodyBuilder();
        if (message.IsHtml)
        {
            bodyBuilder.HtmlBody = message.Body;
        }
        else
        {
            bodyBuilder.TextBody = message.Body;
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        return mimeMessage;
    }

    private async Task SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_config.Host, _config.Port, _config.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

            // Autenticação (se credenciais fornecidas)
            if (!string.IsNullOrWhiteSpace(_config.Username) && !string.IsNullOrWhiteSpace(_config.Password))
            {
                await client.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}
