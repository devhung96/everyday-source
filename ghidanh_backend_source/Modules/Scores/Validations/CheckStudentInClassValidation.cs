using Project.App.DesignPatterns.Repositories;
using Project.Modules.Scores.Requests;
using Project.Modules.Scores.Services;
using Project.Modules.Students.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckStudentInClassValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CreateScoreRequest request = value as CreateScoreRequest;
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
