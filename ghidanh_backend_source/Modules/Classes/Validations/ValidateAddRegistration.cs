using Project.Modules.Students.Requests;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Classes.Validations
{
    public class ValidateAddRegistrationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AddRegistration registration = value as AddRegistration;
            if (string.IsNullOrEmpty(registration.StudentId) && string.IsNullOrEmpty(registration.Password))
            {
                return new ValidationResult("PasswordRequied");
            }
            return ValidationResult.Success;
        }
    }
}
