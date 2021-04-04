using FluentValidation;
using Microsoft.AspNetCore.Http;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class LoginFaceValidation : AbstractValidator<LoginFaceRequest>
    {
        public LoginFaceValidation()
        {
            RuleFor(x => x.ModeId)
                .NotNull()
                .WithMessage("ModeIdNotNull")
                .NotEmpty()
                .WithMessage("ModeIdNotEmpty")
                .Must(CheckMode)
                .WithMessage("ModeIdInvalid");

            RuleFor(x => x)
                .Must(x => (CheckModeValue(x.ModeId, x.CardId, x.Image)))
                .WithMessage("CardIdOrImageNull");
        }

        private bool CheckMode(string modeId)
        {
            return (modeId.Equals("Card_ID") || modeId.Equals("Face_ID"));
        }

        private bool CheckModeValue(string modeId, string cardId, IFormFile formFile)
        {
            switch (modeId)
            {
                case "Face_ID":
                    {
                        return formFile != null;
                    }
                case "Card_ID":
                    {
                        return !(string.IsNullOrEmpty(cardId));
                    }
                default: return false;
            }
        }

    }
}
