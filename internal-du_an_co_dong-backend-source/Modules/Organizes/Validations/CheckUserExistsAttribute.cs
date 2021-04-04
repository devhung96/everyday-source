using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class CheckUserExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string userId = (string)value;

            if (String.IsNullOrEmpty(userId))
            {
                return ValidationResult.Success;
            }
            
            var _mariaDb = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var user = _mariaDb.UserSupers.FirstOrDefault(x => x.UserSuperId == userId && x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED);
            if (user is null)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"AccountNotFound";
        }
    }
}
