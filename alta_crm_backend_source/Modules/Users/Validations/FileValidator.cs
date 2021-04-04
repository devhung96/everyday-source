using FluentValidation;
using Microsoft.AspNetCore.Http;
using Project.App.Vatidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class FileValidator: AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(x => x.Length)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .CheckMaxFileSizeFile(5 * 1024 * 1024);

            RuleFor(x => x.ContentType)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("ExtensionIsNotAllowed");
        }
    }
}
