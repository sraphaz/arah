using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

public sealed class SellerTransactionTests
{
    [Fact]
    public void Constructor_ShouldSetInitialStatusToPending()
    {
        // Arrange & Act
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000, // grossAmountInCents
            2000,  // platformFeeInCents
            "BRL");

        // Assert
        Assert.Equal(SellerTransactionStatus.Pending, transaction.Status);
        Assert.Equal(8000, transaction.NetAmountInCents); // 10000 - 2000
        Assert.Null(transaction.PayoutId);
    }

    [Fact]
    public void MarkAsReadyForPayout_ShouldChangeStatus()
    {
        // Arrange
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000,
            2000,
            "BRL");

        // Act
        transaction.MarkAsReadyForPayout();

        // Assert
        Assert.Equal(SellerTransactionStatus.ReadyForPayout, transaction.Status);
        Assert.NotNull(transaction.ReadyForPayoutAtUtc);
    }

    [Fact]
    public void StartPayout_ShouldChangeStatusToProcessing()
    {
        // Arrange
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000,
            2000,
            "BRL");
        transaction.MarkAsReadyForPayout();
        var payoutId = "payout-123";

        // Act
        transaction.StartPayout(payoutId);

        // Assert
        Assert.Equal(SellerTransactionStatus.ProcessingPayout, transaction.Status);
        Assert.Equal(payoutId, transaction.PayoutId);
    }

    [Fact]
    public void CompletePayout_ShouldChangeStatusToPaid()
    {
        // Arrange
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000,
            2000,
            "BRL");
        transaction.MarkAsReadyForPayout();
        transaction.StartPayout("payout-123");

        // Act
        transaction.CompletePayout();

        // Assert
        Assert.Equal(SellerTransactionStatus.Paid, transaction.Status);
        Assert.NotNull(transaction.PaidAtUtc);
    }

    [Fact]
    public void FailPayout_ShouldChangeStatusToFailed()
    {
        // Arrange
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000,
            2000,
            "BRL");
        transaction.MarkAsReadyForPayout();
        transaction.StartPayout("payout-123");

        // Act
        transaction.FailPayout();

        // Assert
        Assert.Equal(SellerTransactionStatus.Failed, transaction.Status);
    }

    [Fact]
    public void StartPayout_ShouldThrowIfNotReadyForPayout()
    {
        // Arrange
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000,
            2000,
            "BRL");
        // Status is still Pending

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => transaction.StartPayout("payout-123"));
    }
}
