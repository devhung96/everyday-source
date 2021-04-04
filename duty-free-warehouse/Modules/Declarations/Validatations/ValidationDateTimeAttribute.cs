using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidationDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            string dateInput = value.ToString();
            if (String.IsNullOrEmpty(dateInput))
            {
                return ValidationResult.Success;
            }
            try
            {
                DateTime dateTime = DateTime.ParseExact(dateInput, "dd/MM/yyyy", null);
                Console.WriteLine(dateTime);
                return ValidationResult.Success;
            }
            catch(Exception ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }
}
