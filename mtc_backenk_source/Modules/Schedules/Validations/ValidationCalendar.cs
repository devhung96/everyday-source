using FluentValidation;
using Project.App.Validations;
using Project.Modules.Schedules.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Validations
{
    public class ValidationCalendar : AbstractValidator<CalendarRequest>
    {
        public ValidationCalendar()
        {
            RuleFor(x => x.DateFrom)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckDataTime(format: "yyyy-MM-dd")
                .WithName("DateFrom");

            RuleFor(x => x.DateTo)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckDataTime(format: "yyyy-MM-dd")
                .WithName("DateTo");
        }
    }
}
