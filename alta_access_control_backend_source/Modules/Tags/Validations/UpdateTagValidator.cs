using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Validations
{
    public class UpdateTagValidator: AbstractValidator<UpdateTagRequest>
    {

        private readonly IRepositoryWrapperMariaDB repository;
        public UpdateTagValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;

            RuleFor(x => x).Must(x => CheckDateStartDateStop(x.TagDateStart.Value, x.TagDateEnd.Value)).WithMessage("DateEndSmallerDateStart");
            RuleFor(x => x).Must(x => CheckScheduleByScheduleType(x.ListRepeatValue, x.TagRepeat.Value)).When(x=>x.TagRepeat.HasValue).WithMessage("TagValueInValid");

        }
        public bool CheckScheduleByScheduleType(List<string> values, RepeatType repeatType)
        {
            switch (repeatType)
            {
                case RepeatType.NonRepeat:
                    return true;
                case RepeatType.RepeatDay:
                    return true;
                case RepeatType.RepeatWeek:
                    {
                        if (values.Count < 1) return false;
                        return !values.Any(x => x != "Mon" && x != "Tue" && x != "Wed" && x != "Thu" && x != "Fri" && x != "Sat" && x != "Sun");
                    }
                case RepeatType.RepeatMonth:
                    {
                        if (values.Count < 1) return false;
                        return values.Any(x => int.Parse(x) >= 1 && int.Parse(x) <= 31);
                    }
                case RepeatType.RepeatYear:
                    {
                        if (values.Count < 1) return false;
                        foreach (var item in values)
                        {
                            string[] result = item.Split("/");
                            if (result.Length != 2) return false;
                            int d = int.Parse(result.FirstOrDefault());
                            int m = int.Parse(result.LastOrDefault());
                            if (d < 1 || d > 31) return false;
                            if (m < 1 || m > 12) return false;
                        }
                        return true;

                    }
                default:
                    return false;
            }


        }
        public bool CheckDateStartDateStop(DateTime dateStart, DateTime dateStop)
        {
            if (dateStart > dateStop)
            {
                return false;
            }
            return true;
        }
    }
}
