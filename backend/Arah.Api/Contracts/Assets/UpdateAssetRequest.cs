using System.Text.Json.Serialization;

namespace Arah.Api.Contracts.Assets;

/// <summary>
/// PATCH de asset. <see cref="Subtype"/> é tri-estado:
/// omitido (SubtypeSpecified=false) preserva o valor atual;
/// presente com null remove; presente com valor substitui.
/// </summary>
public sealed class UpdateAssetRequest
{
    public UpdateAssetRequest()
    {
    }

    public UpdateAssetRequest(
        string type,
        string name,
        string? description,
        IReadOnlyCollection<AssetGeoAnchorRequest> geoAnchors,
        string? subtype = null,
        bool subtypeSpecified = false)
    {
        Type = type;
        Name = name;
        Description = description;
        GeoAnchors = geoAnchors;
        if (subtypeSpecified)
        {
            _subtype = subtype;
            SubtypeSpecified = true;
        }
    }

    public string Type { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IReadOnlyCollection<AssetGeoAnchorRequest> GeoAnchors { get; init; } = Array.Empty<AssetGeoAnchorRequest>();

    [JsonIgnore]
    public bool SubtypeSpecified { get; private set; }

    private string? _subtype;

    public string? Subtype
    {
        get => _subtype;
        init
        {
            _subtype = value;
            SubtypeSpecified = true;
        }
    }
}
