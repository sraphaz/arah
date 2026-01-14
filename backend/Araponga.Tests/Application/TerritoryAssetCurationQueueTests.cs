using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Assets;
using Araponga.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class TerritoryAssetCurationQueueTests
{
    [Fact]
    public async Task CreateAsync_CreatesSuggestedAsset_AndEnqueuesAssetCurationWorkItem()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton(dataStore);
        services.AddScoped<ITerritoryAssetRepository, InMemoryAssetRepository>();
        services.AddScoped<IAssetGeoAnchorRepository, InMemoryAssetGeoAnchorRepository>();
        services.AddScoped<IAssetValidationRepository, InMemoryAssetValidationRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<TerritoryAssetService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<TerritoryAssetService>();
        var workRepo = sp.GetRequiredService<IWorkItemRepository>();

        var territoryId = dataStore.Territories[0].Id;
        var userId = dataStore.Users[0].Id;

        var create = await svc.CreateAsync(
            territoryId,
            userId,
            "natural",
            "Nascente X",
            "desc",
            new[] { new TerritoryAssetGeoAnchorInput(-23.37, -45.02) },
            CancellationToken.None);

        Assert.True(create.IsSuccess);
        Assert.NotNull(create.Value);
        Assert.Equal(AssetStatus.Suggested, create.Value!.Asset.Status);

        var queued = await workRepo.ListAsync(WorkItemType.AssetCuration, WorkItemStatus.RequiresHumanReview, territoryId, CancellationToken.None);
        Assert.NotEmpty(queued);
        Assert.Contains(queued, w => w.SubjectType == "ASSET" && w.SubjectId == create.Value.Asset.Id);
    }
}

