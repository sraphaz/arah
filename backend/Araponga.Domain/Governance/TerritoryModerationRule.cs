using System.Text.Json;

namespace Araponga.Domain.Governance;

/// <summary>
/// Representa uma regra de moderação comunitária para um território.
/// </summary>
public sealed class TerritoryModerationRule
{
    public TerritoryModerationRule(
        Guid id,
        Guid territoryId,
        Guid? createdByVotingId,
        RuleType ruleType,
        string ruleJson,
        bool isActive,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(ruleJson))
        {
            throw new ArgumentException("Rule JSON is required.", nameof(ruleJson));
        }

        // Validar que é JSON válido
        try
        {
            JsonSerializer.Deserialize<object>(ruleJson);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Rule JSON must be valid JSON.", nameof(ruleJson));
        }

        Id = id;
        TerritoryId = territoryId;
        CreatedByVotingId = createdByVotingId;
        RuleType = ruleType;
        RuleJson = ruleJson;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único da regra.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde a regra se aplica.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador da votação que criou esta regra (nullable, se criado diretamente por curador).
    /// </summary>
    public Guid? CreatedByVotingId { get; }

    /// <summary>
    /// Tipo da regra.
    /// </summary>
    public RuleType RuleType { get; }

    /// <summary>
    /// Configuração da regra em JSON.
    /// </summary>
    public string RuleJson { get; private set; }

    /// <summary>
    /// Indica se a regra está ativa.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Data/hora UTC de criação da regra.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data/hora UTC da última atualização da regra.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Ativa a regra.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Desativa a regra.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza a configuração da regra.
    /// </summary>
    public void UpdateRule(string ruleJson)
    {
        if (string.IsNullOrWhiteSpace(ruleJson))
        {
            throw new ArgumentException("Rule JSON is required.", nameof(ruleJson));
        }

        // Validar que é JSON válido
        try
        {
            JsonSerializer.Deserialize<object>(ruleJson);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Rule JSON must be valid JSON.", nameof(ruleJson));
        }

        RuleJson = ruleJson;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
