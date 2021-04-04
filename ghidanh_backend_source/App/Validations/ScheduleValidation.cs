using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.App.Validations
{
    public class ScheduleValidation : ValidationAttribute
    {
        private class ScheduleInWeek
        {
            public TimeSpan TimeStart;
            public TimeSpan TimeEnd;
            public string WeekdaysCode;

            public ScheduleInWeek(string timeStart, string timeEnd, string weekdaysCode)
            {
                TimeStart = TimeSpan.Parse(timeStart);
                TimeEnd = TimeSpan.Parse(timeEnd);
                WeekdaysCode = weekdaysCode;
            }
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //List<Schedule> schedules = (List<Schedule>)value;
            //List<ScheduleInWeek> scheduleInWeeks = new List<ScheduleInWeek>();
            //foreach (Schedule sche in schedules)
            //{
            //    foreach (StudyTime studyTime in sche.StudyTimes)
            //    {
            //        try
            //        {
            //            ScheduleInWeek scheduleInWeek = new ScheduleInWeek(studyTime.TimeStart, studyTime.TimeEnd, sche.WeekDaysCode);
            //            if (CheckScheduleIsNotConflictInList(scheduleInWeeks, scheduleInWeek))
            //            {
            //                scheduleInWeeks.Add(scheduleInWeek);
            //            }
            //            else
            //            {
            //                return new ValidationResult("Contain class schedule is conflict");
            //            }
            //        }
            //        catch
            //        {
            //            return new ValidationResult("Format time is not correct");
            //        }
            //    }
            //}
            return ValidationResult.Success;
        }
        private bool CheckScheduleIsNotConflictInList(List<ScheduleInWeek> scheduleInWeeks, ScheduleInWeek schedule)
        {
            if (scheduleInWeeks.Count == 0)
            {
                return true;
            }

            ScheduleInWeek classSchedule = scheduleInWeeks.FirstOrDefault(c => c.WeekdaysCode.Equals(schedule.WeekdaysCode) &&
            (
            (
            //new Schedule: time start, time end new Schedule between time start, time end of Exist Schedule
                ((c.TimeStart <= schedule.TimeStart) && (schedule.TimeStart < c.TimeEnd)) ||
                ((c.TimeStart < schedule.TimeEnd) && (schedule.TimeEnd <= c.TimeEnd))
            ) ||
            (
            //new Schedule: time start, time end of Exist Schedule is between time start, time end new schedule
                ((schedule.TimeStart <= c.TimeStart) && (c.TimeStart < schedule.TimeEnd)) ||
                ((c.TimeEnd > schedule.TimeStart) && (c.TimeEnd <= schedule.TimeEnd))
            )
            ));
            if (classSchedule == null)
            {
                return true;
            }
            return false;
        }
    }
}
