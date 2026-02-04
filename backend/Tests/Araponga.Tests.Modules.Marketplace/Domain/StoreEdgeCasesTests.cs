using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

/// <summary>
/// Edge case tests for the Store domain entity.
/// </summary>
public class StoreEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestOwnerUserId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var storeId = Guid.NewGuid();

        var store = new Store(
            storeId,
            TestTerritoryId,
            TestOwnerUserId,
            "Minha Loja",
            "Descrição da loja",
            StoreStatus.Active,
            true,
            StoreContactVisibility.Public,
            "+5511999999999",
            "+5511888888888",
            "loja@example.com",
            "@minhaloja",
            "https://minhaloja.com",
            "WhatsApp",
            TestDate,
            TestDate);

        Assert.Equal(storeId, store.Id);
        Assert.Equal("Minha Loja", store.DisplayName);
        Assert.Equal(StoreStatus.Active, store.Status);
        Assert.True(store.PaymentsEnabled);
    }

    [Fact]
    public void Constructor_WithEmptyTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Store(
                Guid.NewGuid(),
                Guid.Empty,
                TestOwnerUserId,
                "Loja",
                null,
                StoreStatus.Active,
                false,
                StoreContactVisibility.Public,
                null,
                null,
                null,
                null,
                null,
                null,
                TestDate,
                TestDate));
    }
}
