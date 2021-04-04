using Project.Modules.CourseSubjects.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Validations
{
    public class CheckRequiredEditValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EditCourseSubjectRequest checkRequired = value as EditCourseSubjectRequest;
            if (checkRequired.CheckRequired < 0 && checkRequired.CheckRequired > checkRequired.ColumnNumber)
            {
                return new ValidationResult("CheckRequiredErro");
            }
            return ValidationResult.Success;
        }
    }
}
