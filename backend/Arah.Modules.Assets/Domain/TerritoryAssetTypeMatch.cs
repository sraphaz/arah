namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Critérios de filtro Type/Subtype para listagens de TerritoryAsset.
/// <list type="bullet">
/// <item><c>types</c> — só <see cref="TerritoryAsset.Type"/> (API assets).</item>
/// <item><c>subtypes</c> — só <see cref="TerritoryAsset.Subtype"/>.</item>
/// <item><c>typesOrSubtypes</c> — Type <b>ou</b> Subtype (mapa <c>assetTypes</c> / legado + WA-E1).</item>
/// </list>
/// </summary>
public static class TerritoryAssetTypeMatch
{
    public static bool Matches(
        string type,
        string? subtype,
        IReadOnlyCollection<string>? types,
        IReadOnlyCollection<string>? subtypes,
        IReadOnlyCollection<string>? typesOrSubtypes = null)
    {
        if (types is { Count: > 0 }
            && !types.Contains(type, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        if (subtypes is { Count: > 0 })
        {
            if (subtype is null || !subtypes.Contains(subtype, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        if (typesOrSubtypes is { Count: > 0 })
        {
            var orMatch = typesOrSubtypes.Contains(type, StringComparer.OrdinalIgnoreCase)
                || (subtype is not null
                    && typesOrSubtypes.Contains(subtype, StringComparer.OrdinalIgnoreCase));
            if (!orMatch)
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
