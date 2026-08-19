namespace Arah.Domain.Fiscal;

/// <summary>
/// Meios de pagamento habilitáveis por território (config, não gateway de checkout).
/// </summary>
public enum TerritoryPaymentMethodKind
{
    Pix = 1,
    Card = 2,
    Boleto = 3
}
