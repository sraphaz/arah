using Araponga.Api.Contracts.Users;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class UpdatePrivacyPreferencesRequestValidator : AbstractValidator<UpdatePrivacyPreferencesRequest>
{
    public UpdatePrivacyPreferencesRequestValidator()
    {
        RuleFor(x => x.ProfileVisibility)
            .NotEmpty().WithMessage("Visibilidade do perfil é obrigatória.")
            .Must(visibility => visibility == "Public" || visibility == "ResidentsOnly" || visibility == "Private")
            .WithMessage("Visibilidade do perfil inválida. Valores válidos: Public, ResidentsOnly, Private.");

        RuleFor(x => x.ContactVisibility)
            .NotEmpty().WithMessage("Visibilidade de contato é obrigatória.")
            .Must(visibility => visibility == "Public" || visibility == "ResidentsOnly" || visibility == "Private")
            .WithMessage("Visibilidade de contato inválida. Valores válidos: Public, ResidentsOnly, Private.");
    }
}
