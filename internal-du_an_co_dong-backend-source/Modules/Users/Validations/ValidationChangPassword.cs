using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidationChangPassword : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = value as ChangePasswordRequest;

            if (!(input.PasswordNew.Equals(input.ConfirmPassword)))
                return new ValidationResult("ConfirmAndDifferentPasswordNew");

            return ValidationResult.Success;
        }
    }
}
