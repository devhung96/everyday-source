using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Validations
{
    public class CheckScoreTypeIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool CourseId = db.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(id)).Any();
            if (CourseId == false)
            {
                return new ValidationResult("CheckScoreTypeIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
