using Araponga.Api.Contracts.Territories;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class TerritorySelectionRequestValidator : AbstractValidator<TerritorySelectionRequest>
{
    public TerritorySelectionRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .NotEmpty().WithMessage("Territory ID is required.");
    }
}
