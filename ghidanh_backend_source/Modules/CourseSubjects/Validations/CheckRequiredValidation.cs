using Project.Modules.CourseSubjects.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Validations
{
    public class CheckRequiredValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CreateCourseSubjectRequest checkRequired = value as CreateCourseSubjectRequest;
            if (checkRequired.CheckRequired < 0 && checkRequired.CheckRequired > checkRequired.ColumnNumber)
            {
                return new ValidationResult("CheckRequiredErro");
            }
            return ValidationResult.Success;
        }
    }
}
