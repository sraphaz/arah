using Araponga.Api;
using Araponga.Api.Contracts.Payout;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/seller-balance")]
[Produces("application/json")]
[Tags("Seller Balance")]
public sealed class SellerBalanceController : ControllerBase
{
    private readonly ISellerBalanceRepository _balanceRepository;
    private readonly ISellerTransactionRepository _transactionRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public SellerBalanceController(
        ISellerBalanceRepository balanceRepository,
        ISellerTransactionRepository transactionRepository,
        CurrentUserAccessor currentUserAccessor)
    {
        _balanceRepository = balanceRepository;
        _transactionRepository = transactionRepository;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Consulta meu saldo como vendedor no território.
    /// </summary>
    [HttpGet]
    [Route("me")]
    [ProducesResponseType(typeof(SellerBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SellerBalanceResponse>> GetMyBalance(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var balance = await _balanceRepository.GetByTerritoryAndSellerAsync(
            territoryId,
            userContext.User.Id,
            cancellationToken);

        if (balance is null)
        {
            return NotFound(new { error = "Balance not found." });
        }

        var response = new SellerBalanceResponse(
            balance.Id,
            balance.TerritoryId,
            balance.SellerUserId,
            balance.PendingAmountInCents,
            balance.ReadyForPayoutAmountInCents,
            balance.PaidAmountInCents,
            balance.Currency,
            balance.CreatedAtUtc,
            balance.UpdatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Consulta minhas transações como vendedor no território.
    /// </summary>
    [HttpGet]
    [Route("me/transactions")]
    [ProducesResponseType(typeof(List<SellerTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SellerTransactionResponse>>> GetMyTransactions(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var transactions = await _transactionRepository.GetBySellerUserIdAsync(
            userContext.User.Id,
            cancellationToken);

        // Filtrar apenas transações do território
        var territoryTransactions = transactions
            .Where(t => t.TerritoryId == territoryId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();

        var response = territoryTransactions.Select(t => new SellerTransactionResponse(
            t.Id,
            t.TerritoryId,
            t.StoreId,
            t.CheckoutId,
            t.GrossAmountInCents,
            t.PlatformFeeInCents,
            t.NetAmountInCents,
            t.Currency,
            t.Status.ToString(),
            t.PayoutId,
            t.CreatedAtUtc,
            t.ReadyForPayoutAtUtc,
            t.PaidAtUtc,
            t.UpdatedAtUtc)).ToList();

        return Ok(response);
    }
}
