using Project.App.DesignPatterns.Repositories;
using Project.Modules.Scores.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class EditCheckScoreNumberValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //EditCreateScoreRequest valueInput = value as EditCreateScoreRequest;
            //var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            //var ScoreTypeSubject = db.ScoreTypeSubjects.FindByCondition(x => x.ScoreTypeSubjectId.Equals(valueInput.ScoreTypeSubjectId)).FirstOrDefault();
            //var ScoreType = db.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(ScoreTypeSubject.ScoreTypeId)).FirstOrDefault();
            //if (ScoreType == null)
            //{
            //    return new ValidationResult("ScoreTypeDoNotExists");
            //}
            //int number = ScoreType.ScoreTypeNumber;
            //int score = valueInput.ScoreSubject.Count();
            //if (number != score)
            //{
            //    return new ValidationResult("TheScoreColumnIsNotEnough");
            //}
            return ValidationResult.Success;
        }
    }
}
