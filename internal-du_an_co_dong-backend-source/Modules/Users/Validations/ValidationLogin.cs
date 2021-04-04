using Project.App.Database;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidationLogin: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var request = value as LoginRequest;
            var mariaDbContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var user = mariaDbContext.Users.Where(m => m.IdentityCard.Equals(request.UserName)||m.PhoneNumber.Equals(request.UserName))
                .FirstOrDefault();
            if (user is null)
                return new ValidationResult("UserSupperNotExist");
            if (!(user.UserPassword.Equals((user.UserSalt+request.Password).HashPassword())))
                return new ValidationResult("LoginInformationIncorrect");
            return ValidationResult.Success;
        }
    }
}
