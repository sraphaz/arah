using Arah.Api.Contracts.Assets;
using Arah.Modules.Assets.Domain;
using FluentValidation;

namespace Arah.Api.Validators;

public sealed class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
{
    public UpdateAssetRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmptyWithMaxLength(100);

        RuleFor(x => x.Name)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(1000);

        RuleFor(x => x.Subtype)
            .MaximumLength(40)
            .When(x => x.SubtypeSpecified && !string.IsNullOrWhiteSpace(x.Subtype));

        RuleFor(x => x)
            .Must(request =>
            {
                if (!request.SubtypeSpecified)
                {
                    return true;
                }

                var type = request.Type?.Trim().ToLowerInvariant() ?? string.Empty;
                var subtype = NaturalWaterSubtype.Normalize(request.Subtype);
                return NaturalWaterSubtype.TryValidate(type, subtype, out _);
            })
            .WithMessage(request =>
            {
                var type = request.Type?.Trim().ToLowerInvariant() ?? string.Empty;
                var subtype = NaturalWaterSubtype.Normalize(request.Subtype);
                NaturalWaterSubtype.TryValidate(type, subtype, out var error);
                return error ?? "Invalid subtype.";
            });

        RuleFor(x => x.GeoAnchors)
            .NotEmpty().WithMessage("Pelo menos um GeoAnchor é obrigatório.")
            .Must(anchors => anchors != null && anchors.Count > 0)
            .WithMessage("Pelo menos um GeoAnchor é obrigatório.")
            .Must(anchors => anchors == null || anchors.Count <= 50)
            .WithMessage("Máximo de 50 geo anchors permitidos.");

        When(x => x.GeoAnchors != null, () =>
        {
            RuleForEach(x => x.GeoAnchors)
                .ChildRules(anchor =>
                {
                    anchor.RuleFor(a => a.Latitude)
                        .Latitude();

                    anchor.RuleFor(a => a.Longitude)
                        .Longitude();
                });
        });
    }
}
