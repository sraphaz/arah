using Araponga.Application.Events;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for ReportService (ReportPost empty reason, post not found).
/// </summary>
public sealed class ReportServiceEdgeCasesTests
{
    [Fact]
    public async Task ReportPostAsync_WhenReasonEmpty_ReturnsError()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var reportRepo = new InMemoryReportRepository(ds);
        var feedRepo = new InMemoryFeedRepository(ds);
        var userRepo = new InMemoryUserRepository(sharedStore);
        var sanctionRepo = new InMemorySanctionRepository(ds);
        var mediaRepo = new InMemoryMediaAttachmentRepository(ds);
        var audit = new InMemoryAuditLogger(ds);
        var eventBus = new NoOpEventBus();
        var uow = new InMemoryUnitOfWork();
        var svc = new ReportService(
            reportRepo,
            feedRepo,
            userRepo,
            sanctionRepo,
            mediaRepo,
            audit,
            eventBus,
            uow,
            null);
        var reporterId = Guid.NewGuid();
        var postId = ds.Posts[0].Id;

        var (created, error, _) = await svc.ReportPostAsync(reporterId, postId, "", null, CancellationToken.None);

        Assert.False(created);
        Assert.NotNull(error);
        Assert.Contains("Reason", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReportPostAsync_WhenPostNotFound_ReturnsError()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var reportRepo = new InMemoryReportRepository(ds);
        var feedRepo = new InMemoryFeedRepository(ds);
        var userRepo = new InMemoryUserRepository(sharedStore);
        var sanctionRepo = new InMemorySanctionRepository(ds);
        var mediaRepo = new InMemoryMediaAttachmentRepository(ds);
        var audit = new InMemoryAuditLogger(ds);
        var eventBus = new NoOpEventBus();
        var uow = new InMemoryUnitOfWork();
        var svc = new ReportService(
            reportRepo,
            feedRepo,
            userRepo,
            sanctionRepo,
            mediaRepo,
            audit,
            eventBus,
            uow,
            null);
        var reporterId = Guid.NewGuid();

        var (created, error, _) = await svc.ReportPostAsync(reporterId, Guid.NewGuid(), "Spam", null, CancellationToken.None);

        Assert.False(created);
        Assert.NotNull(error);
        Assert.Contains("not found", error, StringComparison.OrdinalIgnoreCase);
    }
}
