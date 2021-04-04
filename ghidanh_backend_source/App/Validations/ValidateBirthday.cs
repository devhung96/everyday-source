using System;
using System.ComponentModel.DataAnnotations;

namespace Project.App.Validations
{
    public class ValidateBirthdayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime? birthday = (DateTime)value;
            if (birthday == null)
            {
                return ValidationResult.Success;
            }
            if (birthday.Value.Date >= DateTime.Today.AddYears(-3).Date)
                return new ValidationResult("BirthdayInValid");
            return ValidationResult.Success;
        }
    }
}