using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class WeekdaysValidation : ValidationAttribute
    {
        private string[] listWeekDaysCode = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string weekdays = value.ToString();
            if (!listWeekDaysCode.Contains(weekdays))
            {
                return new ValidationResult("Weekday is not correct");
            }
            return ValidationResult.Success;
        }
    }
}
