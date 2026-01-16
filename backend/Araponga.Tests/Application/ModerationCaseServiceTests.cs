using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Moderation;
using Araponga.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class ModerationCaseServiceTests
{
    [Fact]
    public async Task DecideAsync_Approved_Post_HidesPost_AndMarksReportActioned()
    {
        var ds = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton(ds);
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<IReportRepository, InMemoryReportRepository>();
        services.AddScoped<IFeedRepository, InMemoryFeedRepository>();
        services.AddScoped<ISanctionRepository, InMemorySanctionRepository>();
        services.AddScoped<Araponga.Application.Interfaces.Media.IMediaAttachmentRepository, InMemoryMediaAttachmentRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<ModerationCaseService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<ModerationCaseService>();
        var reportRepo = sp.GetRequiredService<IReportRepository>();
        var feedRepo = sp.GetRequiredService<IFeedRepository>();
        var workRepo = sp.GetRequiredService<IWorkItemRepository>();

        var post = ds.Posts[0];
        var territoryId = post.TerritoryId;

        var report = new ModerationReport(
            Guid.NewGuid(),
            ds.Users[0].Id,
            territoryId,
            ReportTargetType.Post,
            post.Id,
            "spam",
            null,
            ReportStatus.Open,
            DateTime.UtcNow);
        await reportRepo.AddAsync(report, CancellationToken.None);

        var work = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.ModerationCase,
            WorkItemStatus.RequiresHumanReview,
            territoryId,
            ds.Users[0].Id,
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            subjectType: "REPORT",
            subjectId: report.Id,
            payloadJson: null,
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);
        await workRepo.AddAsync(work, CancellationToken.None);

        var actor = Guid.NewGuid();
        var decided = await svc.DecideAsync(work.Id, actor, WorkItemOutcome.Approved, "ok", CancellationToken.None);
        Assert.True(decided.IsSuccess);

        var updatedPost = await feedRepo.GetPostAsync(post.Id, CancellationToken.None);
        Assert.NotNull(updatedPost);
        Assert.Equal(PostStatus.Hidden, updatedPost!.Status);

        var updatedReport = await reportRepo.GetByIdAsync(report.Id, CancellationToken.None);
        Assert.NotNull(updatedReport);
        Assert.Equal(ReportStatus.Actioned, updatedReport!.Status);
    }
}

