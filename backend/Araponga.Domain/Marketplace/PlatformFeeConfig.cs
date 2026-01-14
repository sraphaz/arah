namespace Araponga.Domain.Marketplace;

/// <summary>
/// Representa uma configuração de taxa da plataforma para um tipo de item em um território.
/// Define como a taxa é calculada (percentual ou fixa) e o valor aplicado.
/// </summary>
public sealed class PlatformFeeConfig
{
    /// <summary>
    /// Inicializa uma nova instância de PlatformFeeConfig.
    /// </summary>
    /// <param name="id">Identificador único da configuração</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="itemType">Tipo de item (Product ou Service)</param>
    /// <param name="feeMode">Modo de cálculo da taxa (Percentage ou Fixed)</param>
    /// <param name="feeValue">Valor da taxa (percentual ou fixo conforme feeMode)</param>
    /// <param name="currency">Moeda (ex: BRL, USD)</param>
    /// <param name="isActive">Indica se a configuração está ativa</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public PlatformFeeConfig(
        Guid id,
        Guid territoryId,
        ItemType itemType,
        PlatformFeeMode feeMode,
        decimal feeValue,
        string? currency,
        bool isActive,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (feeValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(feeValue), "Fee value must be non-negative.");
        }

        Id = id;
        TerritoryId = territoryId;
        ItemType = itemType;
        FeeMode = feeMode;
        FeeValue = feeValue;
        Currency = currency;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único da configuração.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde a configuração se aplica.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Tipo de item ao qual a taxa se aplica (Product ou Service).
    /// </summary>
    public ItemType ItemType { get; }

    /// <summary>
    /// Modo de cálculo da taxa (Percentage ou Fixed).
    /// </summary>
    public PlatformFeeMode FeeMode { get; private set; }

    /// <summary>
    /// Valor da taxa (percentual se FeeMode=Percentage, valor fixo se FeeMode=Fixed).
    /// </summary>
    public decimal FeeValue { get; private set; }

    /// <summary>
    /// Moeda utilizada (ex: BRL, USD).
    /// </summary>
    public string? Currency { get; private set; }

    /// <summary>
    /// Indica se a configuração está ativa e deve ser aplicada.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Data de criação da configuração.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização da configuração.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza os parâmetros da configuração de taxa.
    /// </summary>
    /// <param name="feeMode">Novo modo de cálculo</param>
    /// <param name="feeValue">Novo valor da taxa</param>
    /// <param name="currency">Nova moeda</param>
    /// <param name="isActive">Novo status de ativação</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void Update(PlatformFeeMode feeMode, decimal feeValue, string? currency, bool isActive, DateTime updatedAtUtc)
    {
        FeeMode = feeMode;
        FeeValue = feeValue;
        Currency = currency;
        IsActive = isActive;
        UpdatedAtUtc = updatedAtUtc;
    }
}
