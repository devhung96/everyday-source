using Project.App.Database;
using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Permissions.Validations
{
    public class ValidatePermissionCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string code = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Entities.PermissionOrganize permission = _maria.Permissions.Where(m => m.PermissionCode == code).FirstOrDefault();
            if (permission!=null)
                return new ValidationResult("Faild");
            return ValidationResult.Success;
        }
    }
}
