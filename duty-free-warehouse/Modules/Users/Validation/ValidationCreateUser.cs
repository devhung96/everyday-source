using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validation
{
    public class ValidationCreateUserAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RequestCreateNewUser data = value as RequestCreateNewUser;
            if(String.IsNullOrEmpty(data.Password) || String.IsNullOrEmpty(data.ConfirmPassword))
                return ValidationResult.Success;
            if (!data.Password.Equals(data.ConfirmPassword))
                return new ValidationResult("Mã xác nhận mật khẩu không khớp.");
            return ValidationResult.Success;
        }

    }
}
