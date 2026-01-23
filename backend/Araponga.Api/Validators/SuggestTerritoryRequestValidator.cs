using Araponga.Api.Contracts.Territories;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class SuggestTerritoryRequestValidator : AbstractValidator<SuggestTerritoryRequest>
{
    public SuggestTerritoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(1000);

        RuleFor(x => x.City)
            .NotEmptyWithMaxLength(100);

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Estado é obrigatório.")
            .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres (sigla).");

        RuleFor(x => x.Latitude)
            .Latitude();

        RuleFor(x => x.Longitude)
            .Longitude();
    }
}
