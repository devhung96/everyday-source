using FluentValidation;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Validations;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Validations
{
    public class ValidationStoreSchedule : AbstractValidator<StoreScheduleRequest>
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        public ValidationStoreSchedule(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;

            RuleFor(x => x.DeviceIds)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .Must(IsExistDevices)
                .WithMessage("DeviceNotFound")
                .WithName("DeviceIds");

            RuleFor(x => x.PlaylistId)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .Must(IsExistPlaylist)
                .WithMessage("PlayListNotFound")
                .WithName("PlaylistId");

            RuleFor(x => x.UserId)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .Must((item, userId) => IsExistUser(userId)).WithMessage("{PropertyName}Invalid")
                .WithName("UserId");

            RuleFor(x => x.ScheduleDateTimeBeginStr)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
               .Must((item, scheduleDateStart) => GreateThanDateTime(scheduleDateStart, item.ScheduleDateTimeEndStr, format: "yyyy-MM-dd HH:mm:ss", repeatType: item.ScheduleRepeat)).WithMessage("ScheduleDateEndGreateThanScheduleDateStart")
               .WithName("ScheduleDateStart");

            RuleFor(x => x.ScheduleDateTimeEndStr)
                .Cascade(CascadeMode.Stop)
                .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                .WithName("ScheduleDateEnd");

            RuleFor(x => x.ScheduleTimeBeginStr)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .CheckTimeSpan("c")
               .Must((item, scheduleTimeStart) => GreateThanTimeSpan(scheduleTimeStart, item.ScheduleTimeEndStr, "c")).WithMessage("ScheduleTimeEndGreateThanScheduleTimeStart")
               .WithName("ScheduleTimeStart");


            RuleFor(x => x.ScheduleTimeEndStr)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName}IsNotNull")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .CheckTimeSpan("c")
               .WithName("ScheduleTimeEnd");

            RuleFor(x => x.ScheduleRepeatValues)
              .Cascade(CascadeMode.Stop)
              .NotNull().WithMessage("{PropertyName}IsNotNull")
              .Must((item, scheduleValues) => CheckScheduleByScheduleType(scheduleValues, item.ScheduleRepeat)).WithMessage("{PropertyName}Invalid")
              .WithName("ScheduleValues");

            RuleFor(x => x.ScheduleRepeatValueDetails)
              .Cascade(CascadeMode.Stop)
              .NotNull().WithMessage("{PropertyName}IsNotNull")
              .Must((item, scheduleRepeatValueDetails) => CheckSchedule(item.DeviceIds, item.ScheduleDateTimeBeginStr, item.ScheduleDateTimeEndStr, item.ScheduleTimeBeginStr, item.ScheduleTimeEndStr, item.ScheduleRepeat, scheduleRepeatValueDetails)).WithMessage("{PropertyName}Invalid")
              .WithName("ScheduleRepeatValueDetails");
        }

        private bool IsExistDevices(List<string> deviceIds)
        {
            return RepositoryWrapperMariaDB.Devices.FindByCondition(x => deviceIds.Contains(x.DeviceId)).Any();
        }

        private bool IsExistPlaylist(string playlistId)
        {
            return RepositoryWrapperMariaDB.PlayLists.FirstOrDefault(x => x.PlayListId.Equals(playlistId)) != null;
        }

        private bool GreateThanDateTime(string strDateTimeStart, string strDateTimeEnd, string format, ScheduleRepeatEnum repeatType = ScheduleRepeatEnum.Daily)
        {
            if (String.IsNullOrEmpty(format)) return false;
            if (String.IsNullOrEmpty(strDateTimeStart)) return false;
            if (String.IsNullOrEmpty(strDateTimeEnd) && repeatType != ScheduleRepeatEnum.Non) return true;
            DateTime timeBegin;
            DateTime timeEnd;
            if (DateTime.TryParseExact(strDateTimeStart, format: format, provider: null, style: DateTimeStyles.None, out timeBegin) && DateTime.TryParseExact(strDateTimeEnd, format: format, provider: null, style: DateTimeStyles.None, out timeEnd))
            {
                if (repeatType == ScheduleRepeatEnum.Non)
                {
                    return timeEnd.Date == timeBegin.Date;
                }
                else
                {
                    return timeEnd > timeBegin;

                }
            }
            return false;
        }

        private bool GreateThanTimeSpan(string strTimeBegin, string strTimeEnd, string format = "c")
        {
            if (String.IsNullOrEmpty(strTimeBegin) || String.IsNullOrEmpty(format) || String.IsNullOrEmpty(strTimeEnd)) return false;
            TimeSpan timeBegin;
            TimeSpan timeEnd;
            if (TimeSpan.TryParseExact(strTimeBegin, format, CultureInfo.CurrentCulture, out timeBegin) && TimeSpan.TryParseExact(strTimeEnd, format, CultureInfo.CurrentCulture, out timeEnd))
            {
                return timeEnd > timeBegin;
            }
            return false;
        }

        private bool CheckScheduleByScheduleType(List<string> scheduleValues, ScheduleRepeatEnum repeatType)
        {
            switch (repeatType)
            {
                case ScheduleRepeatEnum.Non:
                    return true;

                case ScheduleRepeatEnum.Daily:
                    return true;

                case ScheduleRepeatEnum.Weekly:
                    {
                        if (scheduleValues.Count < 1) return false;
                        return !scheduleValues.Any(x => x != "Mon" && x != "Tue" && x != "Wed" && x != "Thu" && x != "Fri" && x != "Sat" && x != "Sun");
                    }

                case ScheduleRepeatEnum.Monthly:
                    {
                        if (scheduleValues.Count < 1) return false;
                        return scheduleValues.Any(x => int.Parse(x) >= 1 && int.Parse(x) <= 31);
                    }

                case ScheduleRepeatEnum.Yearly:
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

        private bool IsExistUser(string userId)
        {
            return RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId)) != null;
        }

        private bool CheckSchedule(List<string> deviceIds, string dateBegin, string dateEnd, string timeBegin, string timeEnd, ScheduleRepeatEnum scheduleRepeat, List<string> repeatValueDetail)
        {
            List<string> scheduleIds = RepositoryWrapperMariaDB.ScheduleDevices
                .FindByCondition(x => deviceIds.Contains(x.DeviceId)).Select(x => x.ScheduleId)
                .ToList(); // Filter ScheduleId belong to Devices minus the present one

            List<Schedule> schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => scheduleIds.Contains(x.ScheduleId)).ToList(); // Get All Schedule to check duration

            if (schedules != null && schedules.Count > 0)
            {
                DateTime dateTimeBegin = dateBegin.ParseStringGMTToDatime();
                DateTime dateTimeEnd = dateEnd.ParseStringGMTToDatime();
                TimeSpan timeBeginData;
                TimeSpan timeEndData;
                TimeSpan.TryParseExact(timeBegin, "c", CultureInfo.CurrentCulture, out timeBeginData);
                TimeSpan.TryParseExact(timeEnd, "c", CultureInfo.CurrentCulture, out timeEndData);
                schedules = schedules.Where(x => (dateTimeBegin >= x.ScheduleDateTimeBegin && dateTimeBegin <= x.ScheduleDateTimeEnd) || (dateTimeEnd >= x.ScheduleDateTimeBegin && dateTimeEnd <= x.ScheduleDateTimeEnd)).ToList();

                foreach (var schedule in schedules)
                {

                    if (scheduleRepeat == ScheduleRepeatEnum.Non || scheduleRepeat == ScheduleRepeatEnum.Daily) // client check time
                    {
                        if ((timeBeginData >= schedule.ScheduleTimeBegin && timeBeginData <= schedule.ScheduleTimeEnd) || (timeEndData >= schedule.ScheduleTimeBegin && timeEndData <= schedule.ScheduleTimeEnd))
                        {
                            return false;
                        }
                    }

                    foreach (var repeatValue in repeatValueDetail)
                    {

                        if (schedule.ScheduleRepeat == ScheduleRepeatEnum.Non || schedule.ScheduleRepeat == ScheduleRepeatEnum.Daily) //server check time
                        {
                            if ((timeBeginData >= schedule.ScheduleTimeBegin && timeBeginData <= schedule.ScheduleTimeEnd) || (timeEndData >= schedule.ScheduleTimeBegin && timeEndData <= schedule.ScheduleTimeEnd))
                            {
                                return false;
                            }
                        }

                        List<string> scheduleRepeatValues = JsonConvert.DeserializeObject<List<string>>(schedule.ScheduleRepeatValueDetail);

                        if (scheduleRepeatValues.Contains(repeatValue))
                        {
                            if ((timeBeginData >= schedule.ScheduleTimeBegin && timeBeginData <= schedule.ScheduleTimeEnd) || (timeEndData >= schedule.ScheduleTimeBegin && timeEndData <= schedule.ScheduleTimeEnd))
                            {
                                return false;
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            return true;
        }

    }
}
