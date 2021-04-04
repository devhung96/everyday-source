using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckCourseSubjectIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool CourseSubjectId = db.CourseSubjects.FindByCondition(x => x.CourseSubjectId.Equals(id)).Any();
            if(CourseSubjectId == false)
            {
                return new ValidationResult("CourseSubjectIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
