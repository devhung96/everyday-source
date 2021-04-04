using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Destroys.Validations
{
    public class ValidateStringDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null) return ValidationResult.Success;
            string dateInput = value.ToString();
            if(String.IsNullOrEmpty(dateInput))
            {
                return ValidationResult.Success;
            }
            try
            {
                _ = DateTime.ParseExact(dateInput, "dd/MM/yyyy", null);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("Ngày tháng không đúng định dạng. Định dạng đúng là dd/MM/yyyy");
            }
        }
    }
}
