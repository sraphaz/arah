using Araponga.Api.Contracts.Users;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class UpdateContactInfoRequestValidator : AbstractValidator<UpdateContactInfoRequest>
{
    public UpdateContactInfoRequestValidator()
    {
        // Pelo menos um campo deve ser fornecido
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || 
                      !string.IsNullOrWhiteSpace(x.PhoneNumber) || 
                      !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Pelo menos um campo de contato deve ser fornecido.");

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email inválido.")
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Address), () =>
        {
            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Endereço deve ter no máximo 500 caracteres.");
        });
    }
}
