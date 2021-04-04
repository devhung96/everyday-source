using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class TimeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string time = value.ToString();
            try
            {
                TimeSpan timeSpan = TimeSpan.Parse(time);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("TimeIsNotCorrectFormat");
            }
        }
    }
}
