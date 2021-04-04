using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Validations
{
    public class CourseCodeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            string courseCode = value.ToString();
            MariaDBContext mariaDBContext = validationContext.GetService<MariaDBContext>();
            if (mariaDBContext.Courses.Any(s => s.CourseCode.Equals(courseCode)))
            {
                return new ValidationResult("This course code is already exist!!");
            }
            return ValidationResult.Success;
        }
    }
}
