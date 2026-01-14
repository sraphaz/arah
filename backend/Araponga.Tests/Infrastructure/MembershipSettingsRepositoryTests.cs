using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class MembershipSettingsRepositoryTests
{
    [Fact]
    public async Task MembershipSettingsRepository_AddAndGetByMembershipId()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryMembershipSettingsRepository(dataStore);

        // Usar um MembershipId diferente para evitar conflito com dados pré-populados
        var membershipId = Guid.NewGuid();
        var settings = new MembershipSettings(
            membershipId,
            false,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await repository.AddAsync(settings, CancellationToken.None);

        var found = await repository.GetByMembershipIdAsync(membershipId, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(membershipId, found!.MembershipId);
        Assert.False(found.MarketplaceOptIn);
    }

    [Fact]
    public async Task MembershipSettingsRepository_Update_ModifiesExisting()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryMembershipSettingsRepository(dataStore);

        // Usar um MembershipId diferente para evitar conflito com dados pré-populados
        var membershipId = Guid.NewGuid();
        var settings = new MembershipSettings(
            membershipId,
            false,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await repository.AddAsync(settings, CancellationToken.None);

        settings.UpdateMarketplaceOptIn(true, DateTime.UtcNow);
        await repository.UpdateAsync(settings, CancellationToken.None);

        var found = await repository.GetByMembershipIdAsync(membershipId, CancellationToken.None);
        Assert.NotNull(found);
        Assert.True(found!.MarketplaceOptIn);
    }
}
