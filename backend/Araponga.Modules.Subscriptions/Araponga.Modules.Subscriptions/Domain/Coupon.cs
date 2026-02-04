namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Cupom de desconto.
/// </summary>
public sealed class Coupon
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Código do cupom (único).
    /// </summary>
    public string Code { get; private set; }
    
    public string Name { get; private set; }
    public string? Description { get; private set; }
    
    public CouponDiscountType DiscountType { get; private set; }
    
    /// <summary>
    /// Valor do desconto (percentual 0-100 ou valor fixo).
    /// </summary>
    public decimal DiscountValue { get; private set; }
    
    /// <summary>
    /// Data de início da validade.
    /// </summary>
    public DateTime ValidFrom { get; private set; }
    
    /// <summary>
    /// Data de fim da validade (nullable).
    /// </summary>
    public DateTime? ValidUntil { get; private set; }
    
    /// <summary>
    /// Máximo de usos (nullable = ilimitado).
    /// </summary>
    public int? MaxUses { get; private set; }
    
    /// <summary>
    /// Contador de usos.
    /// </summary>
    public int UsedCount { get; private set; }
    
    /// <summary>
    /// Se o cupom está ativo.
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// ID do cupom no Stripe (nullable).
    /// </summary>
    public string? StripeCouponId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    private Coupon()
    {
        Code = string.Empty;
        Name = string.Empty;
    }
    
    public Coupon(
        Guid id,
        string code,
        string name,
        string? description,
        CouponDiscountType discountType,
        decimal discountValue,
        DateTime validFrom,
        DateTime? validUntil = null,
        int? maxUses = null,
        string? stripeCouponId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }
        
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }
        
        if (discountType == CouponDiscountType.Percentage && (discountValue < 0 || discountValue > 100))
        {
            throw new ArgumentException("Percentage discount must be between 0 and 100.", nameof(discountValue));
        }
        
        if (discountType == CouponDiscountType.FixedAmount && discountValue < 0)
        {
            throw new ArgumentException("Fixed amount discount cannot be negative.", nameof(discountValue));
        }
        
        Id = id;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();
        DiscountType = discountType;
        DiscountValue = discountValue;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        MaxUses = maxUses;
        UsedCount = 0;
        IsActive = true;
        StripeCouponId = stripeCouponId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public bool IsValid(DateTime now)
    {
        if (!IsActive)
        {
            return false;
        }
        
        if (now < ValidFrom)
        {
            return false;
        }
        
        if (ValidUntil.HasValue && now > ValidUntil.Value)
        {
            return false;
        }
        
        if (MaxUses.HasValue && UsedCount >= MaxUses.Value)
        {
            return false;
        }
        
        return true;
    }
    
    public void Use()
    {
        if (MaxUses.HasValue && UsedCount >= MaxUses.Value)
        {
            throw new InvalidOperationException("Coupon has reached maximum uses.");
        }
        
        UsedCount++;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStripeId(string? stripeCouponId)
    {
        StripeCouponId = stripeCouponId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Método para restaurar estado do banco de dados (usado por repositórios).
    /// </summary>
    public void RestoreState(int usedCount, bool isActive, DateTime createdAtUtc, DateTime updatedAtUtc)
    {
        UsedCount = usedCount;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }
}
