using Araponga.Domain.Configuration;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class SystemConfigRepositoryTests
{
    [Fact]
    public async Task SystemConfigRepository_UpsertAndGetByKey()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemorySystemConfigRepository(sharedStore);

        var actor = Guid.NewGuid();
        var cfg = new SystemConfig(
            Guid.NewGuid(),
            "moderation.reports.threshold",
            "3",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            actor,
            null,
            null);

        await repo.UpsertAsync(cfg, CancellationToken.None);

        var found = await repo.GetByKeyAsync("MODERATION.REPORTS.THRESHOLD", CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal("moderation.reports.threshold", found!.Key);
        Assert.Equal("3", found.Value);
    }

    [Fact]
    public async Task SystemConfigRepository_List_ByCategory()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemorySystemConfigRepository(sharedStore);
        var actor = Guid.NewGuid();

        await repo.UpsertAsync(new SystemConfig(
            Guid.NewGuid(),
            "moderation.reports.threshold",
            "3",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            actor,
            null,
            null), CancellationToken.None);

        await repo.UpsertAsync(new SystemConfig(
            Guid.NewGuid(),
            "providers.email.enabled",
            "false",
            SystemConfigCategory.Providers,
            null,
            DateTime.UtcNow,
            actor,
            null,
            null), CancellationToken.None);

        var moderation = await repo.ListAsync(SystemConfigCategory.Moderation, CancellationToken.None);
        Assert.Single(moderation);
        Assert.Equal(SystemConfigCategory.Moderation, moderation[0].Category);
    }
}

