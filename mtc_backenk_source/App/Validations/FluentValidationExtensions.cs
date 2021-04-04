using FluentValidation;
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
    }
}
