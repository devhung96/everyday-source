using FluentValidation;
using Microsoft.AspNetCore.Http;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Vatidations
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

        public static IRuleBuilderOptions<T, IFormFile> CheckExtensionsFile<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, string[] _extensions)
        {
            return ruleBuilder
                       .Must(val => val.AllowedExtensions(_extensions))
                       .WithMessage("ExtensionIsNotAllowed");
        }

        public static IRuleBuilderOptions<T, IFormFile> CheckMaxFileSizeFile<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, int maxFileSize)
        {
            return ruleBuilder
                       .Must(val => val.MaxFileSize(maxFileSize))
                       .WithMessage($"MaximumAllowedFileSizeIs{maxFileSize}Bytes");
        }

        public static IRuleBuilderOptions<T, long> CheckMaxFileSizeFile<T>(this IRuleBuilder<T, long> ruleBuilder, int maxFileSize)
        {
            return ruleBuilder
                       .Must(val => val <= maxFileSize)
                       .WithMessage($"MaximumAllowedFileSizeIs{maxFileSize}Bytes");
        }


        public static IRuleBuilderOptions<T, int?> CheckGender<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                       .Must(val => val.GenderValidate())
                       .WithMessage($"UserGenderInvalid1");
        }

        public static bool GreaterThanTimeSpan(this string strTimeBegin, string strTimeEnd, string format = "c")
        {
            if (String.IsNullOrEmpty(format)) return false;
            if (String.IsNullOrEmpty(strTimeBegin) && String.IsNullOrEmpty(strTimeEnd)) return true;
            TimeSpan timeBegin;
            TimeSpan timeEnd;
            if (TimeSpan.TryParseExact(strTimeBegin, format, CultureInfo.CurrentCulture, out timeBegin) && TimeSpan.TryParseExact(strTimeEnd, format, CultureInfo.CurrentCulture, out timeEnd))
            {
                return timeEnd > timeBegin;
            }
            return false;
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
            if(String.IsNullOrEmpty(strTimeSpan)) return true;
            return TimeSpan.TryParseExact(strTimeSpan, format, CultureInfo.CurrentCulture, out _);
        }

        public static bool GenderValidate(this int? input)
        {
            if (!input.HasValue) return true;
            if (input.Value == 1 || input.Value == 0) return true;
            return false;
        }

        public static bool Test(this string input)
        {
            var x = input;
            return true;
        }


        public static bool AllowedExtensions(this IFormFile file , string[] _extensions)
        {
            if (file is null) return true;
            if(file.Length <= 0 ) return true;
            string extension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(extension.ToLower()))
            {
                return false;
            }
            return true;
        }

        public static bool MaxFileSize(this IFormFile file, int maxFileSize)
        {
            if (file != null)
            {
                if (file.Length > maxFileSize)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckStringDateTime(this string strInput, string format)
        {
            if (String.IsNullOrEmpty(format)) return false;
            return DateTime.TryParseExact(strInput, format: format, provider: null, style: DateTimeStyles.None, out _);

        }


        public static bool GreateThanDateTime(this DateTime? dateTimeStart, DateTime? dateTimeEnd, ScheduleRepeatType? repeatType = ScheduleRepeatType.Daily)
        {
            if (repeatType is null) return true;
            if (dateTimeStart is null && dateTimeEnd is null) return true;
            if (dateTimeStart is null ) return false;
            if (dateTimeEnd is null) return false;
          
            if (repeatType == ScheduleRepeatType.NoRepeat)
            {
                return dateTimeStart == dateTimeEnd;
            }
            else
            {
                return dateTimeEnd > dateTimeStart;

            }
            
        }

        public static bool GreateThanStrDateTime(this string strDateTimeStart, string strDateTimeEnd, string format, ScheduleRepeatType repeatType = ScheduleRepeatType.Daily)
        {
            if (String.IsNullOrEmpty(format)) return false;
            if (String.IsNullOrEmpty(strDateTimeStart)) return false;
            if (String.IsNullOrEmpty(strDateTimeEnd) && repeatType != ScheduleRepeatType.NoRepeat) return true;
            DateTime timeBegin;
            DateTime timeEnd;
            if (DateTime.TryParseExact(strDateTimeStart, format: format, provider: null, style: DateTimeStyles.None, out timeBegin) && DateTime.TryParseExact(strDateTimeEnd, format: format, provider: null, style: DateTimeStyles.None, out timeEnd))
            {
                if (repeatType == ScheduleRepeatType.NoRepeat)
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


        public static bool GreateThanTimeSpan(this string strTimeBegin, string strTimeEnd, string format = "c")
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


        public static bool CheckScheduleByScheduleType(List<string> scheduleValues, ScheduleRepeatType? repeatType)
        {

            switch (repeatType)
            {
                case null:
                    {
                        return true;
                    }
                case ScheduleRepeatType.NoRepeat:
                    return true;
                case ScheduleRepeatType.Daily:
                    return true;
                case ScheduleRepeatType.Weekly:
                    {
                        if (scheduleValues.Count < 1) return false;
                        return !scheduleValues.Any(x => x != "Mon" && x != "Tue" && x != "Wed" && x != "Thu" && x != "Fri" && x != "Sat" && x != "Sun");
                    }
                case ScheduleRepeatType.Monthly:
                    {
                        try
                        {
                            if (scheduleValues.Count < 1) return false;
                            return scheduleValues.Any(x => int.Parse(x) >= 1 && int.Parse(x) <= 31);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                case ScheduleRepeatType.Yearly:
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
