using Araponga.Api.Contracts.Marketplace;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateRatingRequestValidator : AbstractValidator<CreateRatingRequest>
{
    public CreateRatingRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment must not exceed 2000 characters.")
            .When(x => x.Comment is not null);
    }
}
