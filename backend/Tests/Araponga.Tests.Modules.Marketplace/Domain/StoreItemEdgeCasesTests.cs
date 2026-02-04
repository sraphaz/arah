using Araponga.Domain.Geo;
using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

/// <summary>
/// Edge case tests for the StoreItem domain entity, focusing on item creation,
/// pricing validation, coordinates, status, and types.
/// </summary>
public class StoreItemEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestStoreId = Guid.NewGuid();

    // Constructor validation tests
    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var itemId = Guid.NewGuid();

        var item = new StoreItem(
            itemId,
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Produto Teste",
            "Descrição do produto",
            "Categoria",
            "tag1, tag2",
            ItemPricingType.Fixed,
            100.50m,
            "BRL",
            "unidade",
            -23.5505,
            -46.6333,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(itemId, item.Id);
        Assert.Equal("Produto Teste", item.Title);
        Assert.Equal(ItemType.Product, item.Type);
        Assert.Equal(100.50m, item.PriceAmount);
    }

    [Fact]
    public void Constructor_WithNullTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItem(
                Guid.NewGuid(),
                Guid.Empty,
                TestStoreId,
                ItemType.Product,
                "Item",
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithNullStoreId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                Guid.Empty,
                ItemType.Product,
                "Item",
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithNullTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                TestStoreId,
                ItemType.Product,
                null!,
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                TestStoreId,
                ItemType.Product,
                "   ",
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithWhitespaceTitle_TrimsSuccessfully()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "  Produto Teste  ",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal("Produto Teste", item.Title);
    }

    [Fact]
    public void Constructor_WithUnicodeTitle_TrimsAndStores()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "  Café Orgânico 🍃  ",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal("Café Orgânico 🍃", item.Title);
    }

    // ItemType tests
    [Fact]
    public void Constructor_WithAllItemTypes_CreatesSuccessfully()
    {
        var types = new[] { ItemType.Product, ItemType.Service };

        foreach (var type in types)
        {
            var item = new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                TestStoreId,
                type,
                "Item",
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate);

            Assert.Equal(type, item.Type);
        }
    }

    // ItemStatus tests
    [Fact]
    public void Constructor_WithAllItemStatuses_CreatesSuccessfully()
    {
        var statuses = new[] { ItemStatus.Active, ItemStatus.OutOfStock, ItemStatus.Archived };

        foreach (var status in statuses)
        {
            var item = new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                TestStoreId,
                ItemType.Product,
                "Item",
                null,
                null,
                null,
                ItemPricingType.Fixed,
                null,
                null,
                null,
                null,
                null,
                status,
                TestDate,
                TestDate);

            Assert.Equal(status, item.Status);
        }
    }

    // ItemPricingType tests
    [Fact]
    public void Constructor_WithAllPricingTypes_CreatesSuccessfully()
    {
        var pricingTypes = new[]
        {
            ItemPricingType.Fixed,
            ItemPricingType.Estimate,
            ItemPricingType.Hourly,
            ItemPricingType.Negotiable
        };

        foreach (var pricingType in pricingTypes)
        {
            var item = new StoreItem(
                Guid.NewGuid(),
                TestTerritoryId,
                TestStoreId,
                ItemType.Product,
                "Item",
                null,
                null,
                null,
                pricingType,
                pricingType == ItemPricingType.Fixed ? 100m : null,
                pricingType == ItemPricingType.Fixed ? "BRL" : null,
                null,
                null,
                null,
                ItemStatus.Active,
                TestDate,
                TestDate);

            Assert.Equal(pricingType, item.PricingType);
        }
    }

    // Price validation tests
    [Fact]
    public void Constructor_WithFixedPricingAndPrice_CreatesSuccessfully()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            99.99m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(99.99m, item.PriceAmount);
        Assert.Equal("BRL", item.Currency);
        Assert.Equal("unidade", item.Unit);
    }

    [Fact]
    public void Constructor_WithNegotiablePricingAndNullPrice_AllowsNull()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Negotiable,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Null(item.PriceAmount);
        Assert.Null(item.Currency);
    }

    [Fact]
    public void Constructor_WithZeroPrice_AllowsZero()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item Grátis",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            0m,
            "BRL",
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(0m, item.PriceAmount);
    }

    [Fact]
    public void Constructor_WithNegativePrice_AllowsNegative()
    {
        // Note: Domain doesn't prevent negative prices, business logic should handle this
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            -10m,
            "BRL",
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(-10m, item.PriceAmount);
    }

    [Fact]
    public void Constructor_WithVeryLargePrice_StoresSuccessfully()
    {
        var largePrice = decimal.MaxValue;
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item Caro",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            largePrice,
            "BRL",
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(largePrice, item.PriceAmount);
    }

    // Coordinate tests
    [Fact]
    public void Constructor_WithValidCoordinates_StoresCorrectly()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            -23.5505,
            -46.6333,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(-23.5505, item.Latitude);
        Assert.Equal(-46.6333, item.Longitude);
    }

    [Fact]
    public void Constructor_WithNullCoordinates_AllowsNull()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Null(item.Latitude);
        Assert.Null(item.Longitude);
    }

    [Fact]
    public void Constructor_WithBoundaryLatitude_StoresCorrectly()
    {
        // Note: Domain doesn't validate coordinates, but we test boundary values
        var itemMin = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            -90.0,
            0.0,
            ItemStatus.Active,
            TestDate,
            TestDate);

        var itemMax = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            90.0,
            0.0,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(-90.0, itemMin.Latitude);
        Assert.Equal(90.0, itemMax.Latitude);
    }

    [Fact]
    public void Constructor_WithBoundaryLongitude_StoresCorrectly()
    {
        var itemMin = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            0.0,
            -180.0,
            ItemStatus.Active,
            TestDate,
            TestDate);

        var itemMax = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            0.0,
            180.0,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(-180.0, itemMin.Longitude);
        Assert.Equal(180.0, itemMax.Longitude);
    }

    // UpdateDetails tests
    [Fact]
    public void UpdateDetails_WithValidData_UpdatesSuccessfully()
    {
        var item = CreateActiveItem();
        var newDate = TestDate.AddHours(1);

        item.UpdateDetails(
            ItemType.Service,
            "Novo Título",
            "Nova descrição",
            "Nova categoria",
            "tag1, tag2",
            ItemPricingType.Negotiable,
            200m,
            "USD",
            "hora",
            -23.5505,
            -46.6333,
            ItemStatus.OutOfStock,
            newDate);

        Assert.Equal(ItemType.Service, item.Type);
        Assert.Equal("Novo Título", item.Title);
        Assert.Equal(ItemStatus.OutOfStock, item.Status);
        Assert.Equal(newDate, item.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithWhitespaceTitle_TrimsSuccessfully()
    {
        var item = CreateActiveItem();
        var newDate = TestDate.AddHours(1);

        item.UpdateDetails(
            ItemType.Product,
            "  Título Atualizado  ",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            newDate);

        Assert.Equal("Título Atualizado", item.Title);
    }

    // Archive tests
    [Fact]
    public void Archive_FromActiveStatus_ArchivesSuccessfully()
    {
        var item = CreateActiveItem();
        var newDate = TestDate.AddHours(1);

        item.Archive(newDate);

        Assert.Equal(ItemStatus.Archived, item.Status);
        Assert.Equal(newDate, item.UpdatedAtUtc);
    }

    [Fact]
    public void Archive_UpdatesTimestamp()
    {
        var item = CreateActiveItem();
        var originalTimestamp = item.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        item.Archive(DateTime.UtcNow);

        Assert.True(item.UpdatedAtUtc > originalTimestamp);
    }

    // Unicode and special characters tests
    [Fact]
    public void Constructor_WithUnicodeDescription_StoresCorrectly()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            "Descrição com café, naïve, résumé, 文字 e emoji 🍃",
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Contains("café", item.Description!);
        Assert.Contains("文字", item.Description!);
        Assert.Contains("🍃", item.Description!);
    }

    [Fact]
    public void Constructor_WithUnicodeCategory_StoresCorrectly()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            "Categoria com ção e í",
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal("Categoria com ção e í", item.Category);
    }

    [Fact]
    public void Constructor_WithUnicodeTags_StoresCorrectly()
    {
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Item",
            null,
            null,
            "café, orgânico, 文字, 🍃",
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Contains("café", item.Tags!);
        Assert.Contains("文字", item.Tags!);
        Assert.Contains("🍃", item.Tags!);
    }

    // Long title tests
    [Fact]
    public void Constructor_WithLongTitle_StoresSuccessfully()
    {
        var longTitle = new string('A', 500);
        var item = new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            longTitle,
            null,
            null,
            null,
            ItemPricingType.Fixed,
            null,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);

        Assert.Equal(500, item.Title.Length);
    }

    // Helper methods
    private static StoreItem CreateActiveItem()
    {
        return new StoreItem(
            Guid.NewGuid(),
            TestTerritoryId,
            TestStoreId,
            ItemType.Product,
            "Test Item",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            100m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            TestDate,
            TestDate);
    }
}
