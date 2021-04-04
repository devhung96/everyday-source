using Project.App.Database;
using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissonUsers.Validations
{
    public class ValidatePermissionCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string code = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Permissions.Entities.PermissionOrganize  permission = _maria.Permissions.Where(m => m.PermissionCode == code).FirstOrDefault();
            if (permission is null)
                return new ValidationResult("NotExist");
            return ValidationResult.Success;
        }
    }
}
