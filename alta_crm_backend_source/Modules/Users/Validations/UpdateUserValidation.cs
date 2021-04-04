using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class UpdateUserValidation : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidation()
        {


            RuleFor(x => x.UserGender)
             .Cascade(CascadeMode.Stop)
             .CheckGender()
             .WithName("UserGender");


            RuleForEach(x => x.UserImages)
               .Cascade(CascadeMode.Stop)
               .SetValidator(x => new FileValidator())
               .WithName("UserImages");

            //RuleFor(x => x.UserImage)
            // .Cascade(CascadeMode.Stop)
            // .CheckExtensionsFile(_extensions: new[] { ".jpg", ".png" })
            // .CheckMaxFileSizeFile(5 * 1024 * 1024)
            // .WithName("UserImage");
        }

    }
}
