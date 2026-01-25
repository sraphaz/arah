using Araponga.Domain.Email;
using Xunit;

namespace Araponga.Tests.Domain.Email;

public sealed class EmailQueueItemEdgeCasesTests
{
    [Fact]
    public void EmailQueueItem_WithEmptyId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.Empty,
                "test@example.com",
                "Subject",
                "Body"));

        Assert.Contains("ID", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithNullTo_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                null!,
                "Subject",
                "Body"));

        Assert.Contains("to", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithEmptyTo_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                "",
                "Subject",
                "Body"));

        Assert.Contains("to", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithWhitespaceTo_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                "   ",
                "Subject",
                "Body"));

        Assert.Contains("to", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithNullSubject_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                "test@example.com",
                null!,
                "Body"));

        Assert.Contains("subject", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithEmptySubject_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                "test@example.com",
                "",
                "Body"));

        Assert.Contains("subject", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithEmptyBodyAndNoTemplate_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new EmailQueueItem(
                Guid.NewGuid(),
                "test@example.com",
                "Subject",
                ""));

        Assert.Contains("Body or TemplateName", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmailQueueItem_WithEmptyBodyButWithTemplate_DoesNotThrow()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "",
            templateName: "welcome");

        Assert.NotNull(item);
        Assert.Equal("welcome", item.TemplateName);
    }

    [Fact]
    public void EmailQueueItem_MarkAsFailed_IncrementsAttempts()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        Assert.Equal(0, item.Attempts);
        item.MarkAsFailed("Error message");
        Assert.Equal(1, item.Attempts);
        Assert.Equal("Error message", item.ErrorMessage);
    }

    [Fact]
    public void EmailQueueItem_MarkAsFailed_After4Attempts_SetsStatusToFailed()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsFailed("Error 1");
        item.MarkAsFailed("Error 2");
        item.MarkAsFailed("Error 3");
        item.MarkAsFailed("Error 4");

        Assert.Equal(EmailQueueStatus.Failed, item.Status);
        Assert.Equal(4, item.Attempts);
    }

    [Fact]
    public void EmailQueueItem_MarkAsFailed_Before4Attempts_KeepsStatusPending()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsFailed("Error 1");
        item.MarkAsFailed("Error 2");

        Assert.Equal(EmailQueueStatus.Pending, item.Status);
        Assert.Equal(2, item.Attempts);
    }

    [Fact]
    public void EmailQueueItem_ShouldRetry_WhenPendingAndAttemptsLessThan4_ReturnsTrue()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsFailed("Error");
        Assert.True(item.ShouldRetry());
    }

    [Fact]
    public void EmailQueueItem_ShouldRetry_WhenFailed_ReturnsFalse()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsFailed("Error 1");
        item.MarkAsFailed("Error 2");
        item.MarkAsFailed("Error 3");
        item.MarkAsFailed("Error 4");

        Assert.False(item.ShouldRetry());
    }

    [Fact]
    public void EmailQueueItem_ShouldRetry_WhenNextRetryAtInFuture_ReturnsFalse()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsFailed("Error", DateTime.UtcNow.AddHours(1));
        Assert.False(item.ShouldRetry());
    }

    [Fact]
    public void EmailQueueItem_MarkAsCompleted_SetsStatusAndProcessedAt()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsCompleted();

        Assert.Equal(EmailQueueStatus.Completed, item.Status);
        Assert.NotNull(item.ProcessedAtUtc);
    }

    [Fact]
    public void EmailQueueItem_MarkAsDeadLetter_SetsStatusAndProcessedAt()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Body");

        item.MarkAsDeadLetter();

        Assert.Equal(EmailQueueStatus.DeadLetter, item.Status);
        Assert.NotNull(item.ProcessedAtUtc);
    }

    [Fact]
    public void EmailQueueItem_WithUnicodeInSubject_AcceptsUnicode()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Assunto com acentuação: ção",
            "Body");

        Assert.Equal("Assunto com acentuação: ção", item.Subject);
    }

    [Fact]
    public void EmailQueueItem_WithUnicodeInBody_AcceptsUnicode()
    {
        var item = new EmailQueueItem(
            Guid.NewGuid(),
            "test@example.com",
            "Subject",
            "Corpo com acentuação: ção");

        Assert.Equal("Corpo com acentuação: ção", item.Body);
    }
}
