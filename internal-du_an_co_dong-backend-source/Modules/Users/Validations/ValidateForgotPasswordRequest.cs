using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidateForgotPasswordRequest : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ForgotPasswordRequest request = value as ForgotPasswordRequest;
            if (!(request.PasswordNew.Equals(request.ConfirmPassword)))
                return new ValidationResult("ConfirmAndDifferentPasswordNew");
            return ValidationResult.Success;
        }
    }
}
