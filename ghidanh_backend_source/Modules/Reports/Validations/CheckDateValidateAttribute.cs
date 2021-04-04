using Project.Modules.Reports.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Validations
{
    public class CheckDateValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string date = value as string;
            if (String.IsNullOrEmpty(date))
            {
                return ValidationResult.Success;
            }

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                return new ValidationResult("IncorrectFormat:yyyy-MM-dd");
            }
            return ValidationResult.Success;
        }

    }
}
