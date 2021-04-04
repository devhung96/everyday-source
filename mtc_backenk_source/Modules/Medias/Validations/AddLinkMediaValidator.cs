using FluentValidation;
using Project.Modules.Medias.Requests;

namespace Project.Modules.Medias.Validations
{
    public class AddLinkMediaValidator : AbstractValidator<AddLinkMediaRequest>
    {
        public AddLinkMediaValidator()
        {
            RuleFor(x => x.MediaUrl)
                .NotNull().WithMessage("MediaUrlNotNull")
                .NotEmpty().WithMessage("MediaUrlNotEmpty");

            RuleFor(x => x.MediaName)
                .NotNull().WithMessage("MediaNameNotNull")
                .NotEmpty().WithMessage("MediaNameNotEmpty");
        }
    }
}
