using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Project.App.Validations
{
    public class ValidationPhoneNumberAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            string phoneNumber = value as string;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return ValidationResult.Success;
            }
            var phone = new Regex(@"(0)+([0-9]{9})");
            var phone84 = new Regex(@"([+])+(84)+([0-9]{9})");
            if (phone84.IsMatch(phoneNumber))
            {
                return ValidationResult.Success;
            }
            if (phone.IsMatch(phoneNumber))
            {
               return ValidationResult.Success;
            }
            return new ValidationResult("PhoneNumberInValid");

        }
    }
}
