using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Validations
{
    public class SubjectCodeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            string subjectCode = value.ToString();
            MariaDBContext mariaDBContext = validationContext.GetService<MariaDBContext>();
            if (mariaDBContext.Subjects.Any(s => s.SubjectCode.Equals(subjectCode)))
            {
                return new ValidationResult("This subject code is already exist!!");
            }
            return ValidationResult.Success;
        }
    }
}
