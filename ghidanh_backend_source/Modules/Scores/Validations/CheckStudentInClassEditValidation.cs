using Project.App.DesignPatterns.Repositories;
using Project.Modules.Scores.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckStudentInClassEditValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EditCreateScoreRequest request = value as EditCreateScoreRequest;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool registration = db.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(request.ClassId) && x.StudentId.Equals(request.StudentId)).Any();
            if (registration == false)
            {
                return new ValidationResult("StudentsDoExistInThisClassroomAndViceVersa");
            }
            return ValidationResult.Success;
        }
    }
}
