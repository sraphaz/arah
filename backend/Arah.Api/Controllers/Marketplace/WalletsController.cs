using Arah.Api.Contracts.Transactions;
using Arah.Api.Security;
using Arah.Application.Services;
using Arah.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/wallets")]
[Produces("application/json")]
[Tags("Wallets — FASE55")]
public sealed class WalletsController : ControllerBase
{
    private readonly WalletQueryService _wallets;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public WalletsController(WalletQueryService wallets, CurrentUserAccessor currentUserAccessor)
    {
        _wallets = wallets;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Carteira Aratá por id (AC-55-10). Projeta SellerBalance quando aplicável.
    /// </summary>
    [HttpGet("{walletId:guid}")]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponse>> Get(
        Guid walletId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _wallets.GetAuthorizedAsync(
            walletId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure)
        {
            if (string.Equals(result.Error, "Forbidden.", StringComparison.Ordinal))
            {
                return Forbid();
            }

            return NotFound(new { error = result.Error ?? "Wallet not found." });
        }

        var wallet = result.Value!;
        return Ok(new WalletResponse(
            wallet.Id,
            wallet.OwnerType,
            wallet.OwnerId,
            wallet.TerritoryId,
            wallet.Balance,
            wallet.Currency,
            wallet.PayoutMethod,
            wallet.CreatedAtUtc,
            wallet.UpdatedAtUtc));
    }
}
