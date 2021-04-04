using Project.App.Databases;
using Project.Modules.Groups.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.Users.Valdations
{
    public class GroupValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string groupId = value as string;
            if (String.IsNullOrEmpty(groupId))
                return ValidationResult.Success;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Group group = mariaDB.Groups.FirstOrDefault(m => m.GroupId.Equals(groupId));
            if (group is null)
                return new ValidationResult("GroupIdNotExist");
            return ValidationResult.Success;
        }
    }
}
