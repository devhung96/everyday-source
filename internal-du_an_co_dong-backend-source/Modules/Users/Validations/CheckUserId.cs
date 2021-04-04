using Project.App.Database;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class CheckUserId: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string userId = value as string;
            var mariaDbContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));

            User user = mariaDbContext.Users
                                        .Where(m => m.UserId.Equals(userId))
                                        .FirstOrDefault();
            if (user is null)
                return new ValidationResult("NotExist");

            return ValidationResult.Success;
        }
    }
}
