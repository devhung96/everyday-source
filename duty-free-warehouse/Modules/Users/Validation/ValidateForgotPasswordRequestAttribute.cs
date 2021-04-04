using Project.App.Helpers;
using Project.Modules.Users.Requests;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Users.Validations
{
    public class ValidateForgotPasswordRequestAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ForgotPasswordRequest request = value as ForgotPasswordRequest;
            if (!request.PasswordNew.Equals(request.ConfirmPassword))
                return new ValidationResult("PasswordNewNotMatchConfirmPassword");
            return ValidationResult.Success;
        }
    }
}
