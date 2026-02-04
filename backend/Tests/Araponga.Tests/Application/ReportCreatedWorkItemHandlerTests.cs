using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class ReportCreatedWorkItemHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesModerationCaseWorkItem()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton(dataStore);
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<ReportCreatedWorkItemHandler>();

        var sp = services.BuildServiceProvider();
        var handler = sp.GetRequiredService<ReportCreatedWorkItemHandler>();
        var workRepo = sp.GetRequiredService<IWorkItemRepository>();

        var reportId = Guid.NewGuid();
        var territoryId = dataStore.Territories[0].Id;
        var reporterId = dataStore.Users[0].Id;
        var evt = new ReportCreatedEvent(reportId, territoryId, reporterId, DateTime.UtcNow);

        await handler.HandleAsync(evt, CancellationToken.None);

        var items = await workRepo.ListAsync(WorkItemType.ModerationCase, WorkItemStatus.RequiresHumanReview, territoryId, CancellationToken.None);
        Assert.Contains(items, w => w.SubjectType == "REPORT" && w.SubjectId == reportId);
    }
}

