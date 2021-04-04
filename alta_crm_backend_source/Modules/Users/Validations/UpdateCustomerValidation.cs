using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class UpdateCustomerValidation : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerValidation()
        {
            RuleFor(x => x.CustomerGender)
                .Cascade(CascadeMode.Stop)
                .CheckGender()
                .WithName("CustomerGender");

            RuleFor(x => x.CustomerFirstName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName}IsNotNull")
                .WithName("CustomerFirstName");

            RuleFor(x => x.CustomerLastName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName}IsNotNull")
                .WithName("CustomerLastName");

            RuleForEach(x => x.CustomerImages)
                   .Cascade(CascadeMode.Stop)
                   .SetValidator(x => new FileValidator())
                   .WithName("UserImages");
        }

    }
}
