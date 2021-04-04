using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class CheckOrganizeCodeCkUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string organizeCodeCk = (string)value;

            if (String.IsNullOrEmpty(organizeCodeCk))
            {
                return ValidationResult.Success;
            }

            var _mariaDb = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var organize = _mariaDb.Organizes.FirstOrDefault(x => x.OrganizeCodeCk == organizeCodeCk);
            if (organize != null)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"OrganizeCodeCKAlreadyExists";
        }
    }
}
