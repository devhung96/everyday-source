using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validation
{
    public class ValidChangePasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ChangePasswordRequest change = (ChangePasswordRequest)value;
            if (change.newPassword.Equals(change.confirmPassword))
                return ValidationResult.Success;
            return new ValidationResult("Mật khẩu không khớp");
        }
    }
}
