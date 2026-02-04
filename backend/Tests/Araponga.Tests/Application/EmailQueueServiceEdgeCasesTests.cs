using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Email;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for EmailQueueService,
/// focusing on null/empty inputs, error handling, retry logic, and template processing.
/// </summary>
public sealed class EmailQueueServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryEmailQueueRepository _queueRepository;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailTemplateService> _templateServiceMock;
    private readonly Mock<ILogger<EmailQueueService>> _loggerMock;
    private readonly EmailQueueService _service;

    public EmailQueueServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _queueRepository = new InMemoryEmailQueueRepository(_dataStore);
        _emailSenderMock = new Mock<IEmailSender>();
        _templateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<EmailQueueService>>();
        _service = new EmailQueueService(
            _queueRepository,
            _emailSenderMock.Object,
            _loggerMock.Object,
            _templateServiceMock.Object);
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.EnqueueEmailAsync(null!, EmailQueuePriority.Normal, null, CancellationToken.None));
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithEmptyTo_ReturnsFailure()
    {
        var message = new EmailMessage
        {
            To = "",
            Subject = "Test",
            Body = "Body"
        };

        // EmailQueueItem constructor valida e lança ArgumentException, que é capturada e retornada como Failure
        var result = await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Failed to enqueue email", result.Error ?? "");
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithNullTo_ReturnsFailure()
    {
        var message = new EmailMessage
        {
            To = null!,
            Subject = "Test",
            Body = "Body"
        };

        // EmailQueueItem constructor will throw ArgumentException, que é capturada e retornada como Failure
        var result = await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Failed to enqueue email", result.Error ?? "");
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithUnicodeInSubject_HandlesCorrectly()
    {
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Assunto com acentuação: café, naïve, 文字",
            Body = "Body"
        };

        var result = await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var queuedItem = _dataStore.EmailQueueItems.First();
        Assert.Contains("café", queuedItem.Subject);
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithTemplateName_StoresTemplateData()
    {
        var templateData = new { Name = "Test", Value = 123 };
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            TemplateName = "welcome",
            TemplateData = templateData
        };

        var result = await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var queuedItem = _dataStore.EmailQueueItems.First();
        Assert.Equal("welcome", queuedItem.TemplateName);
        Assert.NotNull(queuedItem.TemplateDataJson);
        Assert.Contains("Test", queuedItem.TemplateDataJson);
    }

    [Fact]
    public async Task EnqueueEmailAsync_WithScheduledFor_StoresCorrectly()
    {
        var scheduledFor = DateTime.UtcNow.AddHours(1);
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };

        var result = await _service.EnqueueEmailAsync(message, EmailQueuePriority.High, scheduledFor, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var queuedItem = _dataStore.EmailQueueItems.First();
        Assert.Equal(scheduledFor, queuedItem.ScheduledFor);
        Assert.Equal(EmailQueuePriority.High, queuedItem.Priority);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithEmptyQueue_ReturnsZero()
    {
        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(0, processed);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithSuccessfulEmail_MarksAsCompleted()
    {
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(1, processed);
        var item = _dataStore.EmailQueueItems.First();
        Assert.Equal(EmailQueueStatus.Completed, item.Status);
        Assert.NotNull(item.ProcessedAtUtc);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithFailedEmail_MarksAsFailed()
    {
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Failure("SMTP error"));

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(0, processed); // Não processado com sucesso
        var item = _dataStore.EmailQueueItems.First();
        Assert.Equal(EmailQueueStatus.Pending, item.Status); // Ainda pendente para retry
        Assert.NotNull(item.ErrorMessage);
        Assert.NotNull(item.NextRetryAtUtc);
        Assert.Equal(1, item.Attempts);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithTemplateEmail_CallsEmailSenderWithTemplate()
    {
        var templateData = new { Name = "Test" };
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            TemplateName = "welcome",
            TemplateData = templateData
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(1, processed);
        // Verifica que o email sender foi chamado com template (não com body)
        _emailSenderMock.Verify(s => s.SendEmailAsync(
            "test@example.com",
            "Test",
            "welcome",
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithException_HandlesGracefully()
    {
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Connection failed"));

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(0, processed);
        var item = _dataStore.EmailQueueItems.First();
        Assert.Equal(EmailQueueStatus.Pending, item.Status);
        Assert.NotNull(item.ErrorMessage);
        Assert.Contains("Connection failed", item.ErrorMessage);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithCancellation_StopsProcessing()
    {
        var message1 = new EmailMessage { To = "test1@example.com", Subject = "Test1", Body = "Body1" };
        var message2 = new EmailMessage { To = "test2@example.com", Subject = "Test2", Body = "Body2" };
        await _service.EnqueueEmailAsync(message1, EmailQueuePriority.Normal, null, CancellationToken.None);
        await _service.EnqueueEmailAsync(message2, EmailQueuePriority.Normal, null, CancellationToken.None);

        var cts = new CancellationTokenSource();
        var callCount = 0;
        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    cts.Cancel(); // Cancela após processar o primeiro
                }
            })
            .ReturnsAsync(OperationResult.Success());

        var processed = await _service.ProcessQueueAsync(10, cts.Token);

        // Processa o primeiro antes de cancelar, então deve processar 1
        Assert.Equal(1, processed);
    }

    [Fact]
    public async Task ProcessQueueAsync_WithMaxRetries_MarksAsDeadLetter()
    {
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);

        var item = _dataStore.EmailQueueItems.First();
        // Simular 3 tentativas anteriores
        item.RestoreState(EmailQueueStatus.Pending, 3, "Previous error", DateTime.UtcNow.AddMinutes(-1), null, DateTime.UtcNow.AddHours(-1));

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Failure("Persistent error"));

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(0, processed);
        Assert.Equal(4, item.Attempts);
        Assert.Equal(EmailQueueStatus.Failed, item.Status); // Após 4 tentativas, marca como Failed
    }

    [Fact]
    public async Task ProcessQueueAsync_WithBatchSize_LimitsProcessing()
    {
        // Criar 15 emails
        for (int i = 0; i < 15; i++)
        {
            var message = new EmailMessage
            {
                To = $"test{i}@example.com",
                Subject = $"Test {i}",
                Body = "Body"
            };
            await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, null, CancellationToken.None);
        }

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(10, processed); // Apenas 10 processados
    }

    [Fact]
    public async Task ProcessQueueAsync_WithScheduledForFuture_SkipsItem()
    {
        var scheduledFor = DateTime.UtcNow.AddHours(1);
        var message = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };
        await _service.EnqueueEmailAsync(message, EmailQueuePriority.Normal, scheduledFor, CancellationToken.None);

        var processed = await _service.ProcessQueueAsync(10, CancellationToken.None);

        Assert.Equal(0, processed); // Não processa itens agendados para o futuro
    }
}
