using Araponga.Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class EventReminderWorkerTests
{
    [Fact]
    public void EventReminderWorker_CanBeInstantiated()
    {
        // Arrange
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var logger = new Mock<ILogger<EventReminderWorker>>();

        // Act
        var worker = new EventReminderWorker(scopeFactory.Object, logger.Object);

        // Assert
        Assert.NotNull(worker);
    }

    [Fact]
    public void EventReminderWorker_ImplementsBackgroundService()
    {
        // Arrange
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var logger = new Mock<ILogger<EventReminderWorker>>();

        // Act
        var worker = new EventReminderWorker(scopeFactory.Object, logger.Object);

        // Assert
        Assert.IsAssignableFrom<Microsoft.Extensions.Hosting.BackgroundService>(worker);
    }
}
