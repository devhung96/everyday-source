using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class ExistClassIdValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string classIds = (string)value;
            MariaDBContext dBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            if (!dBContext.Classes.Any(c => c.ClassId.Equals((string)value)))
            {
                return new ValidationResult("Class is not exit");
            }
            return ValidationResult.Success;
        }
    }
}
