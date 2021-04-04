using FluentValidation;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.RegisterDetects.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> CheckDataTime<T>(this IRuleBuilder<T, string> ruleBuilder, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return ruleBuilder
                       .Must(val => val.CheckStringDateTime(format))
                       .WithMessage("{PropertyName}Invalid");
        }

        public static IRuleBuilderOptions<T, string> CheckTimeSpan<T>(this IRuleBuilder<T, string> ruleBuilder, string format = "c")
        {
            return ruleBuilder
                       .Must(val => val.CheckStringTimeSpan(format))
                       .WithMessage("{PropertyName}Invalid");
        }





        /// <summary>
        /// "c" ==> 00:00:00
        /// "g" ==> 3.17:25:30.5000000
        /// "G" ==> 0:18:30:00,0000000
        /// </summary>
        /// <param name="strTimeSpan"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool CheckStringTimeSpan(this string strTimeSpan, string format)
        {
            if (String.IsNullOrEmpty(format)) return false;
            return TimeSpan.TryParseExact(strTimeSpan, format, CultureInfo.CurrentCulture, out _);
        }


        public static bool CheckStringDateTime(this string strInput, string format)
        {
            if (String.IsNullOrEmpty(format)) return false;
            return DateTime.TryParseExact(strInput, format: format, provider: null, style: DateTimeStyles.None, out _);

        }

        public static bool GreaterThanTimeSpan(this string strTimeBegin, string strTimeEnd, string format = "c")
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

        public static bool GreaterThanDateTime(this string strDateTimeStart, string strDateTimeEnd, string format, RepeatType repeatType = RepeatType.RepeatDay)
        {
            if (String.IsNullOrEmpty(format)) return false;
            if (String.IsNullOrEmpty(strDateTimeStart)) return false;
            if (String.IsNullOrEmpty(strDateTimeEnd) && repeatType != RepeatType.NonRepeat) return true;
            DateTime timeBegin;
            DateTime timeEnd;
            if (DateTime.TryParseExact(strDateTimeStart, format: format, provider: null, style: DateTimeStyles.None, out timeBegin) && DateTime.TryParseExact(strDateTimeEnd, format: format, provider: null, style: DateTimeStyles.None, out timeEnd))
            {
                if (repeatType == RepeatType.NonRepeat)
                {
                    return timeEnd == timeBegin;
                }
                else
                {
                    return timeEnd > timeBegin;

                }
            }
            return false;
        }


        public static bool CheckScheduleByScheduleType(List<string> scheduleValues, RepeatType repeatType)
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


    }
}
