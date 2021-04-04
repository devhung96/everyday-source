using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class ExampleValidator
    {
        //public class StoreScheduleValidation : AbstractValidator<StoreScheduleRequest>
        //{
        //    public StoreScheduleValidation()
        //    {
        //        RuleFor(x => x.ScheduleName)
        //           .NotNull().WithMessage("{PropertyName}IsNotNull")
        //           .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
        //           .WithName("ScheduleName");

        //        RuleFor(x => x.ScheduleDateStart)
        //            .Cascade(CascadeMode.Stop)
        //            .NotNull().WithMessage("{PropertyName}IsNotNull")
        //            .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
        //            .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
        //            .Must((item, scheduleDateStart) => FluentValidationExtensions.GreateThanDateTime(scheduleDateStart, item.ScheduleDateEnd, format: "yyyy-MM-dd HH:mm:ss", repeatType: item.RepeatType)).WithMessage("ScheduleDateEndGreateThanScheduleDateStart")
        //            .WithName("ScheduleDateStart");


        //        RuleFor(x => x.ScheduleDateEnd)
        //            .Cascade(CascadeMode.Stop)
        //            .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
        //            .WithName("ScheduleDateEnd");

        //        RuleFor(x => x.ScheduleTimeStart)
        //           .Cascade(CascadeMode.Stop)
        //           .NotNull().WithMessage("{PropertyName}IsNotNull")
        //           .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
        //           .CheckTimeSpan("c")
        //           .Must((item, scheduleTimeStart) => FluentValidationExtensions.GreateThanTimeSpan(scheduleTimeStart, item.ScheduleTimeEnd, "c")).WithMessage("ScheduleTimeEndGreateThanScheduleTimeStart")
        //           .WithName("ScheduleTimeStart");


        //        RuleFor(x => x.ScheduleTimeEnd)
        //           .Cascade(CascadeMode.Stop)
        //           .NotNull().WithMessage("{PropertyName}IsNotNull")
        //           .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
        //           .CheckTimeSpan("c")
        //           .WithName("ScheduleTimeEnd");


        //        RuleFor(x => x.SchedulePriority)
        //          .Cascade(CascadeMode.Stop)
        //          .NotNull().WithMessage("{PropertyName}IsNotNull")
        //          .Must(x => x >= 1 && x <= 20).WithMessage("{PropertyName}GreaterThan0AndLessThan21")
        //          .WithName("SchedulePriority");

        //        RuleFor(x => x.ScheduleValues)
        //          .Cascade(CascadeMode.Stop)
        //          .NotNull().WithMessage("{PropertyName}IsNotNull")
        //          .Must((item, scheduleValues) => FluentValidationExtensions.CheckScheduleByScheduleType(scheduleValues, item.RepeatType)).WithMessage("{PropertyName}Invalid")
        //          .WithName("ScheduleValues");

        //        RuleFor(x => x.KeyCode)
        //          .Cascade(CascadeMode.Stop)
        //          .NotNull().WithMessage("{PropertyName}IsNotNull")
        //          .Must((item, keyCode) => FluentValidationExtensions.CheckKeyCodesByScheduleType(keyCode, item.ScheduleType)).WithMessage("{PropertyName}Invalid")
        //          .WithName("KeyCode");
        //    }
        //}
    }
}
