using Project.App.Database;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidateEmailSystem : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string request = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));

            UserSuper user = mariaDB.UserSupers.Where(m => m.Email.Equals(request))
                                        .FirstOrDefault();
            if (!(user is null))
                return new ValidationResult("AreadlyExist");
            return ValidationResult.Success;
        }
    }
}