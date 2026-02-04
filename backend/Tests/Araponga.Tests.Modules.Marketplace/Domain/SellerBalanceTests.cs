using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

public sealed class SellerBalanceTests
{
    [Fact]
    public void AddPendingAmount_ShouldIncreasePendingAmount()
    {
        // Arrange
        var balance = new SellerBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "BRL");

        // Act
        balance.AddPendingAmount(10000); // R$ 100,00

        // Assert
        Assert.Equal(10000, balance.PendingAmountInCents);
        Assert.Equal(0, balance.ReadyForPayoutAmountInCents);
        Assert.Equal(0, balance.PaidAmountInCents);
    }

    [Fact]
    public void MoveToReadyForPayout_ShouldMovePendingToReady()
    {
        // Arrange
        var balance = new SellerBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "BRL");
        balance.AddPendingAmount(10000);

        // Act
        balance.MoveToReadyForPayout(10000);

        // Assert
        Assert.Equal(0, balance.PendingAmountInCents);
        Assert.Equal(10000, balance.ReadyForPayoutAmountInCents);
        Assert.Equal(0, balance.PaidAmountInCents);
    }

    [Fact]
    public void MarkAsPaid_ShouldMoveReadyToPaid()
    {
        // Arrange
        var balance = new SellerBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "BRL");
        balance.AddPendingAmount(10000);
        balance.MoveToReadyForPayout(10000);

        // Act
        balance.MarkAsPaid(10000);

        // Assert
        Assert.Equal(0, balance.PendingAmountInCents);
        Assert.Equal(0, balance.ReadyForPayoutAmountInCents);
        Assert.Equal(10000, balance.PaidAmountInCents);
    }

    [Fact]
    public void CancelPendingAmount_ShouldReducePendingAmount()
    {
        // Arrange
        var balance = new SellerBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "BRL");
        balance.AddPendingAmount(10000);

        // Act
        balance.CancelPendingAmount(5000);

        // Assert
        Assert.Equal(5000, balance.PendingAmountInCents);
    }

    [Fact]
    public void MoveToReadyForPayout_ShouldThrowIfInsufficientPending()
    {
        // Arrange
        var balance = new SellerBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "BRL");
        balance.AddPendingAmount(5000);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => balance.MoveToReadyForPayout(10000));
    }
}
