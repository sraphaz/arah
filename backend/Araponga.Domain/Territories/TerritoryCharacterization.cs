namespace Araponga.Domain.Territories;

/// <summary>
/// Representa a caracterização de um território (tags que descrevem o território).
/// </summary>
public sealed class TerritoryCharacterization
{
    public TerritoryCharacterization(
        Guid territoryId,
        IReadOnlyList<string> tags,
        DateTime updatedAtUtc)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        // Normalizar tags (trim, lowercase, remover duplicatas)
        var normalizedTags = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        TerritoryId = territoryId;
        Tags = normalizedTags;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador do território.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Lista de tags que caracterizam o território (normalizadas em lowercase, sem duplicatas).
    /// </summary>
    public IReadOnlyList<string> Tags { get; private set; }

    /// <summary>
    /// Data/hora UTC da última atualização da caracterização.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza as tags do território.
    /// </summary>
    public void UpdateTags(IReadOnlyList<string> tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var normalizedTags = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        Tags = normalizedTags;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
