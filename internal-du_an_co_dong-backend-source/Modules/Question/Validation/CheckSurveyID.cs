using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class CheckSurveyID : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("không được bỏ trống.");
            }
            var _dbcontext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            //var SurveyID = _dbcontext.Surveys.FirstOrDefault(x => x.SurveyID == int.Parse(value.ToString()));
            //if (SurveyID is null)
            //{
            //    return new ValidationResult("Survey not exists !!");
            //}
            return ValidationResult.Success;
        }
    }
}
