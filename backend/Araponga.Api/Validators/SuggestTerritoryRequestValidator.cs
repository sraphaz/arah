using Araponga.Api.Contracts.Territories;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class SuggestTerritoryRequestValidator : AbstractValidator<SuggestTerritoryRequest>
{
    public SuggestTerritoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres.");
        });

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Cidade é obrigatória.")
            .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Estado é obrigatório.")
            .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres (sigla).");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.");
    }
}
