using Project.App.Databases;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Validations
{
    public class ValidateSubjectId : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           string Id = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            if(String.IsNullOrEmpty(Id))
            {
                return ValidationResult.Success;
            }
            Subject subject = mariaDB.Subjects.Find(Id);
            if (subject is null)
            {
                return new ValidationResult("SubjectNotExist");
            }
            return ValidationResult.Success;

        }
    }
}