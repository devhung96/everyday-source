using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tickets.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Validations
{
    public class AddTagValidator: AbstractValidator<AddTagRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public AddTagValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
            RuleFor(x => x.TicketTypeId).Must(CheckTicketType)
                                        .When(x => !string.IsNullOrEmpty(x.TicketTypeId))
                                        .WithMessage("TicketTypeIdNotExist");
          
            RuleFor(x => x).Must(x => CheckDateStartDateStop(x.TagDateStart.Value, x.TagDateEnd.Value))
                           .When(x=>x.TagDateStart.HasValue && x.TagDateEnd.HasValue)
                           .WithMessage("DateEndSmallerDateStart");
            
            RuleFor(x => x).Must(x => CheckScheduleByScheduleType(x.ListRepeatValue, x.TagRepeat.Value))
                           .When(x=>x.ListRepeatValue!=null && x.TagRepeat.HasValue )
                           .WithMessage("ListValueInValid");
            
        }
        public  bool CheckScheduleByScheduleType(List<string> scheduleValues, RepeatType repeatType)
        {

            switch (repeatType)
            {
                case RepeatType.NonRepeat:
                    return true;
                case RepeatType.RepeatDay:
                    return true;
                case RepeatType.RepeatWeek:
                    {
                        if (scheduleValues.Count < 1) return false;
                        return !scheduleValues.Any(x => x != "Mon" && x != "Tue" && x != "Wed" && x != "Thu" && x != "Fri" && x != "Sat" && x != "Sun");
                    }
                case RepeatType.RepeatMonth:
                    {
                        if (scheduleValues.Count < 1) return false;
                        return scheduleValues.Any(x => int.Parse(x) >= 1 && int.Parse(x) <= 31);
                    }
                case RepeatType.RepeatYear:
                    {
                        if (scheduleValues.Count < 1) return false;
                        foreach (var item in scheduleValues)
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
            if(dateStart>dateStop)
            {
                return false;
            }
            return true;
        }
        public bool CheckTicketType(string TicketTypeId)
        {
            TicketType ticketType = repository.TicketTypes.GetById(TicketTypeId);
            if(ticketType is null)
            {
                return false;
            }
            return true;
        }
    }
}
