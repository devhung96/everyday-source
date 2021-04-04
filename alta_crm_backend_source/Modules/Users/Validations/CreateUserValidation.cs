using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class CreateUserValidation : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidation()
        {
            RuleFor(x => x.GroupId)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .WithName("GroupId");


            RuleFor(x => x.UserEmail)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .WithName("UserEmail");


            RuleFor(x => x.UserGender)
              .Cascade(CascadeMode.Stop)
              .CheckGender()
              .WithName("UserGender");

            RuleForEach(x => x.UserImages)
                .Cascade(CascadeMode.Stop)
                .SetValidator(x=> new FileValidator())
                .WithName("UserImages");
        }
    }
}
