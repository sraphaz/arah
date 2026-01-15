using Araponga.Api.Contracts.Map;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class SuggestMapEntityRequestValidator : AbstractValidator<SuggestMapEntityRequest>
{
    public SuggestMapEntityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Categoria é obrigatória.")
            .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.");
    }
}
