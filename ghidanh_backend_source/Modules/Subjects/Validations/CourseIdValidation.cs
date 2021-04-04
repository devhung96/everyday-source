using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Validations
{
    public class CourseIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            string courseId = value.ToString();
            if (string.IsNullOrEmpty(courseId))
            {
                return ValidationResult.Success;
            }
            MariaDBContext mariaDBContext = validationContext.GetService<MariaDBContext>();
            
            if (!mariaDBContext.Courses.Any(c => c.CourseId.Equals(courseId)))
            {
                return new ValidationResult("This courseId is not exist");
            }
            return ValidationResult.Success;
        }
    }
}
