using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Validations
{
    public class SubjectGroupIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            string subjectGroupId = value.ToString();
            if (string.IsNullOrEmpty(subjectGroupId))
            {
                return ValidationResult.Success;
            }
            MariaDBContext mariaDBContext = validationContext.GetService<MariaDBContext>();
            if (!mariaDBContext.SubjectGroups.Any(sg => sg.SubjectGroupId.Equals(subjectGroupId)))
            {
                return new ValidationResult("This subjectGroupId is not exist");
            }
            return ValidationResult.Success;
        }
    }
}
