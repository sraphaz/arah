using Araponga.Api.Contracts.Assets;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .NotEmpty().WithMessage("TerritoryId é obrigatório.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tipo é obrigatório.")
            .MaximumLength(100).WithMessage("Tipo deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres.");
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
                        .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.");

                    anchor.RuleFor(a => a.Longitude)
                        .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.");
                });
        });
    }
}
