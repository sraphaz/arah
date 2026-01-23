using Araponga.Api.Contracts.Assets;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .ValidGuid();

        RuleFor(x => x.Type)
            .NotEmptyWithMaxLength(100);

        RuleFor(x => x.Name)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(1000);

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
