using Project.App.Databases;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.ClassSchedules.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class CheckScheduleValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MariaDBContext dBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));

            AddScheduleRequest request = value as AddScheduleRequest;

            List<ClassSchedule> schedules = dBContext.ClassSchedules
                .Where(x => x.LecturerId.Equals(request.LecturerId) || x.ClassId.Equals(request.ClassId))
                .ToList();

            long timeStart = TimeSpan.Parse(request.TimeStart).Ticks;
            long timeEnd = TimeSpan.Parse(request.TimeEnd).Ticks;
            DateTime dayStart = request.DateStart.GetValueOrDefault();
            DateTime dayEnd = request.DateEnd.GetValueOrDefault();

            ClassSchedule scheduleCheckDay = schedules.FirstOrDefault(
                x => (x.LecturerId.Equals(request.LecturerId) || x.ClassId.Equals(request.ClassId)) && (
                (dayStart >= x.DateStart && dayStart <= x.DateEnd) ||
                (dayEnd >= x.DateStart && dayEnd <= x.DateEnd) ||
                (dayStart <= x.DateStart && dayEnd >= x.DateEnd)));

            if (scheduleCheckDay != null)
            {
                List<ClassSchedule> scheduleChecks = schedules.Where(x =>
                    (x.LecturerId.Equals(request.LecturerId) || x.ClassId.Equals(request.ClassId)) &&
                    (
                        (dayStart >= x.DateStart && dayStart <= x.DateEnd) ||
                        (dayEnd >= x.DateStart && dayEnd <= x.DateEnd) ||
                        (dayStart <= x.DateStart && dayEnd >= x.DateEnd)
                    ) && (
                        (timeStart + 1 >= x.TimeStart && timeStart <= x.TimeEnd) ||
                        (timeEnd - 1 >= x.TimeStart && timeEnd <= x.TimeEnd) ||
                        (timeStart + 1 <= x.TimeStart && timeEnd >= x.TimeEnd)
                    )).ToList();

                foreach (var item in request.DayOfWeek)
                {
                    if(scheduleChecks.FirstOrDefault(x => x.DayOfWeek.Contains(item)) != null)
                    {
                        return new ValidationResult("ScheduleTimeLeturerDuplicate");
                    }    
                }
            }

            return ValidationResult.Success;
        }
    }
}
