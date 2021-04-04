using Project.App.Database;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidateEmailAdmin : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value as string;

            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));

            List<User> users = mariaDB.Users.Where(m => m.ShareholderCode == null&& m.UserEmail.Equals(email)).ToList();
            if (users.Count()>0 )
                return new ValidationResult("AreadlyExist");
            return ValidationResult.Success;
            
        }
    }
}
