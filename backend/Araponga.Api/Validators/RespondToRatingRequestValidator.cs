using Araponga.Api.Contracts.Marketplace;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class RespondToRatingRequestValidator : AbstractValidator<RespondToRatingRequest>
{
    public RespondToRatingRequestValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty()
            .WithMessage("Response text is required.")
            .MaximumLength(2000)
            .WithMessage("Response text must not exceed 2000 characters.");
    }
}
