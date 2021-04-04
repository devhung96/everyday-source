using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Schedules.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Validations
{
    public class GetScheduleValidation : AbstractValidator<GetScheduleRequest>
    {
        public GetScheduleValidation()
        {
            RuleFor(x => x.DateFrom)
                        .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                        .When(x => !string.IsNullOrEmpty(x.DateFrom))
                        .WithName("DateFrom");

            RuleFor(x => x.DateTo)
                        .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                        .When(x => !string.IsNullOrEmpty(x.DateTo))
                        .WithName("DateTo");
        }
    }
}
