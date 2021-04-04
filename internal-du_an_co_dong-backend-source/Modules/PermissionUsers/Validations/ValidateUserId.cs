using Project.App.Database;
using Project.Modules.Users.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.PermissonUsers.Validations
{
    public class ValidateUserId : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string code = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            User permission = _maria.Users.Where(m => m.UserId.Equals(code)).FirstOrDefault();
            if (permission is null)
                return new ValidationResult("UserNotExist");
            return ValidationResult.Success;
        }
    }

}

