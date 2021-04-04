using Microsoft.AspNetCore.Http.Connections;
using Project.App.Databases;
using Project.Modules.Groups.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.Users.Valdations
{
    public class ValidationPermissionCodesAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> permisisonCodes = value as List<string>;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            foreach(string permissionCode in permisisonCodes)
            {
                Permission permission = mariaDB.Permissions.FirstOrDefault(m => m.PermissionCode.Equals(permissionCode));
                if(permisisonCodes is null)
                {
                    return new ValidationResult("PermissionNotExist: " + permissionCode);
                }
            }
            return ValidationResult.Success;
        }
    }
}
