using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class CheckOrganizeExists : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string organizeId = (string)value;

            if (String.IsNullOrEmpty(organizeId))
            {
                return ValidationResult.Success;
            }

            var _mariaDb = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            
            var organize = _mariaDb.Organizes.FirstOrDefault(x => x.OrganizeId == organizeId);
            if (organize is null)
            {
                return new ValidationResult(GetErrorMessage());
            }
           
            return ValidationResult.Success;

        }

        public string GetErrorMessage()
        {
            return $"NoOrganizationFound";
        }
    }
}
