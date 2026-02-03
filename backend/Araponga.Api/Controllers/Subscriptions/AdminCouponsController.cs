using Araponga.Api.Contracts.Admin;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/admin/coupons")]
[Produces("application/json")]
[Tags("Admin - Subscriptions")]
[Authorize(Policy = "SystemAdmin")]
public sealed class AdminCouponsController : ControllerBase
{
    private readonly CouponService _couponService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public AdminCouponsController(
        CouponService couponService,
        CurrentUserAccessor currentUserAccessor)
    {
        _couponService = couponService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista cupons.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CouponResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CouponResponse>>> List(
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var coupons = await _couponService.ListCouponsAsync(isActive, cancellationToken);
        var response = coupons.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Cria cupom.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CouponResponse>> Create(
        [FromBody] CreateCouponRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _couponService.CreateCouponAsync(
            request.Code,
            request.Name,
            request.Description,
            Enum.Parse<CouponDiscountType>(request.DiscountType),
            request.DiscountValue,
            request.ValidFrom,
            request.ValidUntil,
            request.MaxUses,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value!.Id },
            ToResponse(result.Value));
    }

    /// <summary>
    /// Obtém cupom por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponResponse>> Get(
        Guid id,
        [FromServices] ICouponRepository couponRepository,
        CancellationToken cancellationToken)
    {
        var coupon = await couponRepository.GetByIdAsync(id, cancellationToken);
        if (coupon == null)
        {
            return NotFound();
        }

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
