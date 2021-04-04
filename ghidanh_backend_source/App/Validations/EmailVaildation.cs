using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Project.App.Validations
{
    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value as string;
            if (String.IsNullOrEmpty(email))
                return ValidationResult.Success;
            Regex mail = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            if (!(mail.IsMatch(email)))
                return new ValidationResult("EmailInValid");
            return ValidationResult.Success;
        }
    }
    public class CodeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string code = value as string;
            if (String.IsNullOrEmpty(code))
                return ValidationResult.Success;
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            if (!(regex.IsMatch(code)))
                return new ValidationResult("CodeInValid");
            return ValidationResult.Success;
        }
    }
 
}
