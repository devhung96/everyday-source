using Project.App.Databases;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Validations
{
    public class ValidateExtraSubjectIds : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> Ids = value as List<string>;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            List<string> SubjectIds = mariaDB.Subjects.Select(m => m.SubjectId).ToList();
            var data = Ids.Except(SubjectIds);
            if (data.Count()>0)
            {
                return new ValidationResult("SubjectNotExist"+data.ToString());
            }
            return ValidationResult.Success;

        }
    }
}