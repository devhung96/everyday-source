using FluentValidation;
using Project.Modules.RegisterDetects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Validations
{
    public class UnRegisterUserDetectValidation : AbstractValidator<UnRegisterUserDetectRequest>
    {
        public UnRegisterUserDetectValidation()
        {
            RuleFor(x => x.ModeId)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("{PropertyName}IsNotNull")
                   .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                   .WithName("ModeId");


            RuleFor(x => x.RgDectectKey)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("{PropertyName}IsNotNull")
                  .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                  .WithName("RgDectectKey");

            RuleFor(x => x.RgDectectUserId)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("{PropertyName}IsNotNull")
                  .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                  .WithName("RgDectectUserId");


            //RuleFor(x => x.TagId)
            //      .Cascade(CascadeMode.Stop)
            //      .NotNull().WithMessage("{PropertyName}IsNotNull")
            //      .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
            //      .WithName("TagId");


            RuleFor(x => x.TagCode)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("{PropertyName}IsNotNull")
                 .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                 .WithName("TagCode");


            RuleFor(x => x.TicketTypeId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName}IsNotNull")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .WithName("TicketTypeId");
        }
    }
}
