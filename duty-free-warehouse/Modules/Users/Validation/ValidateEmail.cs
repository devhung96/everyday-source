using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Project.Modules.Users.Validations
{
    public class ValidationEmailAttribute : ValidationAttribute
    {
        private const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value as string;
            if (string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success;
            }

            Regex regex = new Regex(Pattern);
            Regex mail = regex;
            return !mail.IsMatch(email) ? new ValidationResult("Email không hợp lệ.") : ValidationResult.Success;
        }
    }
}
