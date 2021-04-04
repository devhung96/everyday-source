using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class CheckDateValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string date = value as string;
            if (String.IsNullOrEmpty(date))
            {
                //return new ValidationResult("Required"); 
                return ValidationResult.Success;
            }

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out _))
            {
                return new ValidationResult("IncorrectFormat:yyyy-MM-dd HH:mm");
            }
            return ValidationResult.Success;
        }

    }
}
