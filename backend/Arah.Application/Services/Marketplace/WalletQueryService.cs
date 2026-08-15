using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Domain.Financial;
using Arah.Modules.Marketplace.Application.Interfaces;

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
        var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            wallet = await TryProjectOwnSellerBalanceAsync(walletId, requestingUserId, cancellationToken);
        }

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

        var totalCents = balance.PendingAmountInCents
            + balance.ReadyForPayoutAmountInCents
            + balance.PaidAmountInCents;
        var wallet = Wallet.ForSeller(
            balance.Id,
            balance.SellerUserId,
            balance.TerritoryId,
            totalCents / 100m,
            balance.Currency,
            DateTimeOffset.UtcNow);

        await _wallets.AddAsync(wallet, cancellationToken);
        return wallet;
    }
}
