using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Domain.Financial;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;

namespace Arah.Application.Services;

public sealed class WalletQueryService
{
    private readonly IWalletRepository _wallets;
    private readonly ISellerBalanceRepository _sellerBalances;

    public WalletQueryService(IWalletRepository wallets, ISellerBalanceRepository sellerBalances)
    {
        _wallets = wallets;
        _sellerBalances = sellerBalances;
    }

    public async Task<Result<Wallet>> GetAuthorizedAsync(
        Guid walletId,
        Guid requestingUserId,
        CancellationToken cancellationToken)
    {
        // Always reproject from SellerBalance when this id is the caller's ledger row,
        // so sales/payouts are reflected and paid-out funds are not treated as available.
        var projected = await TryProjectOwnSellerBalanceAsync(walletId, requestingUserId, cancellationToken);
        if (projected is not null)
        {
            return Result<Wallet>.Success(projected);
        }

        var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result<Wallet>.Failure("Wallet not found.");
        }

        if (!string.Equals(wallet.OwnerType, "seller", StringComparison.Ordinal) ||
            wallet.OwnerId != requestingUserId)
        {
            return Result<Wallet>.Failure("Forbidden.");
        }

        return Result<Wallet>.Success(wallet);
    }

    private async Task<Wallet?> TryProjectOwnSellerBalanceAsync(
        Guid walletId,
        Guid requestingUserId,
        CancellationToken cancellationToken)
    {
        var balances = await _sellerBalances.GetBySellerUserIdAsync(requestingUserId, cancellationToken);
        var balance = balances.FirstOrDefault(b => b.Id == walletId);
        if (balance is null)
        {
            return null;
        }

        var availableCents = AvailableBalanceInCents(balance);
        var wallet = Wallet.ForSeller(
            balance.Id,
            balance.SellerUserId,
            balance.TerritoryId,
            availableCents / 100m,
            balance.Currency,
            DateTimeOffset.UtcNow);

        await _wallets.UpsertAsync(wallet, cancellationToken);
        return wallet;
    }

    /// <summary>
    /// Funds still held in Aratá: pending + ready for payout.
    /// PaidAmountInCents has already left the wallet and must not inflate balance.
    /// </summary>
    internal static long AvailableBalanceInCents(SellerBalance balance) =>
        balance.PendingAmountInCents + balance.ReadyForPayoutAmountInCents;
}
