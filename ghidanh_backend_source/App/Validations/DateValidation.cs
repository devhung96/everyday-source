using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string date = value.ToString();
            try
            {
                DateTime.Parse(date);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("Date is not correct format");
            }
        }
    }
}
