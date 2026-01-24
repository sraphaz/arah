using Araponga.Domain.Marketplace;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Edge case tests for Cart and CartItem domain entities,
/// focusing on cart creation, item quantity validation, and updates.
/// </summary>
public class CartEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // Cart constructor validation tests
    [Fact]
    public void Cart_Constructor_WithValidData_CreatesSuccessfully()
    {
        var cartId = Guid.NewGuid();

        var cart = new Cart(
            cartId,
            TestTerritoryId,
            TestUserId,
            TestDate,
            TestDate);

        Assert.Equal(cartId, cart.Id);
        Assert.Equal(TestTerritoryId, cart.TerritoryId);
        Assert.Equal(TestUserId, cart.UserId);
    }

    [Fact]
    public void Cart_Constructor_WithNullTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Cart(
                Guid.NewGuid(),
                Guid.Empty,
                TestUserId,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Cart_Constructor_WithNullUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Cart(
                Guid.NewGuid(),
                TestTerritoryId,
                Guid.Empty,
                TestDate,
                TestDate));
    }

    // Cart Touch tests
    [Fact]
    public void Cart_Touch_UpdatesTimestamp()
    {
        var cart = CreateCart();
        var originalTimestamp = cart.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        cart.Touch(DateTime.UtcNow);

        Assert.True(cart.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void Cart_Touch_WithSpecificDate_UpdatesToThatDate()
    {
        var cart = CreateCart();
        var newDate = TestDate.AddHours(2);

        cart.Touch(newDate);

        Assert.Equal(newDate, cart.UpdatedAtUtc);
    }

    // CartItem constructor validation tests
    [Fact]
    public void CartItem_Constructor_WithValidData_CreatesSuccessfully()
    {
        var cartItemId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var cartItem = new CartItem(
            cartItemId,
            cartId,
            itemId,
            5,
            "Notas do item",
            TestDate,
            TestDate);

        Assert.Equal(cartItemId, cartItem.Id);
        Assert.Equal(cartId, cartItem.CartId);
        Assert.Equal(itemId, cartItem.ItemId);
        Assert.Equal(5, cartItem.Quantity);
        Assert.Equal("Notas do item", cartItem.Notes);
    }

    [Fact]
    public void CartItem_Constructor_WithNullCartId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new CartItem(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                1,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void CartItem_Constructor_WithNullItemId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new CartItem(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                1,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void CartItem_Constructor_WithQuantityZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CartItem(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void CartItem_Constructor_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CartItem(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                -1,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void CartItem_Constructor_WithQuantityOne_CreatesSuccessfully()
    {
        var cartItem = new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            null,
            TestDate,
            TestDate);

        Assert.Equal(1, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Constructor_WithLargeQuantity_CreatesSuccessfully()
    {
        var largeQuantity = int.MaxValue;
        var cartItem = new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            largeQuantity,
            null,
            TestDate,
            TestDate);

        Assert.Equal(largeQuantity, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Constructor_WithNullNotes_AllowsNull()
    {
        var cartItem = new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            null,
            TestDate,
            TestDate);

        Assert.Null(cartItem.Notes);
    }

    [Fact]
    public void CartItem_Constructor_WithUnicodeNotes_StoresCorrectly()
    {
        var cartItem = new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            "Notas com café, naïve, résumé, 文字 e emoji 🛒",
            TestDate,
            TestDate);

        Assert.Contains("café", cartItem.Notes!);
        Assert.Contains("文字", cartItem.Notes!);
        Assert.Contains("🛒", cartItem.Notes!);
    }

    // CartItem Update tests
    [Fact]
    public void CartItem_Update_WithValidData_UpdatesSuccessfully()
    {
        var cartItem = CreateCartItem(1, null);
        var newDate = TestDate.AddHours(1);

        cartItem.Update(10, "Novas notas", newDate);

        Assert.Equal(10, cartItem.Quantity);
        Assert.Equal("Novas notas", cartItem.Notes);
        Assert.Equal(newDate, cartItem.UpdatedAtUtc);
    }

    [Fact]
    public void CartItem_Update_WithQuantityZero_AllowsZero()
    {
        // Note: Update method doesn't validate quantity, business logic should handle this
        var cartItem = CreateCartItem(5, "Original");

        cartItem.Update(0, null, TestDate.AddHours(1));

        Assert.Equal(0, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Update_WithNegativeQuantity_AllowsNegative()
    {
        // Note: Update method doesn't validate quantity, business logic should handle this
        var cartItem = CreateCartItem(5, "Original");

        cartItem.Update(-5, null, TestDate.AddHours(1));

        Assert.Equal(-5, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Update_WithLargeQuantity_UpdatesSuccessfully()
    {
        var cartItem = CreateCartItem(1, null);
        var largeQuantity = int.MaxValue;

        cartItem.Update(largeQuantity, null, TestDate.AddHours(1));

        Assert.Equal(largeQuantity, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Update_WithNullNotes_AllowsNull()
    {
        var cartItem = CreateCartItem(5, "Original");

        cartItem.Update(10, null, TestDate.AddHours(1));

        Assert.Null(cartItem.Notes);
    }

    [Fact]
    public void CartItem_Update_WithUnicodeNotes_StoresCorrectly()
    {
        var cartItem = CreateCartItem(1, null);

        cartItem.Update(2, "Notas atualizadas: café, naïve, résumé, 文字 e emoji 🛒", TestDate.AddHours(1));

        Assert.Contains("café", cartItem.Notes!);
        Assert.Contains("文字", cartItem.Notes!);
        Assert.Contains("🛒", cartItem.Notes!);
    }

    [Fact]
    public void CartItem_Update_UpdatesTimestamp()
    {
        var cartItem = CreateCartItem(1, null);
        var originalTimestamp = cartItem.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        cartItem.Update(2, "Atualizado", DateTime.UtcNow);

        Assert.True(cartItem.UpdatedAtUtc > originalTimestamp);
    }

    // Long notes tests
    [Fact]
    public void CartItem_Constructor_WithLongNotes_StoresSuccessfully()
    {
        var longNotes = new string('A', 1000);
        var cartItem = new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            longNotes,
            TestDate,
            TestDate);

        Assert.Equal(1000, cartItem.Notes!.Length);
    }

    // Helper methods
    private static Cart CreateCart()
    {
        return new Cart(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            TestDate,
            TestDate);
    }

    private static CartItem CreateCartItem(int quantity, string? notes)
    {
        return new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            quantity,
            notes,
            TestDate,
            TestDate);
    }
}
