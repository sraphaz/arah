using Araponga.Domain.Marketplace;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Edge case tests for the Store domain entity, focusing on store creation,
/// status transitions, contact information, and validation.
/// </summary>
public class StoreEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestOwnerUserId = Guid.NewGuid();

    // Constructor validation tests
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
    public void Constructor_WithNullTerritoryId_ThrowsArgumentException()
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

    [Fact]
    public void Constructor_WithNullOwnerUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Store(
                Guid.NewGuid(),
                TestTerritoryId,
                Guid.Empty,
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

    [Fact]
    public void Constructor_WithNullDisplayName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Store(
                Guid.NewGuid(),
                TestTerritoryId,
                TestOwnerUserId,
                null!,
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

    [Fact]
    public void Constructor_WithEmptyDisplayName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Store(
                Guid.NewGuid(),
                TestTerritoryId,
                TestOwnerUserId,
                "   ",
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

    [Fact]
    public void Constructor_WithWhitespaceDisplayName_TrimsSuccessfully()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "  Minha Loja  ",
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
            TestDate);

        Assert.Equal("Minha Loja", store.DisplayName);
    }

    [Fact]
    public void Constructor_WithUnicodeDisplayName_TrimsAndStores()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "  Loja Café & Cia 🏪  ",
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
            TestDate);

        Assert.Equal("Loja Café & Cia 🏪", store.DisplayName);
    }

    // Status tests
    [Fact]
    public void Constructor_WithAllStoreStatuses_CreatesSuccessfully()
    {
        var statuses = new[] { StoreStatus.Active, StoreStatus.Paused, StoreStatus.Archived };

        foreach (var status in statuses)
        {
            var store = new Store(
                Guid.NewGuid(),
                TestTerritoryId,
                TestOwnerUserId,
                "Loja",
                null,
                status,
                false,
                StoreContactVisibility.Public,
                null,
                null,
                null,
                null,
                null,
                null,
                TestDate,
                TestDate);

            Assert.Equal(status, store.Status);
        }
    }

    // Contact visibility tests
    [Fact]
    public void Constructor_WithAllContactVisibilities_CreatesSuccessfully()
    {
        var visibilities = new[] { StoreContactVisibility.OnInquiryOnly, StoreContactVisibility.Public };

        foreach (var visibility in visibilities)
        {
            var store = new Store(
                Guid.NewGuid(),
                TestTerritoryId,
                TestOwnerUserId,
                "Loja",
                null,
                StoreStatus.Active,
                false,
                visibility,
                null,
                null,
                null,
                null,
                null,
                null,
                TestDate,
                TestDate);

            Assert.Equal(visibility, store.ContactVisibility);
        }
    }

    // Payments enabled tests
    [Fact]
    public void Constructor_WithPaymentsEnabledTrue_StoresCorrectly()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "Loja",
            null,
            StoreStatus.Active,
            true,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            TestDate,
            TestDate);

        Assert.True(store.PaymentsEnabled);
    }

    [Fact]
    public void Constructor_WithPaymentsEnabledFalse_StoresCorrectly()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
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
            TestDate);

        Assert.False(store.PaymentsEnabled);
    }

    // Contact information tests
    [Fact]
    public void Constructor_WithAllContactInfo_StoresCorrectly()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "Loja",
            "Descrição",
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

        Assert.Equal("+5511999999999", store.Phone);
        Assert.Equal("+5511888888888", store.Whatsapp);
        Assert.Equal("loja@example.com", store.Email);
        Assert.Equal("@minhaloja", store.Instagram);
        Assert.Equal("https://minhaloja.com", store.Website);
        Assert.Equal("WhatsApp", store.PreferredContactMethod);
    }

    [Fact]
    public void Constructor_WithNullContactInfo_AllowsNull()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
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
            TestDate);

        Assert.Null(store.Phone);
        Assert.Null(store.Whatsapp);
        Assert.Null(store.Email);
        Assert.Null(store.Instagram);
        Assert.Null(store.Website);
        Assert.Null(store.PreferredContactMethod);
        Assert.Null(store.Description);
    }

    // UpdateDetails tests
    [Fact]
    public void UpdateDetails_WithValidData_UpdatesSuccessfully()
    {
        var store = CreateActiveStore();
        var newDate = TestDate.AddHours(1);

        store.UpdateDetails(
            "Nova Loja",
            "Nova descrição",
            StoreContactVisibility.OnInquiryOnly,
            "+5511777777777",
            "+5511666666666",
            "novo@example.com",
            "@novaloja",
            "https://novaloja.com",
            "Email",
            newDate);

        Assert.Equal("Nova Loja", store.DisplayName);
        Assert.Equal("Nova descrição", store.Description);
        Assert.Equal(StoreContactVisibility.OnInquiryOnly, store.ContactVisibility);
        Assert.Equal(newDate, store.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithWhitespaceDisplayName_TrimsSuccessfully()
    {
        var store = CreateActiveStore();
        var newDate = TestDate.AddHours(1);

        store.UpdateDetails(
            "  Loja Atualizada  ",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            newDate);

        Assert.Equal("Loja Atualizada", store.DisplayName);
    }

    [Fact]
    public void UpdateDetails_WithUnicodeDisplayName_TrimsAndStores()
    {
        var store = CreateActiveStore();
        var newDate = TestDate.AddHours(1);

        store.UpdateDetails(
            "  Loja Café & Cia 🏪  ",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            newDate);

        Assert.Equal("Loja Café & Cia 🏪", store.DisplayName);
    }

    // SetStatus tests
    [Fact]
    public void SetStatus_WithAllStatuses_UpdatesSuccessfully()
    {
        var store = CreateActiveStore();
        var newDate = TestDate.AddHours(1);

        store.SetStatus(StoreStatus.Paused, newDate);
        Assert.Equal(StoreStatus.Paused, store.Status);
        Assert.Equal(newDate, store.UpdatedAtUtc);

        store.SetStatus(StoreStatus.Archived, newDate.AddHours(1));
        Assert.Equal(StoreStatus.Archived, store.Status);

        store.SetStatus(StoreStatus.Active, newDate.AddHours(2));
        Assert.Equal(StoreStatus.Active, store.Status);
    }

    [Fact]
    public void SetStatus_UpdatesTimestamp()
    {
        var store = CreateActiveStore();
        var originalTimestamp = store.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        store.SetStatus(StoreStatus.Paused, DateTime.UtcNow);

        Assert.True(store.UpdatedAtUtc > originalTimestamp);
    }

    // SetPaymentsEnabled tests
    [Fact]
    public void SetPaymentsEnabled_WithTrue_UpdatesSuccessfully()
    {
        var store = CreateActiveStore();
        store.SetPaymentsEnabled(false, TestDate);
        Assert.False(store.PaymentsEnabled);

        var newDate = TestDate.AddHours(1);
        store.SetPaymentsEnabled(true, newDate);
        Assert.True(store.PaymentsEnabled);
        Assert.Equal(newDate, store.UpdatedAtUtc);
    }

    [Fact]
    public void SetPaymentsEnabled_WithFalse_UpdatesSuccessfully()
    {
        var store = CreateActiveStore();
        Assert.True(store.PaymentsEnabled);

        var newDate = TestDate.AddHours(1);
        store.SetPaymentsEnabled(false, newDate);
        Assert.False(store.PaymentsEnabled);
        Assert.Equal(newDate, store.UpdatedAtUtc);
    }

    [Fact]
    public void SetPaymentsEnabled_UpdatesTimestamp()
    {
        var store = CreateActiveStore();
        var originalTimestamp = store.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        store.SetPaymentsEnabled(false, DateTime.UtcNow);

        Assert.True(store.UpdatedAtUtc > originalTimestamp);
    }

    // Long display name tests
    [Fact]
    public void Constructor_WithLongDisplayName_StoresSuccessfully()
    {
        var longName = new string('A', 200);
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            longName,
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
            TestDate);

        Assert.Equal(200, store.DisplayName.Length);
    }

    // Unicode in description tests
    [Fact]
    public void Constructor_WithUnicodeDescription_StoresCorrectly()
    {
        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "Loja",
            "Descrição com café, naïve, résumé, 文字 e emoji 🏪",
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
            TestDate);

        Assert.Contains("café", store.Description!);
        Assert.Contains("文字", store.Description!);
        Assert.Contains("🏪", store.Description!);
    }

    // Helper methods
    private static Store CreateActiveStore()
    {
        return new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestOwnerUserId,
            "Test Store",
            null,
            StoreStatus.Active,
            true,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            TestDate,
            TestDate);
    }
}
