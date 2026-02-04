using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for Email Services (SmtpEmailSender and LoggingEmailSender),
/// focusing on Unicode, empty/null values, and error handling.
/// </summary>
public class EmailServiceEdgeCasesTests
{
    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await sender.SendEmailAsync(null!, CancellationToken.None);
        });
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithEmptyTo_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = "",
            Subject = "Test",
            Body = "Test body"
        };

        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email recipient (To) is required", result.Error ?? "");
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithNullTo_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = null!,
            Subject = "Test",
            Body = "Test body"
        };

        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email recipient (To) is required", result.Error ?? "");
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithEmptySubject_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = "recipient@example.com",
            Subject = "",
            Body = "Test body"
        };

        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email subject is required", result.Error ?? "");
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithEmptyBody_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = "recipient@example.com",
            Subject = "Test",
            Body = ""
        };

        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email body is required", result.Error ?? "");
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithUnicodeContent_HandlesCorrectly()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = "recipient@example.com",
            Subject = "Assunto com café, naïve, résumé, 文字 e emoji 🎉",
            Body = "Corpo com Unicode: café, naïve, résumé, 文字 e emoji 🎉",
            IsHtml = false
        };

        // Deve falhar porque a configuração SMTP não é válida (não conecta ao servidor real)
        // Mas não deve falhar por causa do Unicode
        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        // Pode ser sucesso ou falha dependendo da configuração, mas não deve lançar exceção por Unicode
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithInvalidConfig_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "", // Configuração inválida
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object);

        var message = new EmailMessage
        {
            To = "recipient@example.com",
            Subject = "Test",
            Body = "Test body"
        };

        var result = await sender.SendEmailAsync(message, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email configuration is invalid", result.Error ?? "");
    }

    [Fact]
    public async Task LoggingEmailSender_SendEmailAsync_WithUnicodeContent_HandlesCorrectly()
    {
        var logger = new Mock<ILogger<LoggingEmailSender>>();
        var sender = new LoggingEmailSender(logger.Object);

        var result = await sender.SendEmailAsync(
            "recipient@example.com",
            "Assunto com café, naïve, résumé, 文字 e emoji 🎉",
            "Corpo com Unicode: café, naïve, résumé, 文字 e emoji 🎉",
            false,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LoggingEmailSender_SendEmailAsync_WithNullBody_HandlesCorrectly()
    {
        var logger = new Mock<ILogger<LoggingEmailSender>>();
        var sender = new LoggingEmailSender(logger.Object);

        var result = await sender.SendEmailAsync(
            "recipient@example.com",
            "Test",
            null!,
            false,
            CancellationToken.None);

        // LoggingEmailSender sempre retorna sucesso (é apenas para logging)
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LoggingEmailSender_SendEmailAsync_WithEmptyTo_HandlesCorrectly()
    {
        var logger = new Mock<ILogger<LoggingEmailSender>>();
        var sender = new LoggingEmailSender(logger.Object);

        var result = await sender.SendEmailAsync(
            "",
            "Test",
            "Test body",
            false,
            CancellationToken.None);

        // LoggingEmailSender sempre retorna sucesso (é apenas para logging)
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SmtpEmailSender_SendEmailAsync_WithTemplateButNoTemplateService_ReturnsFailure()
    {
        var config = new EmailConfiguration
        {
            Host = "smtp.example.com",
            Port = 587,
            FromAddress = "test@example.com",
            FromName = "Test Sender"
        };
        var options = Options.Create(config);
        var logger = new Mock<ILogger<SmtpEmailSender>>();
        var sender = new SmtpEmailSender(options, logger.Object, null); // Sem template service

        var result = await sender.SendEmailAsync(
            "recipient@example.com",
            "Test",
            "template-name",
            new { },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("EmailTemplateService", result.Error ?? "");
    }
}
