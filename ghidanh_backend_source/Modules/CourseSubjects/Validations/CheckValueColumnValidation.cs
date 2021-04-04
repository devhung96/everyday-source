using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Validations
{
    public class CheckValueColumnValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int checkColumn = (int)value;
            if (checkColumn > 0)
            {
                return new ValidationResult("checkColumnMore0");
            }
            return ValidationResult.Success;
        }
    }
}
