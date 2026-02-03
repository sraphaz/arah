using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using OperationResult = Araponga.Application.Common.OperationResult;

namespace Araponga.Application.Services;

public sealed class CouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork)
    {
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria um novo cupom.
    /// </summary>
    public async Task<Result<Coupon>> CreateCouponAsync(
        string code,
        string name,
        string? description,
        CouponDiscountType discountType,
        decimal discountValue,
        DateTime validFrom,
        DateTime? validUntil,
        int? maxUses,
        CancellationToken cancellationToken)
    {
        // Verificar se código já existe
        var exists = await _couponRepository.CodeExistsAsync(code, cancellationToken);
        if (exists)
        {
            return Result<Coupon>.Failure("Coupon code already exists.");
        }

        // Validar discountValue antes de criar o Coupon
        if (discountType == CouponDiscountType.Percentage && (discountValue < 0 || discountValue > 100))
        {
            return Result<Coupon>.Failure("Percentage discount must be between 0 and 100.");
        }
        
        if (discountType == CouponDiscountType.FixedAmount && discountValue < 0)
        {
            return Result<Coupon>.Failure("Fixed amount discount cannot be negative.");
        }

        var coupon = new Coupon(
            Guid.NewGuid(),
            code,
            name,
            description,
            discountType,
            discountValue,
            validFrom,
            validUntil,
            maxUses);

        await _couponRepository.AddAsync(coupon, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Coupon>.Success(coupon);
    }

    /// <summary>
    /// Valida um cupom.
    /// </summary>
    public async Task<Result<Coupon>> ValidateCouponAsync(
        string code,
        CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken);
        if (coupon == null)
        {
            return Result<Coupon>.Failure("Coupon not found.");
        }

        if (!coupon.IsValid(DateTime.UtcNow))
        {
            return Result<Coupon>.Failure("Coupon is not valid.");
        }

        return Result<Coupon>.Success(coupon);
    }

    /// <summary>
    /// Aplica cupom a uma assinatura.
    /// </summary>
    public async Task<OperationResult> ApplyCouponToSubscriptionAsync(
        Guid subscriptionId,
        string couponCode,
        ISubscriptionCouponRepository subscriptionCouponRepository,
        CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByCodeAsync(couponCode, cancellationToken);
        if (coupon == null)
        {
            return OperationResult.Failure("Coupon not found.");
        }

        if (!coupon.IsValid(DateTime.UtcNow))
        {
            return OperationResult.Failure("Coupon is not valid.");
        }

        // Verificar se já existe cupom aplicado
        var existing = await subscriptionCouponRepository.GetBySubscriptionIdAsync(subscriptionId, cancellationToken);
        if (existing != null)
        {
            return OperationResult.Failure("Subscription already has a coupon applied.");
        }

        // Aplicar cupom
        var subscriptionCoupon = new SubscriptionCoupon(
            Guid.NewGuid(),
            subscriptionId,
            coupon.Id);
        await subscriptionCouponRepository.AddAsync(subscriptionCoupon, cancellationToken);
        
        coupon.Use();
        await _couponRepository.UpdateAsync(coupon, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Lista cupons.
    /// </summary>
    public async Task<IReadOnlyList<Coupon>> ListCouponsAsync(
        bool? isActive,
        CancellationToken cancellationToken)
    {
        return await _couponRepository.ListAsync(isActive, cancellationToken);
    }
}
