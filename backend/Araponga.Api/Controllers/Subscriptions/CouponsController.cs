using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/coupons")]
[Produces("application/json")]
[Tags("Subscriptions")]
public sealed class CouponsController : ControllerBase
{
    private readonly CouponService _couponService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public CouponsController(
        CouponService couponService,
        CurrentUserAccessor currentUserAccessor)
    {
        _couponService = couponService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Valida um cupom.
    /// </summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponResponse>> Validate(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _couponService.ValidateCouponAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(new ErrorResponse { Message = result.Error });
            }
            return BadRequest(new ErrorResponse { Message = result.Error ?? "Invalid coupon." });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var coupon = result.Value!;
        return Ok(ToResponse(coupon));
    }

    private static CouponResponse ToResponse(Coupon coupon)
    {
        return new CouponResponse
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Name = coupon.Name,
            Description = coupon.Description,
            DiscountType = coupon.DiscountType.ToString(),
            DiscountValue = coupon.DiscountValue,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MaxUses = coupon.MaxUses,
            UsedCount = coupon.UsedCount,
            IsActive = coupon.IsActive
        };
    }
}
