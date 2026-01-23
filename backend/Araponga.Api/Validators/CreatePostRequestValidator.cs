using Araponga.Api.Contracts.Feed;
using Araponga.Domain.Feed;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Content)
            .NotEmptyWithMaxLength(4000);

        RuleFor(x => x.Type)
            .Must(type => !string.IsNullOrWhiteSpace(type) && Enum.TryParse<PostType>(type, true, out _))
            .WithMessage("Invalid post type. Valid values: General, Alert, Event.");

        RuleFor(x => x.Visibility)
            .Must(visibility => !string.IsNullOrWhiteSpace(visibility) && Enum.TryParse<PostVisibility>(visibility, true, out _))
            .WithMessage("Invalid visibility setting. Valid values: Public, ResidentsOnly.");

        RuleFor(x => x.GeoAnchors)
            .Must(anchors => anchors == null || anchors.Count <= 50)
            .WithMessage("Maximum 50 geo anchors allowed.");

        When(x => x.GeoAnchors != null, () =>
        {
            RuleForEach(x => x.GeoAnchors)
                .ChildRules(anchor =>
                {
                    anchor.RuleFor(a => a.Latitude)
                        .Latitude();

                    anchor.RuleFor(a => a.Longitude)
                        .Longitude();
                });
        });

        RuleFor(x => x.MediaIds)
            .Must(mediaIds => mediaIds == null || mediaIds.Count <= 10)
            .WithMessage("Maximum 10 media items allowed per post.");

        When(x => x.MediaIds != null, () =>
        {
            RuleFor(x => x.MediaIds!)
                .Must(mediaIds => mediaIds.All(id => id != Guid.Empty))
                .WithMessage("MediaIds cannot contain empty GUIDs.");

            RuleFor(x => x.MediaIds!)
                .Must(mediaIds => mediaIds.Distinct().Count() == mediaIds.Count)
                .WithMessage("MediaIds cannot contain duplicate values.");
        });
    }
}
