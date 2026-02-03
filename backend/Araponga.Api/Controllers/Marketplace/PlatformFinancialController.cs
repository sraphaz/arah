using Araponga.Api;
using Araponga.Api.Contracts.Payout;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/platform-financial")]
[Produces("application/json")]
[Tags("Platform Financial")]
public sealed class PlatformFinancialController : ControllerBase
{
    private readonly IPlatformFinancialBalanceRepository _balanceRepository;
    private readonly IPlatformRevenueTransactionRepository _revenueRepository;
    private readonly IPlatformExpenseTransactionRepository _expenseRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public PlatformFinancialController(
        IPlatformFinancialBalanceRepository balanceRepository,
        IPlatformRevenueTransactionRepository revenueRepository,
        IPlatformExpenseTransactionRepository expenseRepository,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _balanceRepository = balanceRepository;
        _revenueRepository = revenueRepository;
        _expenseRepository = expenseRepository;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Consulta o saldo financeiro da plataforma no território.
    /// </summary>
    [HttpGet]
    [Route("balance")]
    [ProducesResponseType(typeof(PlatformFinancialBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformFinancialBalanceResponse>> GetBalance(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar permissão (Admin do sistema ou FinancialManager do território)
        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            // TODO: Verificar se é FinancialManager do território quando capabilities estiver completo
            return Forbid();
        }

        var balance = await _balanceRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        if (balance is null)
        {
            return NotFound(new { error = "Platform financial balance not found." });
        }

        var response = new PlatformFinancialBalanceResponse(
            balance.Id,
            balance.TerritoryId,
            balance.TotalRevenueInCents,
            balance.TotalExpensesInCents,
            balance.NetBalanceInCents,
            balance.Currency,
            balance.CreatedAtUtc,
            balance.UpdatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Lista transações de receita (fees coletadas) da plataforma no território.
    /// </summary>
    [HttpGet]
    [Route("revenue")]
    [ProducesResponseType(typeof(List<PlatformRevenueTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PlatformRevenueTransactionResponse>>> ListRevenue(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar permissão
        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            // TODO: Verificar se é FinancialManager/FinancialViewer do território
            return Forbid();
        }

        var transactions = await _revenueRepository.GetByTerritoryIdAsync(
            territoryId,
            cancellationToken);

        var pagedTransactions = transactions
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip(skip)
            .Take(Math.Min(take, 100)) // Limitar a 100 por requisição
            .ToList();

        var response = pagedTransactions.Select(t => new PlatformRevenueTransactionResponse(
            t.Id,
            t.TerritoryId,
            t.CheckoutId,
            t.FeeAmountInCents,
            t.Currency,
            t.FinancialTransactionId,
            t.CreatedAtUtc)).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Lista transações de despesa (payouts processados) da plataforma no território.
    /// </summary>
    [HttpGet]
    [Route("expenses")]
    [ProducesResponseType(typeof(List<PlatformExpenseTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PlatformExpenseTransactionResponse>>> ListExpenses(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar permissão
        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            // TODO: Verificar se é FinancialManager/FinancialViewer do território
            return Forbid();
        }

        var transactions = await _expenseRepository.GetByTerritoryIdAsync(
            territoryId,
            cancellationToken);

        var pagedTransactions = transactions
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip(skip)
            .Take(Math.Min(take, 100)) // Limitar a 100 por requisição
            .ToList();

        var response = pagedTransactions.Select(t => new PlatformExpenseTransactionResponse(
            t.Id,
            t.TerritoryId,
            t.SellerTransactionId,
            t.PayoutAmountInCents,
            t.Currency,
            t.PayoutId,
            t.FinancialTransactionId,
            t.CreatedAtUtc)).ToList();

        return Ok(response);
    }
}
