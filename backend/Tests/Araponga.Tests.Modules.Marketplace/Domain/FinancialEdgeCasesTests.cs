using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

/// <summary>
/// Edge case tests for SellerBalance and SellerTransaction domain entities,
/// focusing on negative/zero values, status transitions, and invalid currencies.
/// </summary>
public class FinancialEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // SellerBalance edge cases
    [Fact]
    public void SellerBalance_Constructor_WithValidData_CreatesSuccessfully()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        Assert.Equal(0, balance.PendingAmountInCents);
        Assert.Equal(0, balance.ReadyForPayoutAmountInCents);
        Assert.Equal(0, balance.PaidAmountInCents);
        Assert.Equal("BRL", balance.Currency);
    }

    [Fact]
    public void SellerBalance_AddPendingAmount_WithNegativeValue_ThrowsArgumentException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        Assert.Throws<ArgumentException>(() => balance.AddPendingAmount(-100));
    }

    [Fact]
    public void SellerBalance_AddPendingAmount_WithZeroValue_DoesNotThrow()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(0);

        Assert.Equal(0, balance.PendingAmountInCents);
    }

    [Fact]
    public void SellerBalance_AddPendingAmount_WithLargeValue_AddsSuccessfully()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        var largeAmount = long.MaxValue;
        balance.AddPendingAmount(largeAmount);

        Assert.Equal(largeAmount, balance.PendingAmountInCents);
    }

    [Fact]
    public void SellerBalance_MoveToReadyForPayout_WithNegativeValue_ThrowsArgumentException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);

        Assert.Throws<ArgumentException>(() => balance.MoveToReadyForPayout(-100));
    }

    [Fact]
    public void SellerBalance_MoveToReadyForPayout_WithAmountExceedingPending_ThrowsInvalidOperationException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);

        Assert.Throws<InvalidOperationException>(() => balance.MoveToReadyForPayout(2000));
    }

    [Fact]
    public void SellerBalance_MoveToReadyForPayout_WithExactPendingAmount_MovesSuccessfully()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);
        balance.MoveToReadyForPayout(1000);

        Assert.Equal(0, balance.PendingAmountInCents);
        Assert.Equal(1000, balance.ReadyForPayoutAmountInCents);
    }

    [Fact]
    public void SellerBalance_MarkAsPaid_WithNegativeValue_ThrowsArgumentException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);
        balance.MoveToReadyForPayout(1000);

        Assert.Throws<ArgumentException>(() => balance.MarkAsPaid(-100));
    }

    [Fact]
    public void SellerBalance_MarkAsPaid_WithAmountExceedingReady_ThrowsInvalidOperationException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);
        balance.MoveToReadyForPayout(1000);

        Assert.Throws<InvalidOperationException>(() => balance.MarkAsPaid(2000));
    }

    [Fact]
    public void SellerBalance_CancelPendingAmount_WithNegativeValue_ThrowsArgumentException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);

        Assert.Throws<ArgumentException>(() => balance.CancelPendingAmount(-100));
    }

    [Fact]
    public void SellerBalance_CancelPendingAmount_WithAmountExceedingPending_ThrowsInvalidOperationException()
    {
        var balance = new SellerBalance(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "BRL");

        balance.AddPendingAmount(1000);

        Assert.Throws<InvalidOperationException>(() => balance.CancelPendingAmount(2000));
    }

    // SellerTransaction edge cases
    [Fact]
    public void SellerTransaction_Constructor_WithValidData_CreatesSuccessfully()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        Assert.Equal(10000, transaction.GrossAmountInCents);
        Assert.Equal(1000, transaction.PlatformFeeInCents);
        Assert.Equal(9000, transaction.NetAmountInCents);
        Assert.Equal(SellerTransactionStatus.Pending, transaction.Status);
    }

    [Fact]
    public void SellerTransaction_Constructor_WithZeroGrossAmount_CreatesSuccessfully()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            0,
            0,
            "BRL");

        Assert.Equal(0, transaction.GrossAmountInCents);
        Assert.Equal(0, transaction.NetAmountInCents);
    }

    [Fact]
    public void SellerTransaction_Constructor_WithPlatformFeeExceedingGross_Allows()
    {
        // Nota: Não há validação que impede fee > gross, apenas calcula NetAmount
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            1000,
            2000,
            "BRL");

        Assert.Equal(1000, transaction.GrossAmountInCents);
        Assert.Equal(2000, transaction.PlatformFeeInCents);
        Assert.Equal(-1000, transaction.NetAmountInCents); // Negativo
    }

    [Fact]
    public void SellerTransaction_MarkAsReadyForPayout_WhenNotPending_ThrowsInvalidOperationException()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.MarkAsReadyForPayout();
        transaction.StartPayout("payout-123");

        Assert.Throws<InvalidOperationException>(() => transaction.MarkAsReadyForPayout());
    }

    [Fact]
    public void SellerTransaction_StartPayout_WhenNotReadyForPayout_ThrowsInvalidOperationException()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        Assert.Throws<InvalidOperationException>(() => transaction.StartPayout("payout-123"));
    }

    [Fact]
    public void SellerTransaction_CompletePayout_WhenNotProcessing_ThrowsInvalidOperationException()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.MarkAsReadyForPayout();

        Assert.Throws<InvalidOperationException>(() => transaction.CompletePayout());
    }

    [Fact]
    public void SellerTransaction_FailPayout_WhenNotProcessing_ThrowsInvalidOperationException()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.MarkAsReadyForPayout();

        Assert.Throws<InvalidOperationException>(() => transaction.FailPayout());
    }

    [Fact]
    public void SellerTransaction_Cancel_WhenPaid_ThrowsInvalidOperationException()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.MarkAsReadyForPayout();
        transaction.StartPayout("payout-123");
        transaction.CompletePayout();

        Assert.Throws<InvalidOperationException>(() => transaction.Cancel());
    }

    [Fact]
    public void SellerTransaction_StatusTransitions_WorkCorrectly()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        Assert.Equal(SellerTransactionStatus.Pending, transaction.Status);

        transaction.MarkAsReadyForPayout();
        Assert.Equal(SellerTransactionStatus.ReadyForPayout, transaction.Status);

        transaction.StartPayout("payout-123");
        Assert.Equal(SellerTransactionStatus.ProcessingPayout, transaction.Status);

        transaction.CompletePayout();
        Assert.Equal(SellerTransactionStatus.Paid, transaction.Status);
    }

    [Fact]
    public void SellerTransaction_FailPayout_UpdatesStatus()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.MarkAsReadyForPayout();
        transaction.StartPayout("payout-123");
        transaction.FailPayout();

        Assert.Equal(SellerTransactionStatus.Failed, transaction.Status);
    }

    [Fact]
    public void SellerTransaction_Cancel_WhenPending_UpdatesStatus()
    {
        var transaction = new SellerTransaction(
            Guid.NewGuid(),
            TestTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            10000,
            1000,
            "BRL");

        transaction.Cancel();

        Assert.Equal(SellerTransactionStatus.Canceled, transaction.Status);
    }
}
