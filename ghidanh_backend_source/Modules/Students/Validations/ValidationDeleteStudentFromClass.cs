using Project.App.Databases;
using Project.Modules.Classes.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.Students.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Students.Validations
{
    public class ValidationDeleteStudentFromClass:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DeleteStudentFromClassRequest request = value as DeleteStudentFromClassRequest;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Class @class = mariaDB.Classes.Find(request.ClassId);
            if(@class is null)
            {
                return new ValidationResult("ClassNotExist");
            }
            Student student = mariaDB.Students.Find(request.StudentId);
            if (student is null)
            {
                return new ValidationResult("StudentNotExist");
            }
            string id = mariaDB.RegistrationStudies.Where(m => m.ClassId.Equals(request.ClassId) && m.StudentId.Equals(request.StudentId))
                                                   .Select(m => m.RegistrationStudyId)
                                                   .FirstOrDefault();

            if(String.IsNullOrEmpty(id))
            {
                return new ValidationResult("StudentNotExistInClass");
            }
            return ValidationResult.Success;
        }
    }
}
