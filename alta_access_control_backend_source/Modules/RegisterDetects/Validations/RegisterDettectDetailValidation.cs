using FluentValidation;
using Project.App.Validations;
using Project.Modules.RegisterDetects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Validations
{
    public class RegisterDettectDetailValidation : AbstractValidator<RegisterDettectDetailRequest>
    {
        public RegisterDettectDetailValidation()
        {
            RuleFor(x => x.RgDectectDetailTimeBegin)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .CheckTimeSpan("c")
               .Must((item, scheduleTimeStart) => FluentValidationExtensions.GreaterThanTimeSpan(scheduleTimeStart, item.RgDectectDetailTimeEnd, "c")).WithMessage("RgDectectDetailTimeBeginEndGreateThanRgDectectDetailTimeEnd")
               .WithName("RgDectectDetailTimeBegin");


            RuleFor(x => x.RgDectectDetailTimeEnd)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .CheckTimeSpan("c")
               .WithName("RgDectectDetailTimeEnd");


            RuleFor(x => x.RgDectectDetailDateBegin)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("{PropertyName}IsNotNull")
                 .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                 .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                 .Must((item, scheduleDateStart) => FluentValidationExtensions.GreaterThanDateTime(scheduleDateStart, item.RgDectectDetailDateEnd, format: "yyyy-MM-dd HH:mm:ss", repeatType: item.RgDectectDetailRepeat)).WithMessage("ScheduleDateEndGreateThanScheduleDateStart")
                 .WithName("ScheduleDateStart");


            RuleFor(x => x.RgDectectDetailDateEnd)
                .Cascade(CascadeMode.Stop)
                .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                .WithName("ScheduleDateEnd");

            RuleFor(x => x.RgDectectDetailRepeatValueData)
             .Cascade(CascadeMode.Stop)
             .Must((item, scheduleValues) => FluentValidationExtensions.CheckScheduleByScheduleType(scheduleValues, item.RgDectectDetailRepeat)).WithMessage("{PropertyName}Invalid")
             .WithName("RgDectectDetailRepeatValueData");




        }
    }
}
