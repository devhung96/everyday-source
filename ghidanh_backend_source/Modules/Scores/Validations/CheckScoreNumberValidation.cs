using Project.App.DesignPatterns.Repositories;
using Project.Modules.Scores.Requests;
using Project.Modules.ScoreTypes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckScoreNumberValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //CreateScoreRequest valueInput = value as CreateScoreRequest;
            //var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            //var ScoreTypeSubject = db.ScoreTypeSubjects.FindByCondition(x => x.ScoreTypeSubjectId.Equals(valueInput.CourseSubjectId)).FirstOrDefault();
            //var ScoreType = db.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(ScoreTypeSubject.ScoreTypeId)).FirstOrDefault();
            //if (ScoreType == null)
            //{
            //    return new ValidationResult("ScoreTypeDoNotExists");
            //}
            //int score = valueInput.ScoreSubject.Count();
            return ValidationResult.Success;
        }
    }
}
