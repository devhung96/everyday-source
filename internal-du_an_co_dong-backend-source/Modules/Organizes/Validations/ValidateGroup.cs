using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class ValidateGroup:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            int groupId = (int)value;
            MariaDBContext maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var group = maria.Groups.Find(groupId);
            if(group is null)
            {
                return new ValidationResult("NotExist");
            }
            return ValidationResult.Success;
        }
    }
}
