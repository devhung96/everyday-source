using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckStudentIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool studentId = db.Students.FindByCondition(x => x.StudentId.Equals(id)).Any();
            if (studentId == false)
            {
                return new ValidationResult("StudentIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
