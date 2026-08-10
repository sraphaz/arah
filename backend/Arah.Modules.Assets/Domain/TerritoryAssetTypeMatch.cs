namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Critério de filtro Type/Subtype para listagens de TerritoryAsset (WA-E2).
/// <c>types</c> casa com <see cref="TerritoryAsset.Type"/> ou <see cref="TerritoryAsset.Subtype"/>
/// (compatível com legado type=river e ponte WA-E1 type=natural + subtype=river).
/// <c>subtypes</c> casa apenas com Subtype.
/// </summary>
public static class TerritoryAssetTypeMatch
{
    public static bool Matches(
        string type,
        string? subtype,
        IReadOnlyCollection<string>? types,
        IReadOnlyCollection<string>? subtypes)
    {
        if (types is { Count: > 0 })
        {
            var typeMatch = types.Contains(type, StringComparer.OrdinalIgnoreCase)
                || (subtype is not null && types.Contains(subtype, StringComparer.OrdinalIgnoreCase));
            if (!typeMatch)
            {
                return false;
            }
        }

        if (subtypes is { Count: > 0 })
        {
            if (subtype is null || !subtypes.Contains(subtype, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public static IReadOnlyCollection<string>? NormalizeFilter(IReadOnlyCollection<string>? values)
    {
        if (values is null || values.Count == 0)
        {
            return null;
        }

        return values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
