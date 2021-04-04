using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Tags.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Validation
{
    public class InsertTagValidation : AbstractValidator<InsertTagRequest>
    {
        public InsertTagValidation()
        {
            RuleFor(x => x.TagName)
              .Cascade(CascadeMode.Stop)
              .NotNull().WithMessage("{PropertyName}IsNotNull")
              .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
              .WithName("TagName");


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


            RuleFor(x => x.TagTimeStart)
               .Cascade(CascadeMode.Stop)
               .CheckTimeSpan("c")
               .Must((item, tagTimeStart) => FluentValidationExtensions.GreaterThanTimeSpan(tagTimeStart, item.TagTimeEnd, "c")).WithMessage("TagTimeEndGreateThanTagTimeStart")
               .WithName("TagTimeStart");


            RuleFor(x => x.TagTimeEnd)
               .Cascade(CascadeMode.Stop)
               .CheckTimeSpan("c")
               .WithName("TagTimeEnd");



            RuleFor(x => x.TagDateEnd)
             .Cascade(CascadeMode.Stop)
             .Must((item, tagDateEnd) => FluentValidationExtensions.GreateThanDateTime(item.TagDateStart, tagDateEnd,item.TagRepeat)).WithMessage("TagDateEndGreateThanTagDateStart")
             .WithName("TagDateEnd");



            RuleFor(x => x.ListRepeatValue)
            .Cascade(CascadeMode.Stop)
            .Must((item, listRepeatValue) => FluentValidationExtensions.CheckScheduleByScheduleType(listRepeatValue, item.TagRepeat)).WithMessage("{PropertyName}Invalid")
            .WithName("ListRepeatValue");


        }
    }
}
