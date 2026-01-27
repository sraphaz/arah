namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Tipo de desconto do cupom.
/// </summary>
public enum CouponDiscountType
{
    /// <summary>
    /// Desconto percentual (0-100%).
    /// </summary>
    Percentage = 0,

    /// <summary>
    /// Desconto em valor fixo.
    /// </summary>
    FixedAmount = 1
}
