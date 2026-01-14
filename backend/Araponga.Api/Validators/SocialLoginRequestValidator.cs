using System.Linq;
using Araponga.Api.Contracts.Auth;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class SocialLoginRequestValidator : AbstractValidator<SocialLoginRequest>
{
    public SocialLoginRequestValidator()
    {
        RuleFor(x => x.AuthProvider)
            .NotEmpty().WithMessage("AuthProvider is required.")
            .MaximumLength(50).WithMessage("AuthProvider must not exceed 50 characters.");

        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("ExternalId is required.")
            .MaximumLength(255).WithMessage("ExternalId must not exceed 255 characters.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("DisplayName is required.")
            .MaximumLength(200).WithMessage("DisplayName must not exceed 200 characters.");

        RuleFor(x => x)
            .Must(request =>
            {
                var hasCpf = !string.IsNullOrWhiteSpace(request.Cpf);
                var hasForeignDocument = !string.IsNullOrWhiteSpace(request.ForeignDocument);
                return hasCpf || hasForeignDocument;
            })
            .WithMessage("Either CPF or ForeignDocument is required.");

        RuleFor(x => x)
            .Must(request =>
            {
                var hasCpf = !string.IsNullOrWhiteSpace(request.Cpf);
                var hasForeignDocument = !string.IsNullOrWhiteSpace(request.ForeignDocument);
                return !(hasCpf && hasForeignDocument);
            })
            .WithMessage("Provide either CPF or ForeignDocument, not both.");

        When(x => !string.IsNullOrWhiteSpace(x.Cpf), () =>
        {
            RuleFor(x => x.Cpf)
                .Must(cpf =>
                {
                    // Remove formatação (pontos, hífens, espaços)
                    var digitsOnly = cpf?.Replace(".", "").Replace("-", "").Replace(" ", "") ?? "";
                    // Deve ter exatamente 11 dígitos
                    return digitsOnly.Length == 11 && digitsOnly.All(char.IsDigit);
                })
                .WithMessage("CPF must be a valid 11-digit number (formatting is optional).");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ForeignDocument), () =>
        {
            RuleFor(x => x.ForeignDocument)
                .MaximumLength(50).WithMessage("ForeignDocument must not exceed 50 characters.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("PhoneNumber must not exceed 20 characters.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Address), () =>
        {
            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
        });
    }
}
