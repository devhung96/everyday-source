using Project.Modules.Users.Requests;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Users.Validations
{
    public class ValidateForgotPasswordRequest : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ForgotPasswordRequest request = value as ForgotPasswordRequest;
            if (!(request.PasswordNew.Equals(request.ConfirmPassword)))
                return new ValidationResult("ConfirmPasswordIsDifferentPassword");
            return ValidationResult.Success;
        }
    }
}
