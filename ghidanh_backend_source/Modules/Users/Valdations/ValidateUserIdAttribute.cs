using Project.App.Databases;
using Project.Modules.Users.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.Users.Valdations
{
    public class ValidateUserIdAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string userId = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            User user = mariaDB.Users.FirstOrDefault(m => m.UserId.Equals(userId));
            if(user is null)
            {
                return new ValidationResult("UserNotExist");
            }
            return ValidationResult.Success;
        }
    }
}
