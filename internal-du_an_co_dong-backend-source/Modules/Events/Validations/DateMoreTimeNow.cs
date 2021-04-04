using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class DateMoreTimeNow : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;
            var currentValue = ((DateTime)value).ToString("yyyy-MM-dd HH:mm");
            var dateTimeNow = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm");

            if (String.Compare(dateTimeNow, currentValue)>= 0)
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "TheStartTimeIsGreaterThanTheCurrentTime";
        }



    }
}
